using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct RGBA_ASTC_8x8_UByte : ITextureCoder, IPitchableTextureCoder, IDirectXTexture, IOpenGLES20CompressedTexture
    {
        public static DXGI_FORMAT DirectXFormat => DXGI_FORMAT.DXGI_FORMAT_ASTC_8X8_UNORM;

        public static int OpenGLES20InternalFormat => 0x93B3; // GL_COMPRESSED_RGBA_ASTC_8x8_KHR

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, ((width + 7) / 8) * 16, dstBitmap);
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, ((width + 7) / 8) * 16);
        }

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
        {
            if (PVRTexLibHelper.Decode(srcData, PVRTexLib.PVRTexLibPixelFormat.ASTC_8x8, dstBitmap))
            {
                return;
            }
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            throw new NotSupportedException("PVRTexLib is required to decode ASTC 8x8 textures.");
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height, int pitch)
        {
            if (PVRTexLibHelper.Encode(srcBitmap, PVRTexLib.PVRTexLibPixelFormat.ASTC_8x8, dstData))
            {
                return;
            }
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            throw new NotSupportedException("PVRTexLib is required to encode ASTC 8x8 textures.");
        }
    }
}
