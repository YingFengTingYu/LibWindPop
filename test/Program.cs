﻿using LibWindPop.Images.PtxRsb.Handler;
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

            var rsbOption = new Option<FileInfo?>(
                name: "--rsbfile",
                description: "rsb file");

            rsbOption.AddAlias("-r");

            var ptxOption = new Option<string?>(
                name: "--ptxhandler",
                description: "ptx handler");

            ptxOption.AddAlias("-p");

            var rootCommand = new RootCommand("rsbtool v1.2.1 author: YingFengTingTu");

            var unpackCommand = new Command("unpack", "Unpack rsb");

            rootCommand.Add(unpackCommand);

            unpackCommand.AddOption(rsbOption);
            unpackCommand.AddOption(unpackOption);
            unpackCommand.AddOption(ptxOption);

            unpackCommand.SetHandler((FileInfo? rsb, DirectoryInfo? unpack, string? ptxHandler) =>
            {
                if (rsb != null)
                {
                    Unpack(rsb.FullName, unpack?.FullName, ptxHandler);
                }
            }, rsbOption, unpackOption, ptxOption);
            var packCommand = new Command("pack", "Pack rsb");

            rootCommand.Add(packCommand);

            packCommand.AddOption(rsbOption);
            packCommand.AddOption(unpackOption);

            packCommand.SetHandler((DirectoryInfo? unpack, FileInfo? rsb) =>
            {
                if (unpack != null)
                {
                    Pack(unpack.FullName, rsb?.FullName);
                }
            }, unpackOption, rsbOption);

            return await rootCommand.InvokeAsync(args);
        }

        //static void Main(string[] args)
        //{
        //    //PtxRsbCoder.Encode(
        //    //    "D:\\ffaad8230279d1e490af4bfe03029917.png",
        //    //    "D:\\test.ptx",
        //    //    new NativeFileSystem(),
        //    //    new ConsoleLogger(0, true),
        //    //    30,
        //    //    out uint w,
        //    //    out uint h,
        //    //    out uint p,
        //    //    out uint a,
        //    //    nameof(PtxHandleriOSV5)
        //    //    );
        //    //PtxRsbCoder.Decode(
        //    //    "D:\\test.ptx",
        //    //    "D:\\test.png",
        //    //    new NativeFileSystem(),
        //    //    new ConsoleLogger(0, true),
        //    //    4096,
        //    //    4096,
        //    //    2048,
        //    //    30,
        //    //    0,
        //    //    nameof(PtxHandleriOSV5)
        //    //    );

        //    //const string inPath = "D:\\ipad2_1.0.226789_main.rsb";
        //    //const string outPath = "D:\\ipad2_1.0.226789_main_rsb_unpack";
        //    //Pack(outPath, inPath + ".new");
        //    //Unpack(inPath, outPath, nameof(PtxHandleriOSV5));
        //}

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
            RsbContentPipelineManager.AddContentPipeline(
                outPath,
                nameof(SquarePVRTCImages),
                true,
                fileSystem,
                logger
                );
        }
    }
}
