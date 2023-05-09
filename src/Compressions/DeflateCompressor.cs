using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Compressions
{
    public static class DeflateCompressor
    {
        public static void Compress(string unPath, string deflatePath, IFileSystem fileSystem, ILogger logger, int level, bool throwException)
        {
            try
            {
                using (Stream defStream = fileSystem.Create(deflatePath))
                {
                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(defStream, new Deflater(level, true)))
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

        public static void Uncompress(string deflatePath, string unPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            try
            {
                using (Stream defStream = fileSystem.OpenRead(deflatePath))
                {
                    using (Stream unStream = fileSystem.Create(unPath))
                    {
                        using (InflaterInputStream zlibStream = new InflaterInputStream(defStream, new Inflater(true)))
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
