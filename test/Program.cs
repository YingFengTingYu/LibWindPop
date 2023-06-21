using LibWindPop.Images.PtxRsb.Handler;
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
            RsbUnpacker.Unpack(
                "D:\\main.rsb",
                "D:\\main_rsb_unpack",
                new NativeFileSystem(),
                new ConsoleLogger(0, true),
                nameof(PtxHandlerAndroidV2),
                false
                );
            RsbContentPipelineManager.AddContentPipeline(
                "D:\\main_rsb_unpack",
                nameof(EncodePtxFromPng),
                true,
                new NativeFileSystem(),
                new ConsoleLogger(0, true)
                );
            RsbContentPipelineManager.AddContentPipeline(
                "D:\\main_rsb_unpack",
                nameof(AtlasCreator),
                true,
                new NativeFileSystem(),
                new ConsoleLogger(0, true)
                );
            RsbPacker.Pack(
                "D:\\main_rsb_unpack",
                "D:\\main2.rsb",
                new NativeFileSystem(),
                new ConsoleLogger(0, true)
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
