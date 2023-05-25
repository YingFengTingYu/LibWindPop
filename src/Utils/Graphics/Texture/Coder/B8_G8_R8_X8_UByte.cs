using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct B8_G8_R8_X8_UByte : ITextureCoder, IPitchableTextureCoder, IDirectXTexture, IXbox360D3D9Texture
    {
        public static DXGI_FORMAT DirectXFormat => DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM;

        public static D3DFORMAT Xbox360D3D9Format => D3DFORMAT.D3DFMT_LIN_X8R8G8B8.GetLEFormat();

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
                    tempColor.Blue = srcData[tempDataIndex++];
                    tempColor.Green = srcData[tempDataIndex++];
                    tempColor.Red = srcData[tempDataIndex++];
                    tempDataIndex++;
                    tempColor.Alpha = 0xFF;
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
                    dstData[tempDataIndex++] = tempColor.Blue;
                    dstData[tempDataIndex++] = tempColor.Green;
                    dstData[tempDataIndex++] = tempColor.Red;
                    dstData[tempDataIndex++] = 0xFF;
                }
            }
        }
    }
}
