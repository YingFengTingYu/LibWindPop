using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct RGBA_ASTC_6x6_UByte : ITextureCoder, IPitchableTextureCoder, IDirectXTexture, IOpenGLES20CompressedTexture
    {
        public static DXGI_FORMAT DirectXFormat => DXGI_FORMAT.DXGI_FORMAT_ASTC_6X6_UNORM;

        public static int OpenGLES20InternalFormat => 0x93B2; // GL_COMPRESSED_RGBA_ASTC_6x6_KHR

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, ((width + 5) / 6) * 16, dstBitmap);
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, ((width + 5) / 6) * 16);
        }

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
        {
            if (PVRTexLibHelper.Decode(srcData, PVRTexLib.PVRTexLibPixelFormat.ASTC_6x6, dstBitmap))
            {
                return;
            }
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            throw new NotSupportedException("PVRTexLib is required to decode ASTC 6x6 textures.");
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height, int pitch)
        {
            if (PVRTexLibHelper.Encode(srcBitmap, PVRTexLib.PVRTexLibPixelFormat.ASTC_6x6, dstData))
            {
                return;
            }
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            throw new NotSupportedException("PVRTexLib is required to encode ASTC 6x6 textures.");
        }
    }
}
