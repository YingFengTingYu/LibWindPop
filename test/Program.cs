using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Packs.Rsb;
using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System.CommandLine;

namespace LibWindPop.Test
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var unpackOption = new Option<DirectoryInfo?>(
                name: "--unpackfolder",
                description: "unpack folder");

            unpackOption.AddAlias("-u");

            var obbOption = new Option<FileInfo?>(
                name: "--obbfile",
                description: "obb file");

            obbOption.AddAlias("-o");

            var rootCommand = new RootCommand("rsbtool v1.0 author: YingFengTingTu");

            var unpackCommand = new Command("unpack", "Unpack obb");

            rootCommand.Add(unpackCommand);

            unpackCommand.AddOption(obbOption);
            unpackCommand.AddOption(unpackOption);

            unpackCommand.SetHandler((FileInfo? obb, DirectoryInfo? unpack) =>
            {
                if (obb != null)
                {
                    Unpack(obb.FullName, unpack?.FullName);
                }
            }, obbOption, unpackOption);

            var packCommand = new Command("pack", "Pack obb");

            rootCommand.Add(packCommand);

            packCommand.AddOption(obbOption);
            packCommand.AddOption(unpackOption);

            packCommand.SetHandler((DirectoryInfo? unpack, FileInfo? obb) =>
            {
                if (unpack != null)
                {
                    Pack(unpack.FullName, obb?.FullName);
                }
            }, unpackOption, obbOption);

            return await rootCommand.InvokeAsync(args);
        }

        //static void Main2(string[] args)
        //{
        //    while (true)
        //    {
        //        Console.WriteLine("Enter mode: 1 = unpack, 2 = pack");
        //        string? text = Console.ReadLine();
        //        if (int.TryParse(text, out int value))
        //        {
        //            if (value == 1)
        //            {
        //                Console.WriteLine("Enter obb path...");
        //                string? obbPath = Console.ReadLine();
        //                if (string.IsNullOrEmpty(obbPath))
        //                {
        //                    Console.WriteLine("Error obb path!");
        //                    continue;
        //                }
        //                if (obbPath.StartsWith('"') && obbPath.EndsWith('"'))
        //                {
        //                    obbPath = obbPath[1..^1];
        //                }
        //                Console.WriteLine("Enter unpack folder path...");
        //                string? unpackPath = Console.ReadLine();
        //                if (string.IsNullOrEmpty(unpackPath))
        //                {
        //                    Console.WriteLine("Error unpack folder path!");
        //                    continue;
        //                }
        //                if (unpackPath.StartsWith('"') && unpackPath.EndsWith('"'))
        //                {
        //                    unpackPath = unpackPath[1..^1];
        //                }
        //                TimeTaskRun(() => Unpack(obbPath, unpackPath));
        //                continue;
        //            }
        //            else if (value == 2)
        //            {
                        
        //                Console.WriteLine("Enter unpack folder path...");
        //                string? unpackPath = Console.ReadLine();
        //                if (string.IsNullOrEmpty(unpackPath))
        //                {
        //                    Console.WriteLine("Error unpack folder path!");
        //                    continue;
        //                }
        //                if (unpackPath.StartsWith('"') && unpackPath.EndsWith('"'))
        //                {
        //                    unpackPath = unpackPath[1..^1];
        //                }
        //                Console.WriteLine("Enter obb path...");
        //                string? obbPath = Console.ReadLine();
        //                if (string.IsNullOrEmpty(obbPath))
        //                {
        //                    Console.WriteLine("Error obb path!");
        //                    continue;
        //                }
        //                if (obbPath.StartsWith('"') && obbPath.EndsWith('"'))
        //                {
        //                    obbPath = obbPath[1..^1];
        //                }
        //                TimeTaskRun(() => Pack(unpackPath, obbPath));
        //                continue;
        //            }
        //        }
        //        Console.WriteLine("Error mode!");
        //    }
        //}

        //static void TimeTaskRun(Action action)
        //{
        //    DateTime startTime = DateTime.Now;
        //    action?.Invoke();
        //    DateTime endTime = DateTime.Now;
        //    Console.WriteLine($"Use Time {(endTime - startTime).TotalSeconds}s");
        //}

        static void Pack(string inPath, string? obbPath)
        {
            obbPath ??= inPath + "_pack.obb";
            string innerPath = Path.Combine(inPath, "obb");
            string rsbPath = Path.Combine(innerPath, "main.rsb");
            string rsbUnpackPath = Path.Combine(inPath, "rsb");
            IFileSystem fileSystem = new NativeFileSystem();
            ILogger logger = new ConsoleLogger(0, true);
            RsbPacker.Pack(
                rsbUnpackPath,
                rsbPath,
                fileSystem,
                logger
                );
            ObbCoder.Pack(innerPath, obbPath);
        }

        static void Unpack(string obbPath, string? outPath)
        {
            outPath ??= obbPath + "_unpack";
            string innerPath = Path.Combine(outPath, "obb");
            ObbCoder.Unpack(obbPath, innerPath);
            string rsbPath = Path.Combine(innerPath, "main.rsb");
            string rsbUnpackPath = Path.Combine(outPath, "rsb");
            IFileSystem fileSystem = new NativeFileSystem();
            ILogger logger = new ConsoleLogger(0, true);
            RsbUnpacker.Unpack(
                rsbPath,
                rsbUnpackPath,
                fileSystem,
                logger,
                nameof(PtxHandlerAndroidV2),
                false
                );
            RsbContentPipelineManager.AddContentPipeline(
                rsbUnpackPath,
                nameof(EncodePtxFromPng),
                true,
                fileSystem,
                logger
                );
            RsbContentPipelineManager.AddContentPipeline(
                rsbUnpackPath,
                nameof(AtlasCreator),
                true,
                fileSystem,
                logger
                );
        }
    }
}
