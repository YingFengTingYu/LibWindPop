using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Images.PtxXbox360
{
    public static class PtxXbox360Coder
    {
        public static void Encode(string pngPath, string ptxPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            using (Stream pngStream = fileSystem.OpenRead(pngPath))
            {
                using (Stream ptxStream = fileSystem.Create(ptxPath))
                {
                    Encode(pngStream, ptxStream, logger);
                }
            }
        }

        public static unsafe void Encode(Stream pngStream, Stream ptxStream, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ImageCoder.PeekImageInfo(pngStream, out int pngWidth, out int pngHeight, out ImageFormat imageFormat);
            uint width, height, pitch, dataSize;
            width = (uint)pngWidth;
            height = (uint)pngHeight;
            uint aW = (width + 3u) / 4u * 4u;
            uint aH = (height + 3u) / 4u * 4u;
            pitch = (aW * 4u + 511u) / 512u * 512u;
            dataSize = pitch * aH / 4u + pitch;
            using (NativeBitmap bitmap = new NativeBitmap(pngWidth, pngHeight))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                ImageCoder.DecodeImage(pngStream, refBitmap, imageFormat);
                using (NativeMemoryOwner owner = new NativeMemoryOwner(dataSize))
                {
                    owner.Fill(0xCD);
                    new RGBA_BC3_UShortBE().Encode(refBitmap, owner.AsSpan(), (int)width, (int)height, (int)pitch);
                    ptxStream.Write(owner.Pointer, owner.Size);
                }
            }
            ptxStream.WriteUInt32BE(width);
            ptxStream.WriteUInt32BE(height);
            ptxStream.WriteUInt32BE(pitch);
            ptxStream.WriteUInt32LE((uint)D3DFORMAT.D3DFMT_LIN_DXT5);
        }

        public static void Decode(string ptxPath, string pngPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            using (Stream ptxStream = fileSystem.OpenRead(ptxPath))
            {
                using (Stream pngStream = fileSystem.Create(pngPath))
                {
                    Decode(ptxStream, pngStream, logger);
                }
            }
        }

        public static unsafe void Decode(Stream ptxStream, Stream pngStream, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            long pos = ptxStream.Position;
            ptxStream.Seek(ptxStream.Length - 16, SeekOrigin.Begin);
            uint width = ptxStream.ReadUInt32BE();
            uint height = ptxStream.ReadUInt32BE();
            uint pitch = ptxStream.ReadUInt32BE();
            D3DFORMAT d3dFormat = (D3DFORMAT)ptxStream.ReadUInt32LE();
            ptxStream.Seek(pos, SeekOrigin.Begin);
            if (d3dFormat != D3DFORMAT.D3DFMT_LIN_DXT5)
            {
                logger.LogError($"Ptx magic mismatch: D3DFMT_LIN_DXT5(LE) expected but value is 0x{(uint)d3dFormat:X8}");
            }
            using (NativeBitmap bitmap = new NativeBitmap((int)width, (int)height))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                uint memSize = (uint)(ptxStream.Length - pos - 16);
                using (NativeMemoryOwner owner = new NativeMemoryOwner(memSize))
                {
                    ptxStream.Read(owner.Pointer, owner.Size);
                    new RGBA_BC3_UShortBE().Decode(owner.AsSpan(), (int)width, (int)height, (int)pitch, refBitmap);
                }
                ImageCoder.EncodeImage(pngStream, refBitmap, ImageFormat.Png, null);
            }
        }
    }
}
