using ICSharpCode.SharpZipLib.BZip2;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Compressions
{
    public static class Bzip2Compressor
    {
        public static void Compress(string unPath, string bzip2Path, IFileSystem fileSystem, ILogger logger, int level)
        {
            try
            {
                using (Stream bzStream = fileSystem.Create(bzip2Path))
                {
                    using (BZip2OutputStream bzip2Stream = new BZip2OutputStream(bzStream, level))
                    {
                        bzip2Stream.IsStreamOwner = false;
                        using (Stream unStream = fileSystem.OpenRead(unPath))
                        {
                            unStream.CopyTo(bzip2Stream);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is not LoggerException)
            {
                logger.LogException(ex);
            }
        }

        public static void Uncompress(string bzip2Path, string unPath, IFileSystem fileSystem, ILogger logger)
        {
            try
            {
                using (Stream bzStream = fileSystem.OpenRead(bzip2Path))
                {
                    using (Stream unStream = fileSystem.Create(unPath))
                    {
                        using (BZip2InputStream bzip2Stream = new BZip2InputStream(bzStream))
                        {
                            bzip2Stream.IsStreamOwner = false;
                            bzip2Stream.CopyTo(unStream);
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
