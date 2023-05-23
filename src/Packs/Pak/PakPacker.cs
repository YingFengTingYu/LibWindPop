using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Packs.Pak.ContentPipeline;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using LibWindPop.Utils.Logger;
using System;
using System.IO;
using System.Text;

namespace LibWindPop.Packs.Pak
{
    public static class PakPacker
    {
        public static void Pack(string unpackPath, string pakPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(pakPath, nameof(pakPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            // Update cache
            logger.Log("Building content pipeline...", 0);
            PakContentPipelineManager.StartBuildContentPipeline(unpackPath, fileSystem, logger, throwException);

            logger.Log("Get pack info...", 0);

            // define base path
            PakUnpackPathProvider paths = new PakUnpackPathProvider(unpackPath, fileSystem);

            Encoding encoding = EncodingType.ansi.GetEncoding();

            PakPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<PakPackInfo>(paths.InfoPackInfoPath, 0u, fileSystem, logger, throwException);
            if (packInfo == null)
            {
                logger.LogError("Pack info is null", 0, throwException);
            }
            else if (packInfo.RecordFiles == null)
            {
                logger.LogError("Record file info is null", 0, throwException);
            }
            else
            {
                using (Stream pakStream = fileSystem.Create(pakPath))
                {
                    if (packInfo.UseEncrypt)
                    {
                        using (XorStream xorStream = new XorStream(pakStream, 0xF7))
                        {
                            PackInternal(xorStream, paths, packInfo, fileSystem, logger, encoding, throwException);
                        }
                    }
                    else
                    {
                        PackInternal(pakStream, paths, packInfo, fileSystem, logger, encoding, throwException);
                    }
                }
            }

            PakContentPipelineManager.EndBuildContentPipeline(pakPath, unpackPath, fileSystem, logger, throwException);
        }

        private static unsafe void PackInternal(Stream pakStream, PakUnpackPathProvider paths, PakPackInfo packInfo, IFileSystem fileSystem, ILogger logger, Encoding encoding, bool throwException)
        {
            if (packInfo.RecordFiles != null)
            {
                using (NativeMemoryOwner zeroSpanOwner = new NativeMemoryOwner(packInfo.UseAlign ? 0x2000u : 0x0u))
                {
                    Span<byte> zeroSpan = zeroSpanOwner.AsSpan();
                    zeroSpan.Clear();
                    uint headerSize = 9u;
                    uint fileInfoSize = packInfo.UseZlib ? 18u : 14u;
                    foreach (PakPackFileInfo pakFileInfo in packInfo.RecordFiles)
                    {
                        if (pakFileInfo.Path != null)
                        {
                            headerSize += (uint)encoding.GetByteCount(pakFileInfo.Path);
                        }
                        headerSize += fileInfoSize;
                    }
                    pakStream.SetLength(headerSize);
                    pakStream.Seek(0, SeekOrigin.Begin);
                    pakStream.WriteUInt32LE(0xBAC04AC0u);
                    pakStream.WriteUInt32LE(0x0u);
                    long headerPosition = 8;
                    long filePosition = headerSize;
                    uint rawSize, storeSize;
                    foreach (PakPackFileInfo pakFileInfo in packInfo.RecordFiles)
                    {
                        pakStream.Seek(filePosition, SeekOrigin.Begin);
                        if (packInfo.UseAlign)
                        {
                            int align = pakFileInfo.UseAlign4K ? 0x1000 : 0x8;
                            int alignByteCount = align - (int)((pakStream.Position + 2) & (align - 1));
                            pakStream.WriteUInt16LE((ushort)alignByteCount);
                            pakStream.Write(zeroSpan[..alignByteCount]);
                        }
                        if (string.IsNullOrEmpty(pakFileInfo.Path))
                        {
                            rawSize = 0u;
                            storeSize = 0u;
                        }
                        else
                        {
                            using (Stream fileStream = fileSystem.OpenRead(paths.GetFilePath(pakFileInfo.Path)))
                            {
                                if (packInfo.UseZlib && pakFileInfo.UseZlib)
                                {
                                    long startPos = pakStream.Position;
                                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(pakStream, new Deflater(packInfo.ZlibLevel)))
                                    {
                                        zlibStream.IsStreamOwner = false;
                                        fileStream.CopyTo(zlibStream);
                                    }
                                    storeSize = (uint)(pakStream.Position - startPos);
                                    rawSize = (uint)fileStream.Length;
                                }
                                else
                                {
                                    fileStream.CopyTo(pakStream);
                                    storeSize = (uint)fileStream.Length;
                                    rawSize = 0u;
                                }
                            }
                        }
                        filePosition = pakStream.Position;
                        pakStream.Seek(headerPosition, SeekOrigin.Begin);
                        pakStream.WriteUInt8(0x0);
                        pakStream.WriteStringWithUInt8Head(pakFileInfo.Path, encoding);
                        pakStream.WriteUInt32LE(storeSize);
                        if (packInfo.UseZlib)
                        {
                            pakStream.WriteUInt32LE(rawSize);
                        }
                        pakStream.WriteInt64LE(pakFileInfo.TimeUtc.ToFileTimeUtc());
                        headerPosition = pakStream.Position;
                    }
                    pakStream.Seek(headerPosition, SeekOrigin.Begin);
                    pakStream.WriteUInt8(0x80);
                }
            }
        }
    }
}
