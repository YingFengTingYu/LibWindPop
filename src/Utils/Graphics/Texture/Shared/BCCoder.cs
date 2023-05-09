using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static unsafe class BCCoder
    {
        public static void DecodeDXT5Block(ReadOnlySpan<byte> dxt5word, Span<YFColor> decode_data)
        {
            BCDecoder.DecodeColorWord(dxt5word.Slice(8, 8), decode_data, false);
            BCDecoder.ModifyInterpolatedAlphaWord(dxt5word[..8], decode_data);
        }

        public static void EncodeDXT5Block(Span<byte> dxt5word, ReadOnlySpan<YFColor> encode_data)
        {
            BCEncoder.EncodeColorWord(dxt5word.Slice(8, 8), encode_data, false);
            BCEncoder.EncodeInterpolatedAlphaWord(dxt5word[..8], encode_data);
        }

        public static void DecodeDXT3Block(ReadOnlySpan<byte> dxt3word, Span<YFColor> decode_data)
        {
            BCDecoder.DecodeColorWord(dxt3word.Slice(8, 8), decode_data, false);
            BCDecoder.ModifyExplicitAlphaWord(dxt3word[..8], decode_data);
        }

        public static void EncodeDXT3Block(Span<byte> dxt3word, ReadOnlySpan<YFColor> encode_data)
        {
            BCEncoder.EncodeColorWord(dxt3word.Slice(8, 8), encode_data, false);
            BCEncoder.EncodeExplicitAlphaWord(dxt3word[..8], encode_data);
        }

        public static void DecodeDXT1Block(ReadOnlySpan<byte> dxt1word, Span<YFColor> decode_data)
        {
            BCDecoder.DecodeColorWord(dxt1word, decode_data, false);
        }

        public static void EncodeDXT1Block(Span<byte> dxt1word, ReadOnlySpan<YFColor> encode_data)
        {
            BCEncoder.EncodeColorWord(dxt1word, encode_data, false);
        }

        public static void DecodeDXT1BlockWithAlpha(ReadOnlySpan<byte> dxt1word, Span<YFColor> decode_data)
        {
            BCDecoder.DecodeColorWord(dxt1word, decode_data, true);
        }

        public static void EncodeDXT1BlockWithAlpha(Span<byte> dxt1word, ReadOnlySpan<YFColor> encode_data)
        {
            BCEncoder.EncodeColorWord(dxt1word, encode_data, true);
        }

        /// <summary>
        /// 用于快速编码DXT的类
        /// </summary>
        private static class BCEncoder
        {
            /// <summary>
            /// 编码DXT4/5中线性插值的Alpha通道
            /// </summary>
            /// <param name="texPtr"></param>
            /// <param name="color"></param>
            public static void EncodeInterpolatedAlphaWord(Span<byte> texPtr, ReadOnlySpan<YFColor> color)
            {
                int maxAlpha = 0x0;
                int minAlpha = 0xFF;
                for (int i = 0; i < 16; i++)
                {
                    if (color[i].Alpha < minAlpha) minAlpha = color[i].Alpha;
                    if (color[i].Alpha > maxAlpha) maxAlpha = color[i].Alpha;
                }
                int inset = (maxAlpha - minAlpha) >> 4;
                minAlpha += inset;
                maxAlpha -= inset;
                if (minAlpha == maxAlpha)
                {
                    BinaryPrimitives.WriteUInt64LittleEndian(texPtr, (ulong)(maxAlpha | (minAlpha << 8)));
                }
                else
                {
                    byte* alphas = stackalloc byte[8];
                    alphas[0] = (byte)maxAlpha;
                    alphas[1] = (byte)minAlpha;
                    alphas[2] = (byte)((6 * maxAlpha + minAlpha) / 7);
                    alphas[3] = (byte)((5 * maxAlpha + (minAlpha << 1)) / 7);
                    alphas[4] = (byte)(((maxAlpha << 2) + 3 * minAlpha) / 7);
                    alphas[5] = (byte)((3 * maxAlpha + (minAlpha << 2)) / 7);
                    alphas[6] = (byte)(((maxAlpha << 1) + 5 * minAlpha) / 7);
                    alphas[7] = (byte)((maxAlpha + 6 * minAlpha) / 7);
                    ulong indices = 0;
                    byte buffer = 0;
                    for (int i = 15; i >= 0; i--)
                    {
                        int minDistance = int.MaxValue;
                        byte a = color[i].Alpha;
                        for (byte j = 0; j < 8; j++)
                        {
                            int dist = a - alphas[j];
                            if (dist < 0) dist = -dist;
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                buffer = j;
                            }
                        }
                        indices <<= 3;
                        indices |= buffer;
                    }
                    indices <<= 16;
                    indices |= (uint)maxAlpha | ((uint)minAlpha << 8);
                    BinaryPrimitives.WriteUInt64LittleEndian(texPtr, indices);
                }
            }

            /// <summary>
            /// 给DXT2/3用的编码A4
            /// </summary>
            /// <param name="texPtr"></param>
            /// <param name="color"></param>
            public static void EncodeExplicitAlphaWord(Span<byte> texPtr, ReadOnlySpan<YFColor> color)
            {
                uint buf = 0u;
                for (int i = 7; i >= 0; i--)
                {
                    buf <<= 4;
                    buf |= (uint)BitHelper.EightBitToFourBit(color[i].Alpha);
                }
                BinaryPrimitives.WriteUInt32LittleEndian(texPtr[..4], buf);
                buf = 0u;
                for (int i = 15; i >= 8; i--)
                {
                    buf <<= 4;
                    buf |= (uint)BitHelper.EightBitToFourBit(color[i].Alpha);
                }
                BinaryPrimitives.WriteUInt32LittleEndian(texPtr.Slice(4, 4), buf);
            }

            public static void EncodeColorWord(Span<byte> texPtr, ReadOnlySpan<YFColor> color, bool alpha)
            {
                // check alpha
                if (alpha)
                {
                    alpha = false;
                    for (int i = 0; i < 16; i++)
                    {
                        if ((color[i].Alpha & 0x80) == 0)
                        {
                            alpha = true;
                            break;
                        }
                    }
                }
                // Get min max color
                YFColor minColor, maxColor;
                YFColor inset;
                minColor.Red = 255;
                minColor.Green = 255;
                minColor.Blue = 255;
                minColor.Alpha = 255;
                maxColor.Red = 0;
                maxColor.Green = 0;
                maxColor.Blue = 0;
                maxColor.Alpha = 255;
                for (int i = 0; i < 16; i++)
                {
                    if (color[i].Red < minColor.Red) minColor.Red = color[i].Red;
                    if (color[i].Green < minColor.Green) minColor.Green = color[i].Green;
                    if (color[i].Blue < minColor.Blue) minColor.Blue = color[i].Blue;
                    if (color[i].Red > maxColor.Red) maxColor.Red = color[i].Red;
                    if (color[i].Green > maxColor.Green) maxColor.Green = color[i].Green;
                    if (color[i].Blue > maxColor.Blue) maxColor.Blue = color[i].Blue;
                }
                inset.Red = (byte)((maxColor.Red - minColor.Red) >> 4);
                inset.Green = (byte)((maxColor.Green - minColor.Green) >> 4);
                inset.Blue = (byte)((maxColor.Blue - minColor.Blue) >> 4);
                minColor.Red += inset.Red;
                minColor.Green += inset.Green;
                minColor.Blue += inset.Blue;
                maxColor.Red -= inset.Red;
                maxColor.Green -= inset.Green;
                maxColor.Blue -= inset.Blue;
                // get rgb565-like rgb888 color
                minColor.Red = (byte)((minColor.Red & 0xF8) | (minColor.Red >> 5));
                minColor.Green = (byte)((minColor.Green & 0xFC) | (minColor.Green >> 6));
                minColor.Blue = (byte)((minColor.Blue & 0xF8) | (minColor.Blue >> 5));
                maxColor.Red = (byte)((maxColor.Red & 0xF8) | (maxColor.Red >> 5));
                maxColor.Green = (byte)((maxColor.Green & 0xFC) | (maxColor.Green >> 6));
                maxColor.Blue = (byte)((maxColor.Blue & 0xF8) | (maxColor.Blue >> 5));
                // Get index
                uint color_flags = 0;
                ushort c0, c1;
                if (alpha)
                {
                    c0 = (ushort)((minColor.Red >> 3 << 11) | (minColor.Green >> 2 << 5) | (minColor.Blue >> 3));
                    c1 = (ushort)((maxColor.Red >> 3 << 11) | (maxColor.Green >> 2 << 5) | (maxColor.Blue >> 3));
                    Span<YFColor> color_buffer = stackalloc YFColor[3];
                    color_buffer[0].Red = minColor.Red;
                    color_buffer[0].Green = minColor.Green;
                    color_buffer[0].Blue = minColor.Blue;
                    color_buffer[0].Alpha = 0xFF;
                    color_buffer[1].Red = maxColor.Red;
                    color_buffer[1].Green = maxColor.Green;
                    color_buffer[1].Blue = maxColor.Blue;
                    color_buffer[1].Alpha = 0xFF;
                    color_buffer[2].Red = (byte)((minColor.Red + maxColor.Red) >> 1);
                    color_buffer[2].Green = (byte)((minColor.Green + maxColor.Green) >> 1);
                    color_buffer[2].Blue = (byte)((minColor.Blue + maxColor.Blue) >> 1);
                    color_buffer[2].Alpha = 0xFF;
                    for (int i = 15; i >= 0; i--)
                    {
                        int index = 0;
                        if ((color[i].Alpha & 0x80) == 0)
                        {
                            index = 3;
                        }
                        else
                        {
                            int minDiff = int.MaxValue;
                            for (int j = 0; j < 3; j++)
                            {
                                int delta_red = color_buffer[j].Red - color[i].Red;
                                if (delta_red < 0)
                                {
                                    delta_red = -delta_red;
                                }
                                int delta_green = color_buffer[j].Green - color[i].Green;
                                if (delta_green < 0)
                                {
                                    delta_green = -delta_green;
                                }
                                int delta_blue = color_buffer[j].Blue - color[i].Blue;
                                if (delta_blue < 0)
                                {
                                    delta_blue = -delta_blue;
                                }
                                int diff = delta_red + delta_green + delta_blue;
                                if (diff < minDiff)
                                {
                                    minDiff = diff;
                                    index = j;
                                }
                            }
                        }
                        color_flags <<= 2;
                        color_flags |= (uint)index;
                    }
                }
                else
                {
                    c0 = (ushort)((maxColor.Red >> 3 << 11) | (maxColor.Green >> 2 << 5) | (maxColor.Blue >> 3));
                    c1 = (ushort)((minColor.Red >> 3 << 11) | (minColor.Green >> 2 << 5) | (minColor.Blue >> 3));
                    Span<YFColor> color_buffer = stackalloc YFColor[4];
                    color_buffer[0].Red = maxColor.Red;
                    color_buffer[0].Green = maxColor.Green;
                    color_buffer[0].Blue = maxColor.Blue;
                    color_buffer[0].Alpha = 0xFF;
                    color_buffer[1].Red = minColor.Red;
                    color_buffer[1].Green = minColor.Green;
                    color_buffer[1].Blue = minColor.Blue;
                    color_buffer[1].Alpha = 0xFF;
                    color_buffer[2].Red = (byte)(((maxColor.Red << 1) + minColor.Red) / 3);
                    color_buffer[2].Green = (byte)(((maxColor.Green << 1) + minColor.Green) / 3);
                    color_buffer[2].Blue = (byte)(((maxColor.Blue << 1) + minColor.Blue) / 3);
                    color_buffer[2].Alpha = 0xFF;
                    color_buffer[3].Red = (byte)((maxColor.Red + (minColor.Red << 1)) / 3);
                    color_buffer[3].Green = (byte)((maxColor.Green + (minColor.Green << 1)) / 3);
                    color_buffer[3].Blue = (byte)((maxColor.Blue + (minColor.Blue << 1)) / 3);
                    color_buffer[3].Alpha = 0xFF;
                    for (int i = 15; i >= 0; i--)
                    {
                        int index = 0;
                        int minDiff = int.MaxValue;
                        for (int j = 0; j < 4; j++)
                        {
                            int delta_red = color_buffer[j].Red - color[i].Red;
                            if (delta_red < 0)
                            {
                                delta_red = -delta_red;
                            }
                            int delta_green = color_buffer[j].Green - color[i].Green;
                            if (delta_green < 0)
                            {
                                delta_green = -delta_green;
                            }
                            int delta_blue = color_buffer[j].Blue - color[i].Blue;
                            if (delta_blue < 0)
                            {
                                delta_blue = -delta_blue;
                            }
                            int diff = delta_red + delta_green + delta_blue;
                            if (diff < minDiff)
                            {
                                minDiff = diff;
                                index = j;
                            }
                        }
                        color_flags <<= 2;
                        color_flags |= (uint)index;
                    }
                }
                BinaryPrimitives.WriteUInt16LittleEndian(texPtr[..2], c0);
                BinaryPrimitives.WriteUInt16LittleEndian(texPtr.Slice(2, 2), c1);
                BinaryPrimitives.WriteUInt32LittleEndian(texPtr.Slice(4, 4), color_flags);
            }
        }

        private static class BCDecoder
        {
            public static void ModifyExplicitAlphaWord(ReadOnlySpan<byte> texPtr, Span<YFColor> color)
            {
                ulong buf = BinaryPrimitives.ReadUInt64BigEndian(texPtr);
                for (int i = 0; i < 16; i++)
                {
                    color[i].Alpha = (byte)BitHelper.FourBitToEightBit(buf);
                    buf >>= 4;
                }
            }

            public static void ModifyInterpolatedAlphaWord(ReadOnlySpan<byte> texPtr, Span<YFColor> color)
            {
                Span<byte> alpha_buffer = stackalloc byte[8];
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
                    color[i].Alpha = alpha_buffer[(int)(alpha_flags & 7ul)];
                    alpha_flags >>= 3;
                }
            }

            public static void DecodeColorWord(ReadOnlySpan<byte> texPtr, Span<YFColor> color, bool alpha)
            {
                Span<YFColor> color_buffer = stackalloc YFColor[4];
                ushort c0 = BinaryPrimitives.ReadUInt16LittleEndian(texPtr[..2]);
                ushort c1 = BinaryPrimitives.ReadUInt16LittleEndian(texPtr.Slice(2, 2));
                uint color_flags = BinaryPrimitives.ReadUInt32LittleEndian(texPtr.Slice(4, 4));
                // color0 = c0
                color_buffer[0].Red = (byte)BitHelper.FiveBitToEightBit(c0 >> 11);
                color_buffer[0].Green = (byte)BitHelper.SixBitToEightBit(c0 >> 5);
                color_buffer[0].Blue = (byte)BitHelper.FiveBitToEightBit(c0);
                color_buffer[0].Alpha = 0xFF;
                // color1 = c1
                color_buffer[1].Red = (byte)BitHelper.FiveBitToEightBit(c1 >> 11);
                color_buffer[1].Green = (byte)BitHelper.SixBitToEightBit(c1 >> 5);
                color_buffer[1].Blue = (byte)BitHelper.FiveBitToEightBit(c1);
                color_buffer[1].Alpha = 0xFF;
                if (c0 > c1)
                {
                    // color2 = 2 / 3 * c0 + 1 / 3 * c1
                    color_buffer[2].Red = (byte)(((color_buffer[0].Red << 1) + color_buffer[1].Red + 1) / 3);
                    color_buffer[2].Green = (byte)(((color_buffer[0].Green << 1) + color_buffer[1].Green + 1) / 3);
                    color_buffer[2].Blue = (byte)(((color_buffer[0].Blue << 1) + color_buffer[1].Blue + 1) / 3);
                    color_buffer[2].Alpha = 0xFF;
                    // color3 = 1 / 3 * c0 + 2 / 3 * c1
                    color_buffer[3].Red = (byte)((color_buffer[0].Red + (color_buffer[1].Red << 1) + 1) / 3);
                    color_buffer[3].Green = (byte)((color_buffer[0].Green + (color_buffer[1].Green << 1) + 1) / 3);
                    color_buffer[3].Blue = (byte)((color_buffer[0].Blue + (color_buffer[1].Blue << 1) + 1) / 3);
                    color_buffer[3].Alpha = 0xFF;
                }
                else
                {
                    // color2 = 1 / 2 * c0 + 1 / 2 * c1
                    color_buffer[2].Red = (byte)((color_buffer[0].Red + color_buffer[1].Red + 1) >> 1);
                    color_buffer[2].Green = (byte)((color_buffer[0].Green + color_buffer[1].Green + 1) >> 1);
                    color_buffer[2].Blue = (byte)((color_buffer[0].Blue + color_buffer[1].Blue + 1) >> 1);
                    color_buffer[2].Alpha = 0xFF;
                    // color3 = 0
                    color_buffer[3].Red = 0x0;
                    color_buffer[3].Green = 0x0;
                    color_buffer[3].Blue = 0x0;
                    color_buffer[3].Alpha = alpha ? (byte)0x0 : (byte)0xFF;
                }
                for (int i = 0; i < 16; i++)
                {
                    color[i] = color_buffer[(int)(color_flags & 0x3u)];
                    color_flags >>= 2;
                }
            }
        }
    }
}
