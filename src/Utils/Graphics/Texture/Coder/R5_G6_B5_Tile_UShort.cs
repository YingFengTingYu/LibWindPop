using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct R5_G6_B5_Tile_UShort : ITextureCoder, IOpenGLES20Texture
    {
        public static int OpenGLES20InternalFormat => 0x1907; // GL_RGB

        public static int OpenGLES20Format => 0x1907; // GL_RGB

        public static int OpenGLES20Type => 0x8363; // GL_UNSIGNED_SHORT_5_6_5

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            YFColor tempColor;
            int tempDataIndex = 0;
            ushort colorU16;
            for (int yTile = 0; yTile < height; yTile += 32)
            {
                for (int xTile = 0; xTile < width; xTile += 32)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            if ((xTile + x) < width && (yTile + y) < height)
                            {
                                colorU16 = BinaryPrimitives.ReadUInt16LittleEndian(srcData.Slice(tempDataIndex, 2));
                                tempColor.Red = (byte)BitHelper.FiveBitToEightBit(colorU16 >> 11);
                                tempColor.Green = (byte)BitHelper.SixBitToEightBit(colorU16 >> 5);
                                tempColor.Blue = (byte)BitHelper.FiveBitToEightBit(colorU16);
                                tempColor.Alpha = 0xFF;
                                dstBitmap[xTile + x, yTile + y] = tempColor;
                            }
                            tempDataIndex += 2;
                        }
                    }
                }
            }
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            YFColor tempColor;
            int tempDataIndex = 0;
            ushort colorU16;
            for (int yTile = 0; yTile < height; yTile += 32)
            {
                for (int xTile = 0; xTile < width; xTile += 32)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            if ((xTile + x) < width && (yTile + y) < height)
                            {
                                tempColor = srcBitmap[xTile + x, yTile + y];
                                colorU16 = (ushort)((BitHelper.EightBitToFiveBit(tempColor.Red) << 11) | (BitHelper.EightBitToSixBit(tempColor.Green) << 5) | BitHelper.EightBitToFiveBit(tempColor.Blue));
                            }
                            else
                            {
                                colorU16 = 0;
                            }
                            BinaryPrimitives.WriteUInt16LittleEndian(dstData.Slice(tempDataIndex, 2), colorU16);
                            tempDataIndex += 2;
                        }
                    }
                }
            }
        }
    }
}
