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
    public static class PopCapZlibCompressor
    {
        public static void Compress(string unPath, string soePath, IFileSystem fileSystem, ILogger logger, int level, bool throwException)
        {
            try
            {
                using (Stream popStream = fileSystem.Create(soePath))
                {
                    using (Stream unStream = fileSystem.OpenRead(unPath))
                    {
                        Span<byte> popHeader = stackalloc byte[8];
                        BinaryPrimitives.WriteUInt32LittleEndian(popHeader, 0xDEADFED4);
                        BinaryPrimitives.WriteUInt32LittleEndian(popHeader.Slice(4, 4), (uint)unStream.Length);
                        popStream.Write(popHeader);
                        using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(popStream, new Deflater(level)))
                        {
                            zlibStream.IsStreamOwner = false;
                            unStream.CopyTo(zlibStream);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is not LoggerException)
            {
                logger.LogException(ex, 0, throwException);
            }
        }

        public static void Uncompress(string soePath, string unPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            try
            {
                using (Stream popStream = fileSystem.OpenRead(soePath))
                {
                    Span<byte> popHeader = stackalloc byte[8];
                    popStream.Read(popHeader);
                    uint magic = BinaryPrimitives.ReadUInt32LittleEndian(popHeader);
                    uint uncompressedSize = BinaryPrimitives.ReadUInt32LittleEndian(popHeader.Slice(8, 4));
                    if (magic != 0xDEADFED4)
                    {
                        logger.LogError($"PopCap zlib magic mismatch: 0xDEADFED4(LE) expected but value is {magic:X8}", 1, throwException);
                    }
                    using (Stream unStream = fileSystem.Create(unPath))
                    {
                        using (InflaterInputStream zlibStream = new InflaterInputStream(popStream))
                        {
                            zlibStream.IsStreamOwner = false;
                            zlibStream.CopyTo(unStream);
                        }
                        Debug.Assert(unStream.Length == uncompressedSize);
                    }
                }
            }
            catch (Exception ex) when (ex is not LoggerException)
            {
                logger.LogException(ex, 0, throwException);
            }
        }
    }
}
