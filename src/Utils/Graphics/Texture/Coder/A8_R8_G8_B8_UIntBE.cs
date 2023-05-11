using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gcm;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct A8_R8_G8_B8_UIntBE : ITextureCoder, IPitchableTextureCoder, IXbox360D3D9Texture, IGcmTexture
    {
        public static D3DFORMAT Xbox360D3D9Format => D3DFORMAT.D3DFMT_LIN_A8R8G8B8;

        public static TextureFormat GcmFormat => new TextureFormat(
            CellGcmEnum.CELL_GCM_LOCATION_MAIN,
            CellGcmEnum.CELL_GCM_FALSE,
            CellGcmEnum.CELL_GCM_TEXTURE_DIMENSION_2,
            CellGcmEnum.CELL_GCM_TEXTURE_A8R8G8B8 | CellGcmEnum.CELL_GCM_TEXTURE_LN | CellGcmEnum.CELL_GCM_TEXTURE_UN,
            1
            );

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
                    tempColor.Alpha = srcData[tempDataIndex++];
                    tempColor.Red = srcData[tempDataIndex++];
                    tempColor.Green = srcData[tempDataIndex++];
                    tempColor.Blue = srcData[tempDataIndex++];
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
                    dstData[tempDataIndex++] = tempColor.Alpha;
                    dstData[tempDataIndex++] = tempColor.Red;
                    dstData[tempDataIndex++] = tempColor.Green;
                    dstData[tempDataIndex++] = tempColor.Blue;
                }
            }
        }
    }
}
