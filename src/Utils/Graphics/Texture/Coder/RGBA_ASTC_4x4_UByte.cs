using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct RGBA_ASTC_4x4_UByte : ITextureCoder, IPitchableTextureCoder, IDirectXTexture, IOpenGLES20CompressedTexture
    {
        public static DXGI_FORMAT DirectXFormat => DXGI_FORMAT.DXGI_FORMAT_ASTC_4X4_UNORM;

        public static int OpenGLES20InternalFormat => 0x93B0; // GL_COMPRESSED_RGBA_ASTC_4x4_KHR

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, ((width + 3) / 4) * 16, dstBitmap);
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, ((width + 3) / 4) * 16);
        }

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
        {
            if (PVRTexLibHelper.Decode(srcData, PVRTexLib.PVRTexLibPixelFormat.ASTC_4x4, dstBitmap))
            {
                return;
            }
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            throw new NotSupportedException("PVRTexLib is required to decode ASTC 4x4 textures.");
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height, int pitch)
        {
            if (PVRTexLibHelper.Encode(srcBitmap, PVRTexLib.PVRTexLibPixelFormat.ASTC_4x4, dstData))
            {
                return;
            }
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            throw new NotSupportedException("PVRTexLib is required to encode ASTC 4x4 textures.");
        }
    }
}