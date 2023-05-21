using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Packs.Pak.ContentPipeline;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LibWindPop.Packs.Pak
{
    public static class PakUnpacker
    {
        private record struct PakFilePair(string Path, uint StoreSize, uint RawSize, DateTime TimeUtc);

        public static void Unpack(string pakPath, string unpackPath, IFileSystem fileSystem, ILogger logger, bool useZlib, bool useAlign, bool throwException)
        {
            ArgumentNullException.ThrowIfNull(pakPath, nameof(pakPath));
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get unpack info...", 0);

            PakUnpackPathProvider paths = new PakUnpackPathProvider(unpackPath, fileSystem);

            Encoding encoding = EncodingType.ansi.GetEncoding();

            PakPackInfo packInfo = new PakPackInfo
            {
                ZlibLevel = 6
            };

            using (Stream pakRawStream = fileSystem.OpenRead(pakPath))
            {
                uint magic = pakRawStream.ReadUInt32LE();
                pakRawStream.Seek(0, SeekOrigin.Begin);
                
                if (magic == 0x4D37BD37u)
                {
                    // Xor 0xF7 pak
                    packInfo.UseEncrypt = true;
                    using (XorStream pakStream = new XorStream(pakRawStream, 0xF7))
                    {
                        UnpackInternal(pakStream, paths, packInfo, fileSystem, logger, useZlib, useAlign, encoding, throwException);
                    }
                }
                else
                {
                    // Raw pak
                    packInfo.UseEncrypt = false;
                    UnpackInternal(pakRawStream, paths, packInfo, fileSystem, logger, useZlib, useAlign, encoding, throwException);
                }
            }

            logger.Log("Save pack info...", 0);
            WindJsonSerializer.TrySerializeToFile(paths.InfoPackInfoPath, packInfo, 0u, fileSystem, logger, throwException);

            // Add Content Pipeline
            logger.Log("Create content pipeline...", 0);
            try
            {
                if (File.Exists(paths.InfoContentPipelinePath))
                {
                    File.Delete(paths.InfoContentPipelinePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, 0, throwException);
            }
            PakContentPipelineManager.AddContentPipeline(paths.UnpackPath, nameof(PakRebuildFile), true, fileSystem, logger, throwException);
        }

        private static void UnpackInternal(Stream pakStream, PakUnpackPathProvider paths, PakPackInfo packInfo, IFileSystem fileSystem, ILogger logger, bool useZlib, bool useAlign, Encoding encoding, bool throwException)
        {
            uint magic = pakStream.ReadUInt32LE();
            if (magic != 0xBAC04AC0u)
            {
                if (BinaryPrimitives.ReverseEndianness(magic) == 0x0FF512EDu)
                {
                    // Xmem unsupported
                    logger.LogError($"Pak magic mismatch: 0xBAC04AC0 expected but value is 0x{magic:X8}. If this pak is xmem file, please use xbdecompress.exe to uncompress it.", 0, throwException);
                }
                else
                {
                    logger.LogError($"Pak magic mismatch: 0xBAC04AC0 expected but value is 0x{magic:X8}", 1, throwException);
                }
            }
            uint version = pakStream.ReadUInt32LE();
            Debug.Assert(version == 0u);
            packInfo.UseAlign = useAlign;
            packInfo.UseZlib = useZlib;
            List<PakFilePair> fileList = new List<PakFilePair>(128);
            while (true)
            {
                byte contentType = pakStream.ReadUInt8();
                if ((contentType & 0x80) != 0)
                {
                    break;
                }
                byte pathSize = pakStream.ReadUInt8();
                string path = pakStream.ReadString(pathSize, encoding);
                uint storeSize = pakStream.ReadUInt32LE();
                uint rawSize = useZlib ? pakStream.ReadUInt32LE() : 0u;
                DateTime timeUtc = DateTime.FromFileTimeUtc(pakStream.ReadInt64LE());
                fileList.Add(new PakFilePair(path, storeSize, rawSize, timeUtc));
            }
            List<PakPackFileInfo> recordFiles = new List<PakPackFileInfo>(fileList.Count);
            for (int i = 0; i < fileList.Count; i++)
            {
                if (useAlign)
                {
                    // Align
                    ushort alignSize = pakStream.ReadUInt16LE();
                    pakStream.Seek(alignSize, SeekOrigin.Current);
                }
                PakPackFileInfo fileInfo = new PakPackFileInfo();
                fileInfo.Path = fileList[i].Path;
                fileInfo.UseZlib = useZlib && (fileList[i].RawSize != 0);
                fileInfo.UseAlign4K = useAlign && ((pakStream.Position & 0xFFF) == 0);
                fileInfo.TimeUtc = fileList[i].TimeUtc;
                using (Stream fileStream = fileSystem.Create(paths.GetFilePath(fileInfo.Path)))
                {
                    long backPos = pakStream.Position;
                    if (fileInfo.UseZlib)
                    {
                        using (InflaterInputStream zlibStream = new InflaterInputStream(pakStream))
                        {
                            zlibStream.IsStreamOwner = false; 
                            zlibStream.CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        pakStream.CopyLengthTo(fileStream, fileList[i].StoreSize);
                    }
                    pakStream.Seek(backPos + fileList[i].StoreSize, SeekOrigin.Begin);
                }
                recordFiles.Add(fileInfo);
            }
            packInfo.RecordFiles = recordFiles;
        }
    }
}
