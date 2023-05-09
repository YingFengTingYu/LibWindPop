using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public unsafe struct R5_G5_B5_A1_UShort : ITextureCoder, IPitchableTextureCoder, IOpenGLES20Texture
    {
        public static int OpenGLES20InternalFormat => 0x1908; // GL_RGBA

        public static int OpenGLES20Format => 0x1908; // GL_RGBA

        public static int OpenGLES20Type => 0x8034; // GL_UNSIGNED_SHORT_5_5_5_1

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
                    colorU16 = BinaryPrimitives.ReadUInt16LittleEndian(srcData.Slice(tempDataIndex, 2));
                    tempDataIndex += 2;
                    tempColor.Red = (byte)BitHelper.FiveBitToEightBit(colorU16 >> 11);
                    tempColor.Green = (byte)BitHelper.FiveBitToEightBit(colorU16 >> 6);
                    tempColor.Blue = (byte)BitHelper.FiveBitToEightBit(colorU16 >> 1);
                    tempColor.Alpha = (byte)BitHelper.OneBitToEightBit(colorU16);
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
                    colorU16 = (ushort)((BitHelper.EightBitToFiveBit(tempColor.Red) << 11) | (BitHelper.EightBitToFiveBit(tempColor.Green) << 6) | (BitHelper.EightBitToFiveBit(tempColor.Blue) << 1) | BitHelper.EightBitToOneBit(tempColor.Alpha));
                    BinaryPrimitives.WriteUInt16LittleEndian(dstData.Slice(tempDataIndex, 2), colorU16);
                    tempDataIndex += 2;
                }
            }
        }
    }
}
