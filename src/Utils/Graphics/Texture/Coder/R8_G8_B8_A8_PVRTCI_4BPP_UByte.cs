﻿using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public unsafe struct R8_G8_B8_A8_PVRTCI_4BPP_UByte : ITextureCoder, IOpenGLES20CompressedTexture
    {
        public static int OpenGLES20InternalFormat => 0x8C02; // GL_COMPRESSED_RGBA_PVRTC_4BPPV1_IMG

        public void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || !BitHelper.IsPowerOfTwo(width) || !BitHelper.IsPowerOfTwo(height));
            fixed (YFColor* color = dstBitmap.Data)
            {
                fixed (byte* data = srcData)
                {
                    PVRTCCoder.DecodeTexture_RGBA_PVRTCI_4BPP(data, color, (uint)width, (uint)height);
                }
            }
        }

        public void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || !BitHelper.IsPowerOfTwo(width) || !BitHelper.IsPowerOfTwo(height));
            fixed (YFColor* color = srcBitmap.Data)
            {
                fixed (byte* data = dstData)
                {
                    PVRTCCoder.EncodeTexture_RGBA_PVRTCI_4BPP(data, color, (uint)width, (uint)height);
                }
            }
        }
    }
}
