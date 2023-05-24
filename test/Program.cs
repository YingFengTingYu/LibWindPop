using LibWindPop.Images.PtxRsb;
using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Packs.Pak;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Test
{
    internal class Program
    {
        private static void TestTask()
        {
            //const string pakPath = "D:\\paks";
            //foreach (string pak in Directory.GetFiles(pakPath))
            //{
            //    PakUnpacker.Unpack(
            //        pak,
            //        pak + "_unpack",
            //        new NativeFileSystem(),
            //        new ConsoleLogger(),
            //        false,
            //        true,
            //        true
            //        );
            //}
            static void DecodePtxPS3(string folderPath)
            {
                foreach (string folder in Directory.GetDirectories(folderPath))
                {
                    DecodePtxPS3(folder);
                }
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    if (file.EndsWith(".ptx"))
                    {
                        using (Stream stream = File.OpenRead(file))
                        {
                            ImageCoder.PeekImageInfo(stream, out int w, out int h, out ImageFormat f);
                            using (NativeBitmap bitmap = new NativeBitmap(w, h))
                            {
                                RefBitmap refBitmap = bitmap.AsRefBitmap();
                                ImageCoder.DecodeImage(stream, refBitmap, f);
                                using (Stream outStream = File.Create(file + ".png"))
                                {
                                    ImageCoder.EncodeImage(outStream, refBitmap, ImageFormat.Png);
                                }
                            }
                        }
                    }
                }
            }
            //PakUnpacker.Unpack(
            //    "D:\\main.pak",
            //    "D:\\main_pak_unpack",
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    true,
            //    true,
            //    true
            //    );
            DecodePtxPS3("D:\\main_pak_unpack");
            //PakPacker.Pack(
            //    "D:\\main_pak_unpack",
            //    "D:\\main2.pak",
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    true
            //    );
            //PtxCoder.Decode(
            //    "D:\\DELAYLOAD_BACKGROUND_EGYPT_COMPRESSED_1536_00.PTX",
            //    "D:\\DELAYLOAD_BACKGROUND_EGYPT_COMPRESSED_1536_00.PNG",
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    4096,
            //    4096,
            //    2048,
            //    30,
            //    0,
            //    nameof(PtxHandleriOSV5)
            //    );
        }

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            TestTask();
            DateTime endTime = DateTime.Now;
            Console.WriteLine($"Use Time {(endTime - startTime).TotalSeconds}s");
        }
    }
}
