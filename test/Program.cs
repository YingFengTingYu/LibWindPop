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
            //const uint w = 128;
            //const uint h = 128;
            //const int fmt = 0;
            //PtxCoder.Encode("D:\\BG_DESERT_LEVEL03_1080_01.png", "D:\\BG_DESERT_LEVEL03_1080_01.PTX", new NativeFileSystem(), new ConsoleLogger(), fmt, out uint width, out uint height, out uint pitch, out uint alphaSize, nameof(PtxHandlerPS4V1));
            //Console.WriteLine(width);
            //Console.WriteLine(height);
            //Console.WriteLine(pitch);
            //PtxCoder.Decode("D:\\BG_DESERT_LEVEL03_1080_01.PTX", "D:\\BG_DESERT_LEVEL03_1080_01_2.png", new NativeFileSystem(), new ConsoleLogger(), width, height, pitch, fmt, alphaSize, nameof(PtxHandlerPS4V1));
            //RsbUnpacker.Unpack(
            //    "D:\\main.rsb",
            //    "D:\\main_unpack",
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    nameof(PtxHandlerPS4V1),
            //    false,
            //    false
            //    );
            //RsbContentPipelineManager.AddContentPipeline(
            //    "D:\\main_unpack",
            //    nameof(EncodePtxFromPng),
            //    new NativeFileSystem(),
            //    new ConsoleLogger(),
            //    true
            //    );
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
