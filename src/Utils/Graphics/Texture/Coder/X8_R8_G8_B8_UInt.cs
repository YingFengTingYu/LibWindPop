using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct X8_R8_G8_B8_UInt : ITextureCoder, IPitchableTextureCoder, IXbox360D3D9Texture
    {
        public static D3DFORMAT Xbox360D3D9Format => D3DFORMAT.D3DFMT_LIN_A8R8G8B8.GetLEFormat();

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
            uint colorU32;
            for (int y = 0; y < height; y++)
            {
                tempDataIndex = srcDataIndex;
                srcDataIndex += pitch;
                row = dstBitmap[y];
                for (int x = 0; x < width; x++)
                {
                    colorU32 = BinaryPrimitives.ReadUInt32LittleEndian(srcData.Slice(tempDataIndex, 4));
                    tempDataIndex += 4;
                    tempColor.Alpha = 0xFF;
                    tempColor.Red = (byte)BitHelper.EightBitToEightBit(colorU32 >> 16);
                    tempColor.Green = (byte)BitHelper.EightBitToEightBit(colorU32 >> 8);
                    tempColor.Blue = (byte)BitHelper.EightBitToEightBit(colorU32);
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
            uint colorU32;
            for (int y = 0; y < height; y++)
            {
                tempDataIndex = dstDataIndex;
                dstDataIndex += pitch;
                row = srcBitmap[y];
                for (int x = 0; x < width; x++)
                {
                    tempColor = row[x];
                    colorU32 = (((uint)0xFF << 24) | ((uint)BitHelper.EightBitToFourBit(tempColor.Red) << 16) | ((uint)BitHelper.EightBitToFourBit(tempColor.Green) << 8) | (uint)BitHelper.EightBitToFourBit(tempColor.Blue));
                    BinaryPrimitives.WriteUInt32LittleEndian(dstData.Slice(tempDataIndex, 4), colorU32);
                    tempDataIndex += 4;
                }
            }
        }
    }
}
