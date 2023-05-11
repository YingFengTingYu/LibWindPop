using LibWindPop.Compressions;
using LibWindPop.Images.PtxRsb;
using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Packs.Rsb;
using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Packs.Xpr;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PopCapZlibCompressor.Uncompress("D:\\zombie3.rsb.smf", "D:\\main.rsb", new NativeFileSystem(), new NullLogger(), false);
            //const uint w = 256;
            //const uint h = 256;
            //const int fmt = 147;
            //PtxCoder.Encode("D:\\COWBOYMINIGAMEMODULE_640_00.png", "D:\\COWBOYMINIGAMEMODULE_640_00.PTX", new NativeFileSystem(), new ConsoleLogger(), fmt, out uint width, out uint height, out uint pitch, out uint alphaSize, nameof(PtxHandlerPVZ2CNAndroidV3));
            //Console.WriteLine(width);
            //Console.WriteLine(height);
            //Console.WriteLine(pitch);
            //PtxCoder.Decode("D:\\COWBOYMINIGAMEMODULE_640_00.PTX", "D:\\COWBOYMINIGAMEMODULE_640_00_2.png", new NativeFileSystem(), new ConsoleLogger(), width, height, pitch, fmt, alphaSize, nameof(PtxHandlerPVZ2CNAndroidV3));
            RsbUnpacker.Unpack(
                "D:\\main.rsb",
                "D:\\main_unpack",
                new NativeFileSystem(),
                new ConsoleLogger(),
                nameof(PtxHandlerPVZ2CNAndroidV3),
                false,
                false
                );
            RsbContentPipelineManager.AddContentPipeline(
                "D:\\main_unpack",
                nameof(EncodePtxFromPng),
                new NativeFileSystem(),
                new ConsoleLogger(),
                true
                );
            //RsbPacker.Pack(
            //    "D:\\main_unpack",
            //    "D:\\main.rsb2",
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    true
            //    );
        }
    }
}
