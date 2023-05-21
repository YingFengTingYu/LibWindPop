using LibWindPop.Images.PtxRsb;
using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Packs.Pak;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Test
{
    internal class Program
    {
        private static void TestTask()
        {
            //PakUnpacker.Unpack(
            //    "D:\\main.pak",
            //    "D:\\main_pak_unpack",
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    false,
            //    false,
            //    true
            //    );
            PakPacker.Pack(
                "D:\\main_pak_unpack",
                "D:\\main2.pak",
                new NativeFileSystem(),
                new ConsoleLogger(),
                true
                );
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
