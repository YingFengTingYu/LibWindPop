using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;

namespace LibWindPop.Compressions
{
    public static class SoeCompressor
    {
        public static void Compress(string unPath, string soePath, IFileSystem fileSystem, ILogger logger, int level, bool throwException)
        {
            try
            {
                Span<byte> soeHeader = stackalloc byte[20];
                BinaryPrimitives.WriteUInt32LittleEndian(soeHeader, 0x00454F53u); // SOE\0
                BinaryPrimitives.WriteUInt32LittleEndian(soeHeader.Slice(4, 4), 0x00424C5Au); // ZLB\0
                BinaryPrimitives.WriteUInt32LittleEndian(soeHeader.Slice(16, 4), 1u); // ZLB\0
                using (Stream soeStream = fileSystem.Create(soePath))
                {
                    soeStream.SetLength(20);
                    soeStream.Seek(20, SeekOrigin.Begin);
                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(soeStream, new Deflater(level)))
                    {
                        zlibStream.IsStreamOwner = false;
                        using (Stream unStream = fileSystem.OpenRead(unPath))
                        {
                            BinaryPrimitives.WriteUInt32LittleEndian(soeHeader.Slice(8, 4), (uint)unStream.Length);
                            unStream.CopyTo(zlibStream);
                        }
                    }
                    BinaryPrimitives.WriteUInt32LittleEndian(soeHeader.Slice(12, 4), (uint)(soeStream.Length - 20));
                    soeStream.Seek(0, SeekOrigin.Begin);
                    soeStream.Write(soeHeader);
                }
            }
            catch (Exception ex) when (ex is not LoggerException)
            {
                logger.LogException(ex);
            }
        }

        public static void Uncompress(string soePath, string unPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            try
            {
                using (Stream soeStream = fileSystem.OpenRead(soePath))
                {
                    Span<byte> soeHeader = stackalloc byte[20];
                    soeStream.Read(soeHeader);
                    uint magic = BinaryPrimitives.ReadUInt32LittleEndian(soeHeader);
                    uint compression = BinaryPrimitives.ReadUInt32LittleEndian(soeHeader.Slice(4, 4));
                    uint uncompressedSize = BinaryPrimitives.ReadUInt32LittleEndian(soeHeader.Slice(8, 4));
                    uint compressedSize = BinaryPrimitives.ReadUInt32LittleEndian(soeHeader.Slice(12, 4));
                    uint version = BinaryPrimitives.ReadUInt32LittleEndian(soeHeader.Slice(16, 4));
                    if (magic != 0x00454F53u) // SOE\0
                    {
                        logger.LogError($"Soe magic mismatch: SOE\0 expected but value is {magic:X8}");
                    }
                    if (compression != 0x00424C5Au) // ZLB\0
                    {
                        logger.LogError($"Soe compression mismatch: ZLB\0 expected but value is {compression:X8}");
                    }
                    Debug.Assert(version == 1u);
                    using (Stream unStream = fileSystem.Create(unPath))
                    {
                        using (InflaterInputStream zlibStream = new InflaterInputStream(soeStream))
                        {
                            zlibStream.IsStreamOwner = false;
                            zlibStream.CopyTo(unStream);
                        }
                        Debug.Assert(unStream.Length == uncompressedSize);
                        Debug.Assert((soeStream.Position - 20) == compressedSize);
                    }
                }
            }
            catch (Exception ex) when (ex is not LoggerException)
            {
                logger.LogException(ex);
            }
        }
    }
}
