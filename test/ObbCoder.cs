using System.IO.Compression;

namespace LibWindPop.Test
{
    public static class ObbCoder
    {
        public static void Unpack(string obbPath, string outPath)
        {
            ZipFile.ExtractToDirectory(obbPath, outPath, true);
        }

        public static void Pack(string inPath, string obbPath)
        {
            if (File.Exists(obbPath))
            {
                File.Delete(obbPath);
            }
            ZipFile.CreateFromDirectory(inPath, obbPath);
        }
    }
}
