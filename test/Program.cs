using LibWindPop.Packs.Pak;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Test
{
    internal class Program
    {
        private static void TestTask()
        {
            PakUnpacker.Unpack(
                "D:\\main_mac.pak",
                "D:\\main_mac_pak_unpack",
                new NativeFileSystem(),
                new ConsoleLogger(),
                false,
                false,
                true
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
