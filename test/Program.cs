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
        //static async Task<int> Main(string[] args)
        //{
        //    var unpackOption = new Option<DirectoryInfo?>(
        //        name: "--unpackfolder",
        //        description: "unpack folder");

        //    unpackOption.AddAlias("-u");

        //    var rsbOption = new Option<FileInfo?>(
        //        name: "--rsbfile",
        //        description: "rsb file");

        //    rsbOption.AddAlias("-r");

        //    var ptxOption = new Option<string?>(
        //        name: "--ptxhandler",
        //        description: "ptx handler");

        //    ptxOption.AddAlias("-p");

        //    var rootCommand = new RootCommand("rsbtool v1.0 author: YingFengTingTu");

        //    var unpackCommand = new Command("unpack", "Unpack rsb");

        //    rootCommand.Add(unpackCommand);

        //    unpackCommand.AddOption(rsbOption);
        //    unpackCommand.AddOption(unpackOption);
        //    unpackCommand.AddOption(ptxOption);

        //    unpackCommand.SetHandler((FileInfo? rsb, DirectoryInfo? unpack, string? ptxHandler) =>
        //    {
        //        if (rsb != null)
        //        {
        //            Unpack(rsb.FullName, unpack?.FullName, ptxHandler);
        //        }
        //    }, rsbOption, unpackOption, ptxOption);
        //    var packCommand = new Command("pack", "Pack rsb");

        //    rootCommand.Add(packCommand);

        //    packCommand.AddOption(rsbOption);
        //    packCommand.AddOption(unpackOption);

        //    packCommand.SetHandler((DirectoryInfo? unpack, FileInfo? rsb) =>
        //    {
        //        if (unpack != null)
        //        {
        //            Pack(unpack.FullName, rsb?.FullName);
        //        }
        //    }, unpackOption, rsbOption);

        //    return await rootCommand.InvokeAsync(args);
        //}

        static void Main(string[] args)
        {
            const string inPath = "D:\\main.176.com.ea.game.pvz2_row.obb";
            const string outPath = "D:\\main.176.com.ea.game.pvz2_row_obb_unpack";
            RsbUnpacker.Unpack(
                inPath,
                outPath,
                new NativeFileSystem(),
                new ConsoleLogger(0, true),
                nameof(PtxHandlerAndroidV3),
                false
                );
            RsbContentPipelineManager.AddContentPipeline(
                outPath,
                nameof(EncodePtxFromPng),
                true,
                new NativeFileSystem(),
                new ConsoleLogger(0, true));
        }

        static void TimeTaskRun(Action action)
        {
            DateTime startTime = DateTime.Now;
            action?.Invoke();
            DateTime endTime = DateTime.Now;
            Console.WriteLine($"Use Time {(endTime - startTime).TotalSeconds}s");
        }

        static void Pack(string inPath, string? rsbPath)
        {
            if (rsbPath == null)
            {
                rsbPath = inPath + ".rsb";
            }
            IFileSystem fileSystem = new NativeFileSystem();
            ILogger logger = new ConsoleLogger(0, true);
            RsbPacker.Pack(
                inPath,
                rsbPath,
                fileSystem,
                logger
                );
        }

        static void Unpack(string rsbPath, string? outPath = null, string? ptxHandler = null)
        {
            outPath ??= rsbPath + "_unpack";
            ptxHandler ??= nameof(PtxHandlerAndroidV3);
            IFileSystem fileSystem = new NativeFileSystem();
            ILogger logger = new ConsoleLogger(0, true);
            RsbUnpacker.Unpack(
                rsbPath,
                outPath,
                fileSystem,
                logger,
                ptxHandler,
                false
                );
            RsbContentPipelineManager.AddContentPipeline(
                outPath,
                nameof(EncodePtxFromPng),
                true,
                fileSystem,
                logger
                );
        }
    }
}
