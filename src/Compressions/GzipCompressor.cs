using ICSharpCode.SharpZipLib.GZip;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Compressions
{
    public static class GzipCompressor
    {
        public static void Compress(string unPath, string gzipPath, IFileSystem fileSystem, ILogger logger, int level, bool throwException)
        {
            try
            {
                using (Stream gzStream = fileSystem.Create(gzipPath))
                {
                    using (GZipOutputStream zlibStream = new GZipOutputStream(gzStream))
                    {
                        zlibStream.SetLevel(level);
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
                logger.LogException(ex);
            }
        }

        public static void Uncompress(string gzipPath, string unPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            try
            {
                using (Stream gzStream = fileSystem.OpenRead(gzipPath))
                {
                    using (Stream unStream = fileSystem.Create(unPath))
                    {
                        using (GZipInputStream zlibStream = new GZipInputStream(gzStream))
                        {
                            zlibStream.IsStreamOwner = false;
                            zlibStream.CopyTo(unStream);
                        }
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
