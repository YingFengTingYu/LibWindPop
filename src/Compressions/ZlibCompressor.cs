using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Compressions
{
    public static class ZlibCompressor
    {
        public static void Compress(string unPath, string zlibPath, IFileSystem fileSystem, ILogger logger, int level, bool throwException)
        {
            try
            {
                using (Stream zlbStream = fileSystem.Create(zlibPath))
                {
                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(zlbStream, new Deflater(level)))
                    {
                        zlibStream.IsStreamOwner = false;
                        using (Stream unStream = fileSystem.OpenRead(unPath))
                        {
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

        public static void Uncompress(string zlibPath, string unPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            try
            {
                using (Stream zlbStream = fileSystem.OpenRead(zlibPath))
                {
                    using (Stream unStream = fileSystem.Create(unPath))
                    {
                        using (InflaterInputStream zlibStream = new InflaterInputStream(zlbStream))
                        {
                            zlibStream.IsStreamOwner = false;
                            zlibStream.CopyTo(unStream);
                        }
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
