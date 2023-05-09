using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static unsafe class ATCCoder
    {
        public static void DecodeATCBlock(ReadOnlySpan<byte> atcword, Span<YFColor> decode_data)
        {
            ATCDecoder.DecodeColorWord(atcword, decode_data);
        }

        public static void EncodeATCBlock(Span<byte> atcword, ReadOnlySpan<YFColor> encode_data)
        {
            // ATCDecoder.DecodeColorWord(atcword.Slice(8, 8), decode_data, false);
        }

        public static void DecodeATCBlockWithExplicitAlpha(ReadOnlySpan<byte> atcword, Span<YFColor> decode_data)
        {
            ATCDecoder.DecodeColorWord(atcword.Slice(8, 8), decode_data);
            ATCDecoder.ModifyExplicitAlphaWord(atcword[..8], decode_data);
        }

        public static void EncodeATCBlockWithExplicitAlpha(Span<byte> atcword, ReadOnlySpan<YFColor> encode_data)
        {
            // ATCDecoder.DecodeColorWord(atcword.Slice(8, 8), decode_data, false);
        }

        public static void DecodeATCBlockWithInterpolatedAlpha(ReadOnlySpan<byte> atcword, Span<YFColor> decode_data)
        {
            ATCDecoder.DecodeColorWord(atcword.Slice(8, 8), decode_data);
            ATCDecoder.ModifyInterpolatedAlphaWord(atcword[..8], decode_data);
        }

        public static void EncodeATCBlockWithInterpolatedAlpha(Span<byte> atcword, ReadOnlySpan<YFColor> encode_data)
        {
            // ATCDecoder.DecodeColorWord(atcword.Slice(8, 8), decode_data, false);
        }

        private static class ATCDecoder
        {
            public static void ModifyExplicitAlphaWord(ReadOnlySpan<byte> texPtr, Span<YFColor> color)
            {
                ulong buf = BinaryPrimitives.ReadUInt64BigEndian(texPtr);
                ulong a;
                for (int i = 0; i < 16; i++)
                {
                    a = buf & 0xF;
                    buf >>= 4;
                    color[i].Alpha = (byte)BitHelper.FourBitToEightBit(a);
                }
            }

            public static void ModifyInterpolatedAlphaWord(ReadOnlySpan<byte> texPtr, Span<YFColor> color)
            {
                byte* alpha_buffer = stackalloc byte[8];
                ulong alpha_flags = BinaryPrimitives.ReadUInt64LittleEndian(texPtr[..8]) >> 16;
                alpha_buffer[0] = texPtr[0];
                alpha_buffer[1] = texPtr[1];
                if (alpha_buffer[0] > alpha_buffer[1])
                {
                    alpha_buffer[2] = (byte)((6 * alpha_buffer[0] + alpha_buffer[1]) / 7);
                    alpha_buffer[3] = (byte)((5 * alpha_buffer[0] + (alpha_buffer[1] << 1)) / 7);
                    alpha_buffer[4] = (byte)(((alpha_buffer[0] << 2) + 3 * alpha_buffer[1]) / 7);
                    alpha_buffer[5] = (byte)((3 * alpha_buffer[0] + (alpha_buffer[1] << 2)) / 7);
                    alpha_buffer[6] = (byte)(((alpha_buffer[0] << 1) + 5 * alpha_buffer[1]) / 7);
                    alpha_buffer[7] = (byte)((alpha_buffer[0] + 6 * alpha_buffer[1]) / 7);
                }
                else
                {
                    alpha_buffer[2] = (byte)(((alpha_buffer[0] << 2) + alpha_buffer[1]) / 5);
                    alpha_buffer[3] = (byte)((3 * alpha_buffer[0] + (alpha_buffer[1] << 1)) / 5);
                    alpha_buffer[4] = (byte)(((alpha_buffer[0] << 1) + 3 * alpha_buffer[1]) / 5);
                    alpha_buffer[5] = (byte)((alpha_buffer[0] + (alpha_buffer[1] << 2)) / 5);
                    alpha_buffer[6] = 0x0;
                    alpha_buffer[7] = 0xFF;
                }
                for (int i = 0; i < 16; i++)
                {
                    color[i].Alpha = alpha_buffer[alpha_flags & 0x7];
                    alpha_flags >>= 3;
                }
            }

            public static void DecodeColorWord(ReadOnlySpan<byte> texPtr, Span<YFColor> color)
            {
                YFColor* color_buffer = stackalloc YFColor[4];
                ushort c0 = BinaryPrimitives.ReadUInt16LittleEndian(texPtr[..2]);
                ushort c1 = BinaryPrimitives.ReadUInt16LittleEndian(texPtr.Slice(2, 2));
                uint color_flags = BinaryPrimitives.ReadUInt32LittleEndian(texPtr.Slice(4, 4));

                if ((c0 & 0x8000) != 0)
                {
                    // color2 = c0
                    color_buffer[2].Red = (byte)BitHelper.FiveBitToEightBit(c0 >> 10);
                    color_buffer[2].Green = (byte)BitHelper.FiveBitToEightBit(c0 >> 5);
                    color_buffer[2].Blue = (byte)BitHelper.FiveBitToEightBit(c0);
                    color_buffer[2].Alpha = 0xFF;
                    // color3 = c1
                    color_buffer[3].Red = (byte)BitHelper.FiveBitToEightBit(c1 >> 11);
                    color_buffer[3].Green = (byte)BitHelper.SixBitToEightBit(c1 >> 5);
                    color_buffer[3].Blue = (byte)BitHelper.FiveBitToEightBit(c1);
                    color_buffer[3].Alpha = 0xFF;
                    // color0 = 0
                    color_buffer[0].Red = 0x0;
                    color_buffer[0].Green = 0x0;
                    color_buffer[0].Blue = 0x0;
                    color_buffer[0].Alpha = 0xFF;
                    // color1 = ?
                    color_buffer[1].Red = (byte)(Math.Abs((color_buffer[2].Red << 2) - color_buffer[3].Red) >> 2);
                    color_buffer[1].Green = (byte)(Math.Abs((color_buffer[2].Green << 2) - color_buffer[3].Green) >> 2);
                    color_buffer[1].Blue = (byte)(Math.Abs((color_buffer[2].Blue << 2) - color_buffer[3].Blue) >> 2);
                    color_buffer[1].Alpha = 0xFF;
                }
                else
                {
                    // color0 = c0
                    color_buffer[0].Red = (byte)BitHelper.FiveBitToEightBit(c0 >> 10);
                    color_buffer[0].Green = (byte)BitHelper.FiveBitToEightBit(c0 >> 5);
                    color_buffer[0].Blue = (byte)BitHelper.FiveBitToEightBit(c0);
                    color_buffer[0].Alpha = 0xFF;
                    // color3 = c1
                    color_buffer[3].Red = (byte)BitHelper.FiveBitToEightBit(c1 >> 11);
                    color_buffer[3].Green = (byte)BitHelper.SixBitToEightBit(c1 >> 5);
                    color_buffer[3].Blue = (byte)BitHelper.FiveBitToEightBit(c1);
                    color_buffer[3].Alpha = 0xFF;
                    // color1 = (c0 * 5 + c1 * 3) / 8
                    color_buffer[1].Red = (byte)((color_buffer[0].Red * 5 + color_buffer[3].Red * 3 + 4) >> 3);
                    color_buffer[1].Green = (byte)((color_buffer[0].Green * 5 + color_buffer[3].Green * 3 + 4) >> 3);
                    color_buffer[1].Blue = (byte)((color_buffer[0].Blue * 5 + color_buffer[3].Blue * 3 + 4) >> 3);
                    color_buffer[1].Alpha = 0xFF;
                    // color2 = (c0 * 3 + c1 * 5) / 8
                    color_buffer[2].Red = (byte)((color_buffer[0].Red * 3 + color_buffer[3].Red * 5 + 4) >> 3);
                    color_buffer[2].Green = (byte)((color_buffer[0].Green * 3 + color_buffer[3].Green * 5 + 4) >> 3);
                    color_buffer[2].Blue = (byte)((color_buffer[0].Blue * 3 + color_buffer[3].Blue * 5 + 4) >> 3);
                    color_buffer[2].Alpha = 0xFF;
                }
                for (int i = 0; i < 16; i++)
                {
                    color[i] = color_buffer[color_flags & 0x3];
                    color_flags >>= 2;
                }
            }
        }
    }
}
