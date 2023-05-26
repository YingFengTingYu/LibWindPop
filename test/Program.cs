using LibWindPop.Images.PtxPS3;
using LibWindPop.Images.PtxRsb;
using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Packs.Pak;
using LibWindPop.Packs.Pak.ContentPipeline;
using LibWindPop.Packs.Rsb;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Graphics.FormatProvider.Dds;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Test
{
    internal class Program
    {
        private static void TestTask()
        {
            RsbUnpacker.Unpack("D:\\main.rsb", "D:\\main_rsb_unpack", new NativeFileSystem(), new ConsoleLogger(), nameof(PtxHandlerXbox360V1), false);
            RsbPacker.Pack("D:\\main_rsb_unpack", "D:\\main2.rsb", new NativeFileSystem(), new ConsoleLogger());
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
