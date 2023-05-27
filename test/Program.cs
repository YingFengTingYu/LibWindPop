using LibWindPop.Packs.Pak;
using LibWindPop.Packs.Pak.ContentPipeline;
using LibWindPop.Packs.Rsb;
using LibWindPop.Packs.Rsb.ContentPipeline;
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
            //    new ConsoleLogger(0, true),
            //    false,
            //    true
            //    );
            //PakContentPipelineManager.AddContentPipeline(
            //    "D:\\main_pak_unpack",
            //    nameof(PakPtxXbox360AutoEncoder),
            //    true,
            //    new NativeFileSystem(),
            //    new ConsoleLogger(0, true)
            //    );
            PakPacker.Pack(
                "D:\\main_pak_unpack",
                "F:\\Games\\pvzx360\\main.pak",
                new NativeFileSystem(),
                new ConsoleLogger()
                );
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
