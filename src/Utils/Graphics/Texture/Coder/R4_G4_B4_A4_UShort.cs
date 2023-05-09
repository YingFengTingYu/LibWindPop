using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public unsafe struct R4_G4_B4_A4_UShort : ITextureCoder, IPitchableTextureCoder, IOpenGLES20Texture
    {
        public static int OpenGLES20InternalFormat => 0x1908; // GL_RGBA

        public static int OpenGLES20Format => 0x1908; // GL_RGBA

        public static int OpenGLES20Type => 0x8033; // GL_UNSIGNED_SHORT_4_4_4_4

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
                    tempColor.Red = (byte)BitHelper.FourBitToEightBit(colorU16 >> 12);
                    tempColor.Green = (byte)BitHelper.FourBitToEightBit(colorU16 >> 8);
                    tempColor.Blue = (byte)BitHelper.FourBitToEightBit(colorU16 >> 4);
                    tempColor.Alpha = (byte)BitHelper.FourBitToEightBit(colorU16);
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
                    colorU16 = (ushort)((BitHelper.EightBitToFourBit(tempColor.Red) << 12) | (BitHelper.EightBitToFourBit(tempColor.Green) << 8) | (BitHelper.EightBitToFourBit(tempColor.Blue) << 4) | BitHelper.EightBitToFourBit(tempColor.Alpha));
                    BinaryPrimitives.WriteUInt16LittleEndian(dstData.Slice(tempDataIndex, 2), colorU16);
                    tempDataIndex += 2;
                }
            }
        }
    }
}
