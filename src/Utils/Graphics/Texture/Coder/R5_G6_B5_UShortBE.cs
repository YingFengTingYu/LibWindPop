using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public unsafe struct R5_G6_B5_UShortBE : ITextureCoder, IPitchableTextureCoder, IXbox360D3D9Texture
    {
        public static D3DFORMAT Xbox360D3D9Format => D3DFORMAT.D3DFMT_LIN_R5G6B5;

        public void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, width << 1, dstBitmap);
        }

        public void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, width << 1);
        }

        public void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            int tempDataIndex;
            YFColor tempColor;
            Span<YFColor> row;
            int srcDataIndex = 0;
            ushort colorU16;
            for (int y = 0; y < height; y++)
            {
                tempDataIndex = srcDataIndex;
                srcDataIndex += pitch;
                row = dstBitmap[y];
                for (int x = 0; x < width; x++)
                {
                    colorU16 = BinaryPrimitives.ReadUInt16BigEndian(srcData.Slice(tempDataIndex, 2));
                    tempDataIndex += 2;
                    tempColor.Red = (byte)BitHelper.FiveBitToEightBit(colorU16 >> 11);
                    tempColor.Green = (byte)BitHelper.SixBitToEightBit(colorU16 >> 5);
                    tempColor.Blue = (byte)BitHelper.FiveBitToEightBit(colorU16);
                    tempColor.Alpha = 0xFF;
                    row[x] = tempColor;
                }
            }
        }

        public void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height, int pitch)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0 || pitch < 0);
            int tempDataIndex;
            YFColor tempColor;
            Span<YFColor> row;
            int dstDataIndex = 0;
            ushort colorU16;
            for (int y = 0; y < height; y++)
            {
                tempDataIndex = dstDataIndex;
                dstDataIndex += pitch;
                row = srcBitmap[y];
                for (int x = 0; x < width; x++)
                {
                    tempColor = row[x];
                    colorU16 = (ushort)((BitHelper.EightBitToFiveBit(tempColor.Red) << 11) | (BitHelper.EightBitToSixBit(tempColor.Green) << 5) | BitHelper.EightBitToFiveBit(tempColor.Blue));
                    BinaryPrimitives.WriteUInt16BigEndian(dstData.Slice(tempDataIndex, 2), colorU16);
                    tempDataIndex += 2;
                }
            }
        }
    }
}
