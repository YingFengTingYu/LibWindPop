using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct A4_R4_G4_B4_UShortBE : ITextureCoder, IPitchableTextureCoder, IXbox360D3D9Texture
    {
        public static D3DFORMAT Xbox360D3D9Format => D3DFORMAT.D3DFMT_LIN_A4R4G4B4;

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            Decode(srcData, width, height, width << 1, dstBitmap);
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            Encode(srcBitmap, dstData, width, height, width << 1);
        }

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, int pitch, RefBitmap dstBitmap)
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
                    tempColor.Alpha = (byte)BitHelper.FourBitToEightBit(colorU16 >> 12);
                    tempColor.Red = (byte)BitHelper.FourBitToEightBit(colorU16 >> 8);
                    tempColor.Green = (byte)BitHelper.FourBitToEightBit(colorU16 >> 4);
                    tempColor.Blue = (byte)BitHelper.FourBitToEightBit(colorU16);
                    row[x] = tempColor;
                }
            }
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dst_data, int width, int height, int pitch)
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
                    colorU16 = (ushort)((BitHelper.EightBitToFourBit(tempColor.Alpha) << 12) | (BitHelper.EightBitToFourBit(tempColor.Red) << 8) | (BitHelper.EightBitToFourBit(tempColor.Green) << 4) | BitHelper.EightBitToFourBit(tempColor.Blue));
                    BinaryPrimitives.WriteUInt16BigEndian(dst_data.Slice(tempDataIndex, 2), colorU16);
                    tempDataIndex += 2;
                }
            }
        }
    }
}
