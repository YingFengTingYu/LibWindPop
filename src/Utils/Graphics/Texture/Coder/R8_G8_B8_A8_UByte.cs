﻿using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct R8_G8_B8_A8_UByte : ITextureCoder, IPitchableTextureCoder, IDirectXTexture, IOpenGLES20Texture
    {
        public static DXGI_FORMAT DirectXFormat => DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;

        public static int OpenGLES20InternalFormat => 0x1908; // GL_RGBA

        public static int OpenGLES20Format => 0x1908; // GL_RGBA

        public static int OpenGLES20Type => 0x1401; // GL_UNSIGNED_BYTE

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, width << 2, dstBitmap);
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, width << 2);
        }

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            int tempDataIndex;
            YFColor tempColor;
            Span<YFColor> row;
            int srcDataIndex = 0;
            for (int y = 0; y < height; y++)
            {
                tempDataIndex = srcDataIndex;
                srcDataIndex += pitch;
                row = dstBitmap[y];
                for (int x = 0; x < width; x++)
                {
                    tempColor.Red = srcData[tempDataIndex++];
                    tempColor.Green = srcData[tempDataIndex++];
                    tempColor.Blue = srcData[tempDataIndex++];
                    tempColor.Alpha = srcData[tempDataIndex++];
                    row[x] = tempColor;
                }
            }
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height, int pitch)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            int tempDataIndex;
            YFColor tempColor;
            Span<YFColor> row;
            int dstDataIndex = 0;
            for (int y = 0; y < height; y++)
            {
                tempDataIndex = dstDataIndex;
                dstDataIndex += pitch;
                row = srcBitmap[y];
                for (int x = 0; x < width; x++)
                {
                    tempColor = row[x];
                    dstData[tempDataIndex++] = tempColor.Red;
                    dstData[tempDataIndex++] = tempColor.Green;
                    dstData[tempDataIndex++] = tempColor.Blue;
                    dstData[tempDataIndex++] = tempColor.Alpha;
                }
            }
        }
    }
}
