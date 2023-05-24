using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct R8_G8_B8_A8_DXT5_UByte : ITextureCoder, IPitchableTextureCoder, IXbox360D3D9Texture, IOpenGLES20CompressedTexture
    {
        public static D3DFORMAT Xbox360D3D9Format => D3DFORMAT.D3DFMT_LIN_DXT5.GetLEFormat();

        public static int OpenGLES20InternalFormat => 0x83F3; // GL_COMPRESSED_RGBA_S3TC_DXT5_EXT

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, (width + 3) / 4 * 16, dstBitmap);
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, (width + 3) / 4 * 16);
        }

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            int tempDataIndex;
            Span<YFColor> decodeBlockData = stackalloc YFColor[16];
            int srcDataIndex = 0;
            for (int yTile = 0; yTile < height; yTile += 4)
            {
                tempDataIndex = srcDataIndex;
                srcDataIndex += pitch;
                for (int xTile = 0; xTile < width; xTile += 4)
                {
                    BCCoder.DecodeDXT5Block(srcData.Slice(tempDataIndex, 16), decodeBlockData);
                    tempDataIndex += 16;
                    for (int y = 0; y < 4; y++)
                    {
                        if ((yTile + y) >= height)
                        {
                            break;
                        }
                        for (int x = 0; x < 4; x++)
                        {
                            if ((xTile + x) >= width)
                            {
                                break;
                            }
                            dstBitmap[xTile | x, yTile | y] = decodeBlockData[(y << 2) | x];
                        }
                    }
                }
            }
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height, int pitch)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            int tempDataIndex;
            Span<YFColor> encodeBlockData = stackalloc YFColor[16];
            int srcDataIndex = 0;
            for (int yTile = 0; yTile < height; yTile += 4)
            {
                tempDataIndex = srcDataIndex;
                srcDataIndex += pitch;
                for (int xTile = 0; xTile < width; xTile += 4)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            encodeBlockData[(y << 2) | x] = ((yTile + y) >= height || (xTile + x) >= width) ? YFColor.Transparent : srcBitmap[xTile | x, yTile | y];
                        }
                    }
                    BCCoder.EncodeDXT5Block(dstData.Slice(tempDataIndex, 16), encodeBlockData);
                    tempDataIndex += 16;
                }
            }
        }
    }
}
