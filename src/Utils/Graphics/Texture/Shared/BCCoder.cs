using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static unsafe class BCCoder
    {
        public static void DecodeBC7Block(ReadOnlySpan<byte> bc7Word, Span<YFColor> decodeData)
        {
            fixed (byte* texData = bc7Word)
            {
                fixed (YFColor* colorData = decodeData)
                {
                    BC6To7Decoder.DecodeBC7Block(texData, colorData);
                }
            }
        }

        public static void EncodeBC7Block(ReadOnlySpan<byte> bc7Word, Span<YFColor> decodeData)
        {
            throw new NotImplementedException();
        }

        public static void DecodeBC6HUnsignedBlock(ReadOnlySpan<byte> bc6hWord, Span<YFColor> decodeData)
        {
            fixed (byte* texData = bc6hWord)
            {
                fixed (YFColor* colorData = decodeData)
                {
                    BC6To7Decoder.DecodeBC6Block(texData, colorData, false);
                }
            }
        }

        public static void EncodeBC6HUnsignedBlock(ReadOnlySpan<byte> bc6hWord, Span<YFColor> decodeData)
        {
            throw new NotImplementedException();
        }

        public static void DecodeBC6HSignedBlock(ReadOnlySpan<byte> bc6hWord, Span<YFColor> decodeData)
        {
            fixed (byte* texData = bc6hWord)
            {
                fixed (YFColor* colorData = decodeData)
                {
                    BC6To7Decoder.DecodeBC6Block(texData, colorData, true);
                }
            }
        }

        public static void EncodeBC6HSignedBlock(ReadOnlySpan<byte> bc6hWord, Span<YFColor> decodeData)
        {
            throw new NotImplementedException();
        }

        public static void DecodeBC3Block(ReadOnlySpan<byte> dxt5word, Span<YFColor> decode_data)
        {
            BC1To5Decoder.DecodeColorWord(dxt5word.Slice(8, 8), decode_data, false);
            BC1To5Decoder.ModifyInterpolatedAlphaWord(dxt5word[..8], decode_data);
        }

        public static void EncodeBC3Block(Span<byte> dxt5word, ReadOnlySpan<YFColor> encode_data)
        {
            BC1To5Encoder.EncodeColorWord(dxt5word.Slice(8, 8), encode_data, false);
            BC1To5Encoder.EncodeInterpolatedAlphaWord(dxt5word[..8], encode_data);
        }

        public static void DecodeBC2Block(ReadOnlySpan<byte> dxt3word, Span<YFColor> decode_data)
        {
            BC1To5Decoder.DecodeColorWord(dxt3word.Slice(8, 8), decode_data, false);
            BC1To5Decoder.ModifyExplicitAlphaWord(dxt3word[..8], decode_data);
        }

        public static void EncodeBC2Block(Span<byte> dxt3word, ReadOnlySpan<YFColor> encode_data)
        {
            BC1To5Encoder.EncodeColorWord(dxt3word.Slice(8, 8), encode_data, false);
            BC1To5Encoder.EncodeExplicitAlphaWord(dxt3word[..8], encode_data);
        }

        public static void DecodeBC1Block(ReadOnlySpan<byte> dxt1word, Span<YFColor> decode_data)
        {
            BC1To5Decoder.DecodeColorWord(dxt1word, decode_data, false);
        }

        public static void EncodeBC1Block(Span<byte> dxt1word, ReadOnlySpan<YFColor> encode_data)
        {
            BC1To5Encoder.EncodeColorWord(dxt1word, encode_data, false);
        }

        public static void DecodeBC1BlockWithAlpha(ReadOnlySpan<byte> dxt1word, Span<YFColor> decode_data)
        {
            BC1To5Decoder.DecodeColorWord(dxt1word, decode_data, true);
        }

        public static void EncodeBC1BlockWithAlpha(Span<byte> dxt1word, ReadOnlySpan<YFColor> encode_data)
        {
            BC1To5Encoder.EncodeColorWord(dxt1word, encode_data, true);
        }

        /// <summary>
        /// 用于快速编码DXT的类
        /// </summary>
        private static class BC1To5Encoder
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

        private static class BC1To5Decoder
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

        private static class BC6To7Decoder
        {
            private struct BitReader
            {
                byte* m_data;
                ushort m_bitPos;

                public BitReader(byte* data)
                {
                    m_data = data;
                }

                static void memcpy(void* d1, void* d2, int num)
                {
                    NativeMemory.Copy(d2, d1, (nuint)num);
                }

                public ushort read(byte _numBits)
                {
                    ushort pos = (ushort)(m_bitPos / 8);
                    ushort shift = (ushort)(m_bitPos & 7);
                    uint data = 0;
                    memcpy(&data, &m_data[pos], Math.Min(4, 16 - pos));
                    m_bitPos += _numBits;
                    return (ushort)((data >> shift) & ((1 << _numBits) - 1));
                }

                public ushort peek(ushort _offset, byte _numBits)
                {
                    ushort bitPos = (ushort)(m_bitPos + _offset);
                    ushort shift = (ushort)(bitPos & 7);
                    ushort pos = (ushort)(bitPos / 8);
                    uint data = 0;
                    memcpy(&data, &m_data[pos], Math.Min(4, 16 - pos));
                    return (byte)((data >> shift) & ((1 << _numBits) - 1));
                }
            }

            public static void DecodeBC6Block(byte* texPtr, YFColor* color, bool _signed)
            {

                BitReader bit = new BitReader(texPtr);

                byte mode = (byte)(bit.read(2));

                ushort* epR = stackalloc ushort[4];
                ushort* epG = stackalloc ushort[4];
                ushort* epB = stackalloc ushort[4];

                if ((mode & 2) != 0)
                {
                    // 5-bit mode
                    mode |= (byte)(bit.read(3) << 2);

                    if (0 == s_bc6hModeInfo[mode].endpointBits)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            color->Red = 0;
                            color->Green = 0;
                            color->Blue = 0;
                            color->Alpha = 0;
                            color++;
                        }
                        return;
                    }

                    switch (mode)
                    {
                        case 2:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(5) << 0);
                            epR[0] |= (ushort)(bit.read(1) << 10);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(4) << 0);
                            epG[0] |= (ushort)(bit.read(1) << 10);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(4) << 0);
                            epB[0] |= (ushort)(bit.read(1) << 10);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 3:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(10) << 0);
                            epG[1] |= (ushort)(bit.read(10) << 0);
                            epB[1] |= (ushort)(bit.read(10) << 0);
                            break;

                        case 6:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(4) << 0);
                            epR[0] |= (ushort)(bit.read(1) << 10);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(5) << 0);
                            epG[0] |= (ushort)(bit.read(1) << 10);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(4) << 0);
                            epB[0] |= (ushort)(bit.read(1) << 10);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(4) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(4) << 0);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 7:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(9) << 0);
                            epR[0] |= (ushort)(bit.read(1) << 10);
                            epG[1] |= (ushort)(bit.read(9) << 0);
                            epG[0] |= (ushort)(bit.read(1) << 10);
                            epB[1] |= (ushort)(bit.read(9) << 0);
                            epB[0] |= (ushort)(bit.read(1) << 10);
                            break;

                        case 10:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(4) << 0);
                            epR[0] |= (ushort)(bit.read(1) << 10);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(4) << 0);
                            epG[0] |= (ushort)(bit.read(1) << 10);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(5) << 0);
                            epB[0] |= (ushort)(bit.read(1) << 10);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(4) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(4) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 11:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(8) << 0);
                            epR[0] |= (ushort)(bit.read(1) << 11);
                            epR[0] |= (ushort)(bit.read(1) << 10);
                            epG[1] |= (ushort)(bit.read(8) << 0);
                            epG[0] |= (ushort)(bit.read(1) << 11);
                            epG[0] |= (ushort)(bit.read(1) << 10);
                            epB[1] |= (ushort)(bit.read(8) << 0);
                            epB[0] |= (ushort)(bit.read(1) << 11);
                            epB[0] |= (ushort)(bit.read(1) << 10);
                            break;

                        case 14:
                            epR[0] |= (ushort)(bit.read(9) << 0);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[0] |= (ushort)(bit.read(9) << 0);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[0] |= (ushort)(bit.read(9) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[1] |= (ushort)(bit.read(5) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 15:
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(4) << 0);
                            epR[0] |= (ushort)(bit.read(1) << 15);
                            epR[0] |= (ushort)(bit.read(1) << 14);
                            epR[0] |= (ushort)(bit.read(1) << 13);
                            epR[0] |= (ushort)(bit.read(1) << 12);
                            epR[0] |= (ushort)(bit.read(1) << 11);
                            epR[0] |= (ushort)(bit.read(1) << 10);
                            epG[1] |= (ushort)(bit.read(4) << 0);
                            epG[0] |= (ushort)(bit.read(1) << 15);
                            epG[0] |= (ushort)(bit.read(1) << 14);
                            epG[0] |= (ushort)(bit.read(1) << 13);
                            epG[0] |= (ushort)(bit.read(1) << 12);
                            epG[0] |= (ushort)(bit.read(1) << 11);
                            epG[0] |= (ushort)(bit.read(1) << 10);
                            epB[1] |= (ushort)(bit.read(4) << 0);
                            epB[0] |= (ushort)(bit.read(1) << 15);
                            epB[0] |= (ushort)(bit.read(1) << 14);
                            epB[0] |= (ushort)(bit.read(1) << 13);
                            epB[0] |= (ushort)(bit.read(1) << 12);
                            epB[0] |= (ushort)(bit.read(1) << 11);
                            epB[0] |= (ushort)(bit.read(1) << 10);
                            break;

                        case 18:
                            epR[0] |= (ushort)(bit.read(8) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[0] |= (ushort)(bit.read(8) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[0] |= (ushort)(bit.read(8) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[1] |= (ushort)(bit.read(6) << 0);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(6) << 0);
                            epR[3] |= (ushort)(bit.read(6) << 0);
                            break;

                        case 22:
                            epR[0] |= (ushort)(bit.read(8) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[0] |= (ushort)(bit.read(8) << 0);
                            epG[2] |= (ushort)(bit.read(1) << 5);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[0] |= (ushort)(bit.read(8) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[1] |= (ushort)(bit.read(5) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(6) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 26:
                            epR[0] |= (ushort)(bit.read(8) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[0] |= (ushort)(bit.read(8) << 0);
                            epB[2] |= (ushort)(bit.read(1) << 5);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[0] |= (ushort)(bit.read(8) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[1] |= (ushort)(bit.read(5) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(6) << 0);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 30:
                            epR[0] |= (ushort)(bit.read(6) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[0] |= (ushort)(bit.read(6) << 0);
                            epG[2] |= (ushort)(bit.read(1) << 5);
                            epB[2] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[0] |= (ushort)(bit.read(6) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            epB[3] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[1] |= (ushort)(bit.read(6) << 0);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(6) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(6) << 0);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(6) << 0);
                            epR[3] |= (ushort)(bit.read(6) << 0);
                            break;

                        default:
                            break;
                    }
                }

                else
                {
                    switch (mode)
                    {
                        case 0:
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[0] |= (ushort)(bit.read(10) << 0);
                            epG[0] |= (ushort)(bit.read(10) << 0);
                            epB[0] |= (ushort)(bit.read(10) << 0);
                            epR[1] |= (ushort)(bit.read(5) << 0);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epR[3] |= (ushort)(bit.read(5) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            break;

                        case 1:
                            epG[2] |= (ushort)(bit.read(1) << 5);
                            epG[3] |= (ushort)(bit.read(1) << 4);
                            epG[3] |= (ushort)(bit.read(1) << 5);
                            epR[0] |= (ushort)(bit.read(7) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 1);
                            epB[2] |= (ushort)(bit.read(1) << 4);
                            epG[0] |= (ushort)(bit.read(7) << 0);
                            epB[2] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 2);
                            epG[2] |= (ushort)(bit.read(1) << 4);
                            epB[0] |= (ushort)(bit.read(7) << 0);
                            epB[3] |= (ushort)(bit.read(1) << 3);
                            epB[3] |= (ushort)(bit.read(1) << 5);
                            epB[3] |= (ushort)(bit.read(1) << 4);
                            epR[1] |= (ushort)(bit.read(6) << 0);
                            epG[2] |= (ushort)(bit.read(4) << 0);
                            epG[1] |= (ushort)(bit.read(6) << 0);
                            epG[3] |= (ushort)(bit.read(4) << 0);
                            epB[1] |= (ushort)(bit.read(6) << 0);
                            epB[2] |= (ushort)(bit.read(4) << 0);
                            epR[2] |= (ushort)(bit.read(6) << 0);
                            epR[3] |= (ushort)(bit.read(6) << 0);
                            break;

                        default:
                            break;
                    }
                }

                Bc6hModeInfo mi = s_bc6hModeInfo[mode];

                if (_signed)
                {
                    epR[0] = sign_extend(epR[0], mi.endpointBits);
                    epG[0] = sign_extend(epG[0], mi.endpointBits);
                    epB[0] = sign_extend(epB[0], mi.endpointBits);
                }

                byte numSubsets = mi.partitionBits == 0 ? (byte)1 : (byte)2;  // 存疑

                for (byte ii = 1, num = (byte)(numSubsets * 2); ii < num; ++ii)
                {
                    if (_signed
                        || (mi.transformed != 0))
                    {
                        epR[ii] = sign_extend(epR[ii], mi.deltaBits[0]);
                        epG[ii] = sign_extend(epG[ii], mi.deltaBits[1]);
                        epB[ii] = sign_extend(epB[ii], mi.deltaBits[2]);
                    }

                    if (mi.transformed != 0)
                    {
                        ushort mask = (ushort)((1 << mi.endpointBits) - 1);

                        epR[ii] = (ushort)((epR[ii] + epR[0]) & mask);
                        epG[ii] = (ushort)((epG[ii] + epG[0]) & mask);
                        epB[ii] = (ushort)((epB[ii] + epB[0]) & mask);

                        if (_signed)
                        {
                            epR[ii] = sign_extend(epR[ii], mi.endpointBits);
                            epG[ii] = sign_extend(epG[ii], mi.endpointBits);
                            epB[ii] = sign_extend(epB[ii], mi.endpointBits);
                        }
                    }
                }

                for (byte ii = 0, num = (byte)(numSubsets * 2); ii < num; ++ii)
                {
                    epR[ii] = unquantize(epR[ii], _signed, mi.endpointBits);
                    epG[ii] = unquantize(epG[ii], _signed, mi.endpointBits);
                    epB[ii] = unquantize(epB[ii], _signed, mi.endpointBits);
                }

                byte partitionSetIdx = (byte)((mi.partitionBits != 0) ? bit.read(5) : 0);
                byte indexBits = (mi.partitionBits != 0) ? (byte)3 : (byte)4;
                byte[] factors = s_bptcFactors[indexBits - 2];

                for (byte yy = 0; yy < 4; ++yy)
                {
                    for (byte xx = 0; xx < 4; ++xx)
                    {
                        int idx = yy * 4 + xx;

                        byte subsetIndex = 0;
                        byte indexAnchor = 0;

                        if (0 != mi.partitionBits)
                        {
                            subsetIndex = (byte)((s_bptcP2[partitionSetIdx] >> idx) & 1);
                            indexAnchor = (subsetIndex != 0) ? s_bptcA2[partitionSetIdx] : (byte)0;
                        }

                        byte anchor = idx == indexAnchor ? (byte)1 : (byte)0;
                        byte num = (byte)(indexBits - anchor);
                        byte index = (byte)bit.read(num);

                        byte fc = factors[index];
                        byte fca = (byte)(64 - fc);
                        byte fcb = fc;

                        subsetIndex *= 2;
                        ushort rr = finish_unquantize((ushort)((epR[subsetIndex] * fca + epR[subsetIndex + 1] * fcb + 32) >> 6), _signed);
                        ushort gg = finish_unquantize((ushort)((epG[subsetIndex] * fca + epG[subsetIndex + 1] * fcb + 32) >> 6), _signed);
                        ushort bb = finish_unquantize((ushort)((epB[subsetIndex] * fca + epB[subsetIndex + 1] * fcb + 32) >> 6), _signed);

                        color[idx].Red = half_to_u8(rr);
                        color[idx].Green = half_to_u8(gg);
                        color[idx].Blue = half_to_u8(bb);
                        color[idx].Alpha = 255;
                    }
                }
            }

            static byte half_to_u8(ushort v)
            {
                double value = (double)*(Half*)&v;
                value = value <= 0.0031308d ? (12.92d * value) : (1.055d * Math.Pow(value, 1.0d / 2.4d) - 0.055d);
                value = Math.Round(value * 255.0d);
                if (value > 254.5)
                {
                    return 255;
                }
                if (value < 0.5)
                {
                    return 0;
                }
                return (byte)value;
            }

            static ushort sign_extend(ushort _value, byte _numBits)
            {
                ushort mask = (ushort)(1 << (_numBits - 1));
                ushort result = (ushort)((_value ^ mask) - mask);
                return result;
            }

            static ushort unquantize(ushort _value, bool _signed, byte _endpointBits)
            {
                ushort maxValue = (ushort)(1 << (_endpointBits - 1));

                if (_signed)
                {
                    if (_endpointBits >= 16)
                    {
                        return _value;
                    }

                    bool sign = (_value & 0x8000) != 0;
                    _value &= 0x7fff;

                    ushort unq;

                    if (0 == _value)
                    {
                        unq = 0;
                    }
                    else if (_value >= maxValue - 1)
                    {
                        unq = 0x7fff;
                    }
                    else
                    {
                        unq = (ushort)(((_value << 15) + 0x4000) >> (_endpointBits - 1));
                    }

                    return (ushort)(sign ? -unq : unq); // 存疑
                }

                if (_endpointBits >= 15)
                {
                    return _value;
                }

                if (0 == _value)
                {
                    return 0;
                }

                if (_value == maxValue)
                {
                    return ushort.MaxValue;
                }

                return (ushort)(((_value << 15) + 0x4000) >> (_endpointBits - 1));
            }

            static ushort finish_unquantize(ushort _value, bool _signed)
            {
                if (_signed)
                {
                    ushort sign = (ushort)(_value & 0x8000);
                    _value &= 0x7fff;

                    return (ushort)(((_value * 31) >> 5) | sign);
                }

                return (ushort)((_value * 31) >> 6);
            }

            public static void DecodeBC7Block(byte* texPtr, YFColor* color)
            {
                // 计算块使用的模式
                BitReader bit = new BitReader(texPtr);
                int mode = 0;
                while (mode < 8 && bit.read(1) == 0)
                {
                    mode++;
                }
                // 如果模式未定义则返回RGBA值均为0的块
                if (mode == 8)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        color->Red = 0;
                        color->Green = 0;
                        color->Blue = 0;
                        color->Alpha = 0;
                        color++;
                    }
                    return;
                }
                Bc7ModeInfo mi = s_bp7ModeInfo[mode];
                byte modePBits = 0 != mi.endpointPBits ? mi.endpointPBits : mi.sharedPBits;
                byte partitionSetIdx = (byte)bit.read(mi.partitionBits);
                byte rotationMode = (byte)bit.read(mi.rotationBits);
                byte indexSelectionMode = (byte)bit.read(mi.indexSelectionBits);
                byte* epR = stackalloc byte[6];
                byte* epG = stackalloc byte[6];
                byte* epB = stackalloc byte[6];
                byte* epA = stackalloc byte[6];
                for (int ii = 0; ii < mi.numSubsets; ii++)
                {
                    epR[ii << 1] = (byte)(bit.read(mi.colorBits) << modePBits);
                    epR[(ii << 1) | 1] = (byte)(bit.read(mi.colorBits) << modePBits);
                }
                for (int ii = 0; ii < mi.numSubsets; ii++)
                {
                    epG[ii << 1] = (byte)(bit.read(mi.colorBits) << modePBits);
                    epG[(ii << 1) | 1] = (byte)(bit.read(mi.colorBits) << modePBits);
                }
                for (int ii = 0; ii < mi.numSubsets; ii++)
                {
                    epB[ii << 1] = (byte)(bit.read(mi.colorBits) << modePBits);
                    epB[(ii << 1) | 1] = (byte)(bit.read(mi.colorBits) << modePBits);
                }
                if (mi.alphaBits != 0)
                {
                    for (int ii = 0; ii < mi.numSubsets; ii++)
                    {
                        epA[ii << 1] = (byte)(bit.read(mi.alphaBits) << modePBits);
                        epA[(ii << 1) | 1] = (byte)(bit.read(mi.alphaBits) << modePBits);
                    }
                }
                else
                {
                    for (int ii = 0; ii < 6; ii++)
                    {
                        epA[ii] = 0xFF;
                    }
                }
                if (modePBits != 0)
                {
                    for (int ii = 0; ii < mi.numSubsets; ii++)
                    {
                        byte pda = (byte)bit.read(modePBits);
                        byte pdb = (byte)(0 == mi.sharedPBits ? bit.read(modePBits) : pda);
                        epR[ii * 2 + 0] |= pda;
                        epR[ii * 2 + 1] |= pdb;
                        epG[ii * 2 + 0] |= pda;
                        epG[ii * 2 + 1] |= pdb;
                        epB[ii * 2 + 0] |= pda;
                        epB[ii * 2 + 1] |= pdb;
                        epA[ii * 2 + 0] |= pda;
                        epA[ii * 2 + 1] |= pdb;
                    }
                }
                byte colorBits = (byte)(mi.colorBits + modePBits);
                for (int ii = 0; ii < mi.numSubsets; ++ii)
                {
                    epR[ii * 2 + 0] = expand_quantized(epR[ii * 2 + 0], colorBits);
                    epR[ii * 2 + 1] = expand_quantized(epR[ii * 2 + 1], colorBits);
                    epG[ii * 2 + 0] = expand_quantized(epG[ii * 2 + 0], colorBits);
                    epG[ii * 2 + 1] = expand_quantized(epG[ii * 2 + 1], colorBits);
                    epB[ii * 2 + 0] = expand_quantized(epB[ii * 2 + 0], colorBits);
                    epB[ii * 2 + 1] = expand_quantized(epB[ii * 2 + 1], colorBits);
                }
                if (mi.alphaBits != 0)
                {
                    byte alphaBits = (byte)(mi.alphaBits + modePBits);

                    for (int ii = 0; ii < mi.numSubsets; ++ii)
                    {
                        epA[ii * 2 + 0] = expand_quantized(epA[ii * 2 + 0], alphaBits);
                        epA[ii * 2 + 1] = expand_quantized(epA[ii * 2 + 1], alphaBits);
                    }
                }
                bool hasIndexBits1 = mi.indexBits[1] != 0;
                byte[][] factors = new byte[2][]
                {
                    s_bptcFactors[mi.indexBits[0] - 2],
                    s_bptcFactors[mi.indexBits[hasIndexBits1 ? 1 : 0] - 2]
                };
                ushort* offset = stackalloc ushort[2]
                {
                    0,
                    (ushort)(mi.numSubsets * (16 * mi.indexBits[0] - 1)),
                };
                byte* num = stackalloc byte[2];
                byte* index = stackalloc byte[2];
                for (int yy = 0; yy < 4; ++yy)
                {
                    for (int xx = 0; xx < 4; ++xx)
                    {
                        int idx = yy * 4 + xx;

                        byte subsetIndex = 0;
                        byte indexAnchor = 0;
                        switch (mi.numSubsets)
                        {
                            case 2:
                                subsetIndex = (byte)((s_bptcP2[partitionSetIdx] >> idx) & 1);
                                indexAnchor = subsetIndex != 0 ? s_bptcA2[partitionSetIdx] : (byte)0;
                                break;

                            case 3:
                                subsetIndex = (byte)((s_bptcP3[partitionSetIdx] >> (2 * idx)) & 3);
                                indexAnchor = subsetIndex != 0 ? s_bptcA3[subsetIndex - 1, partitionSetIdx] : (byte)0;
                                break;
                            default:
                                break;
                        }

                        byte anchor = idx == indexAnchor ? (byte)1 : (byte)0;
                        num[0] = (byte)(mi.indexBits[0] - anchor);
                        num[1] = (byte)(hasIndexBits1 ? mi.indexBits[1] - anchor : 0);
                        index[0] = (byte)bit.peek(offset[0], num[0]);
                        index[1] = hasIndexBits1 ? (byte)bit.peek(offset[1], num[1]) : index[0];
                        offset[0] += num[0];
                        offset[1] += num[1];

                        byte fc = factors[indexSelectionMode][index[indexSelectionMode]];
                        byte fa = factors[indexSelectionMode == 0 ? 1 : 0][index[indexSelectionMode == 0 ? 1 : 0]];

                        byte fca = (byte)(64 - fc);
                        byte fcb = fc;
                        byte faa = (byte)(64 - fa);
                        byte fab = fa;

                        subsetIndex *= 2;
                        byte rr = (byte)((ushort)(epR[subsetIndex] * fca + epR[subsetIndex + 1] * fcb + 32) >> 6);
                        byte gg = (byte)((ushort)(epG[subsetIndex] * fca + epG[subsetIndex + 1] * fcb + 32) >> 6);
                        byte bb = (byte)((ushort)(epB[subsetIndex] * fca + epB[subsetIndex + 1] * fcb + 32) >> 6);
                        byte aa = (byte)((ushort)(epA[subsetIndex] * faa + epA[subsetIndex + 1] * fab + 32) >> 6);

                        switch (rotationMode)
                        {
                            case 1: (aa, rr) = (rr, aa); break;
                            case 2: (aa, gg) = (gg, aa); break;
                            case 3: (aa, bb) = (bb, aa); break;
                            default: break;
                        };

                        color[idx].Red = rr;
                        color[idx].Green = gg;
                        color[idx].Blue = bb;
                        color[idx].Alpha = aa;
                    }
                }
            }

            static byte expand_quantized(byte v, int bits)
            {
                v <<= 8 - bits;
                return (byte)(v | (v >> bits));
            }

            private struct Bc6hModeInfo
            {
                public byte transformed;
                public byte partitionBits;
                public byte endpointBits;
                public fixed byte deltaBits[3];

                public Bc6hModeInfo(byte transformed, byte partitionBits, byte endpointBits, byte deltaBits0, byte deltaBits1, byte deltaBits2)
                {
                    this.transformed = transformed;
                    this.partitionBits = partitionBits;
                    this.endpointBits = endpointBits;
                    deltaBits[0] = deltaBits0;
                    deltaBits[1] = deltaBits1;
                    deltaBits[2] = deltaBits2;
                }
            }
            private struct Bc7ModeInfo
            {
                public byte numSubsets;
                public byte partitionBits;
                public byte rotationBits;
                public byte indexSelectionBits;
                public byte colorBits;
                public byte alphaBits;
                public byte endpointPBits;
                public byte sharedPBits;
                public fixed byte indexBits[2];

                public Bc7ModeInfo(byte numSubsets, byte partitionBits, byte rotationBits, byte indexSelectionBits, byte colorBits, byte alphaBits, byte endpointPBits, byte sharedPBits, byte indexBits0, byte indexBits1)
                {
                    this.numSubsets = numSubsets;
                    this.partitionBits = partitionBits;
                    this.rotationBits = rotationBits;
                    this.indexSelectionBits = indexSelectionBits;
                    this.colorBits = colorBits;
                    this.alphaBits = alphaBits;
                    this.endpointPBits = endpointPBits;
                    this.sharedPBits = sharedPBits;
                    indexBits[0] = indexBits0;
                    indexBits[1] = indexBits1;
                }
            }

            static uint[] s_bptcP3 = new uint[]
            { //  76543210     0000   1111   2222   3333   4444   5555   6666   7777
                0xaa685050, // 0, 0,  1, 1,  0, 0,  1, 1,  0, 2,  2, 1,  2, 2,  2, 2
                0x6a5a5040, // 0, 0,  0, 1,  0, 0,  1, 1,  2, 2,  1, 1,  2, 2,  2, 1
                0x5a5a4200, // 0, 0,  0, 0,  2, 0,  0, 1,  2, 2,  1, 1,  2, 2,  1, 1
                0x5450a0a8, // 0, 2,  2, 2,  0, 0,  2, 2,  0, 0,  1, 1,  0, 1,  1, 1
                0xa5a50000, // 0, 0,  0, 0,  0, 0,  0, 0,  1, 1,  2, 2,  1, 1,  2, 2
                0xa0a05050, // 0, 0,  1, 1,  0, 0,  1, 1,  0, 0,  2, 2,  0, 0,  2, 2
                0x5555a0a0, // 0, 0,  2, 2,  0, 0,  2, 2,  1, 1,  1, 1,  1, 1,  1, 1
                0x5a5a5050, // 0, 0,  1, 1,  0, 0,  1, 1,  2, 2,  1, 1,  2, 2,  1, 1
                0xaa550000, // 0, 0,  0, 0,  0, 0,  0, 0,  1, 1,  1, 1,  2, 2,  2, 2
                0xaa555500, // 0, 0,  0, 0,  1, 1,  1, 1,  1, 1,  1, 1,  2, 2,  2, 2
                0xaaaa5500, // 0, 0,  0, 0,  1, 1,  1, 1,  2, 2,  2, 2,  2, 2,  2, 2
                0x90909090, // 0, 0,  1, 2,  0, 0,  1, 2,  0, 0,  1, 2,  0, 0,  1, 2
                0x94949494, // 0, 1,  1, 2,  0, 1,  1, 2,  0, 1,  1, 2,  0, 1,  1, 2
                0xa4a4a4a4, // 0, 1,  2, 2,  0, 1,  2, 2,  0, 1,  2, 2,  0, 1,  2, 2
                0xa9a59450, // 0, 0,  1, 1,  0, 1,  1, 2,  1, 1,  2, 2,  1, 2,  2, 2
                0x2a0a4250, // 0, 0,  1, 1,  2, 0,  0, 1,  2, 2,  0, 0,  2, 2,  2, 0
                0xa5945040, // 0, 0,  0, 1,  0, 0,  1, 1,  0, 1,  1, 2,  1, 1,  2, 2
                0x0a425054, // 0, 1,  1, 1,  0, 0,  1, 1,  2, 0,  0, 1,  2, 2,  0, 0
                0xa5a5a500, // 0, 0,  0, 0,  1, 1,  2, 2,  1, 1,  2, 2,  1, 1,  2, 2
                0x55a0a0a0, // 0, 0,  2, 2,  0, 0,  2, 2,  0, 0,  2, 2,  1, 1,  1, 1
                0xa8a85454, // 0, 1,  1, 1,  0, 1,  1, 1,  0, 2,  2, 2,  0, 2,  2, 2
                0x6a6a4040, // 0, 0,  0, 1,  0, 0,  0, 1,  2, 2,  2, 1,  2, 2,  2, 1
                0xa4a45000, // 0, 0,  0, 0,  0, 0,  1, 1,  0, 1,  2, 2,  0, 1,  2, 2
                0x1a1a0500, // 0, 0,  0, 0,  1, 1,  0, 0,  2, 2,  1, 0,  2, 2,  1, 0
                0x0050a4a4, // 0, 1,  2, 2,  0, 1,  2, 2,  0, 0,  1, 1,  0, 0,  0, 0
                0xaaa59090, // 0, 0,  1, 2,  0, 0,  1, 2,  1, 1,  2, 2,  2, 2,  2, 2
                0x14696914, // 0, 1,  1, 0,  1, 2,  2, 1,  1, 2,  2, 1,  0, 1,  1, 0
                0x69691400, // 0, 0,  0, 0,  0, 1,  1, 0,  1, 2,  2, 1,  1, 2,  2, 1
                0xa08585a0, // 0, 0,  2, 2,  1, 1,  0, 2,  1, 1,  0, 2,  0, 0,  2, 2
                0xaa821414, // 0, 1,  1, 0,  0, 1,  1, 0,  2, 0,  0, 2,  2, 2,  2, 2
                0x50a4a450, // 0, 0,  1, 1,  0, 1,  2, 2,  0, 1,  2, 2,  0, 0,  1, 1
                0x6a5a0200, // 0, 0,  0, 0,  2, 0,  0, 0,  2, 2,  1, 1,  2, 2,  2, 1
                0xa9a58000, // 0, 0,  0, 0,  0, 0,  0, 2,  1, 1,  2, 2,  1, 2,  2, 2
                0x5090a0a8, // 0, 2,  2, 2,  0, 0,  2, 2,  0, 0,  1, 2,  0, 0,  1, 1
                0xa8a09050, // 0, 0,  1, 1,  0, 0,  1, 2,  0, 0,  2, 2,  0, 2,  2, 2
                0x24242424, // 0, 1,  2, 0,  0, 1,  2, 0,  0, 1,  2, 0,  0, 1,  2, 0
                0x00aa5500, // 0, 0,  0, 0,  1, 1,  1, 1,  2, 2,  2, 2,  0, 0,  0, 0
                0x24924924, // 0, 1,  2, 0,  1, 2,  0, 1,  2, 0,  1, 2,  0, 1,  2, 0
                0x24499224, // 0, 1,  2, 0,  2, 0,  1, 2,  1, 2,  0, 1,  0, 1,  2, 0
                0x50a50a50, // 0, 0,  1, 1,  2, 2,  0, 0,  1, 1,  2, 2,  0, 0,  1, 1
                0x500aa550, // 0, 0,  1, 1,  1, 1,  2, 2,  2, 2,  0, 0,  0, 0,  1, 1
                0xaaaa4444, // 0, 1,  0, 1,  0, 1,  0, 1,  2, 2,  2, 2,  2, 2,  2, 2
                0x66660000, // 0, 0,  0, 0,  0, 0,  0, 0,  2, 1,  2, 1,  2, 1,  2, 1
                0xa5a0a5a0, // 0, 0,  2, 2,  1, 1,  2, 2,  0, 0,  2, 2,  1, 1,  2, 2
                0x50a050a0, // 0, 0,  2, 2,  0, 0,  1, 1,  0, 0,  2, 2,  0, 0,  1, 1
                0x69286928, // 0, 2,  2, 0,  1, 2,  2, 1,  0, 2,  2, 0,  1, 2,  2, 1
                0x44aaaa44, // 0, 1,  0, 1,  2, 2,  2, 2,  2, 2,  2, 2,  0, 1,  0, 1
                0x66666600, // 0, 0,  0, 0,  2, 1,  2, 1,  2, 1,  2, 1,  2, 1,  2, 1
                0xaa444444, // 0, 1,  0, 1,  0, 1,  0, 1,  0, 1,  0, 1,  2, 2,  2, 2
                0x54a854a8, // 0, 2,  2, 2,  0, 1,  1, 1,  0, 2,  2, 2,  0, 1,  1, 1
                0x95809580, // 0, 0,  0, 2,  1, 1,  1, 2,  0, 0,  0, 2,  1, 1,  1, 2
                0x96969600, // 0, 0,  0, 0,  2, 1,  1, 2,  2, 1,  1, 2,  2, 1,  1, 2
                0xa85454a8, // 0, 2,  2, 2,  0, 1,  1, 1,  0, 1,  1, 1,  0, 2,  2, 2
                0x80959580, // 0, 0,  0, 2,  1, 1,  1, 2,  1, 1,  1, 2,  0, 0,  0, 2
                0xaa141414, // 0, 1,  1, 0,  0, 1,  1, 0,  0, 1,  1, 0,  2, 2,  2, 2
                0x96960000, // 0, 0,  0, 0,  0, 0,  0, 0,  2, 1,  1, 2,  2, 1,  1, 2
                0xaaaa1414, // 0, 1,  1, 0,  0, 1,  1, 0,  2, 2,  2, 2,  2, 2,  2, 2
                0xa05050a0, // 0, 0,  2, 2,  0, 0,  1, 1,  0, 0,  1, 1,  0, 0,  2, 2
                0xa0a5a5a0, // 0, 0,  2, 2,  1, 1,  2, 2,  1, 1,  2, 2,  0, 0,  2, 2
                0x96000000, // 0, 0,  0, 0,  0, 0,  0, 0,  0, 0,  0, 0,  2, 1,  1, 2
                0x40804080, // 0, 0,  0, 2,  0, 0,  0, 1,  0, 0,  0, 2,  0, 0,  0, 1
                0xa9a8a9a8, // 0, 2,  2, 2,  1, 2,  2, 2,  0, 2,  2, 2,  1, 2,  2, 2
                0xaaaaaa44, // 0, 1,  0, 1,  2, 2,  2, 2,  2, 2,  2, 2,  2, 2,  2, 2
                0x2a4a5254, // 0, 1,  1, 1,  2, 0,  1, 1,  2, 2,  0, 1,  2, 2,  2, 0
            };

            static byte[,] s_bptcA3 = new byte[2, 64]
            {
                {
                     3,  3, 15, 15,  8,  3, 15, 15,
                     8,  8,  6,  6,  6,  5,  3,  3,
                     3,  3,  8, 15,  3,  3,  6, 10,
                     5,  8,  8,  6,  8,  5, 15, 15,
                     8, 15,  3,  5,  6, 10,  8, 15,
                    15,  3, 15,  5, 15, 15, 15, 15,
                     3, 15,  5,  5,  5,  8,  5, 10,
                     5, 10,  8, 13, 15, 12,  3,  3,
                },
                {
                    15,  8,  8,  3, 15, 15,  3,  8,
                    15, 15, 15, 15, 15, 15, 15,  8,
                    15,  8, 15,  3, 15,  8, 15,  8,
                     3, 15,  6, 10, 15, 15, 10,  8,
                    15,  3, 15, 10, 10,  8,  9, 10,
                     6, 15,  8, 15,  3,  6,  6,  8,
                    15,  3, 15, 15, 15, 15, 15, 15,
                    15, 15, 15, 15,  3, 15, 15,  8,
                },
            };

            static ushort[] s_bptcP2 = new ushort[]
            { //  3210     0000000000   1111111111   2222222222   3333333333
                0xcccc, // 0, 0, 1, 1,  0, 0, 1, 1,  0, 0, 1, 1,  0, 0, 1, 1
                0x8888, // 0, 0, 0, 1,  0, 0, 0, 1,  0, 0, 0, 1,  0, 0, 0, 1
                0xeeee, // 0, 1, 1, 1,  0, 1, 1, 1,  0, 1, 1, 1,  0, 1, 1, 1
                0xecc8, // 0, 0, 0, 1,  0, 0, 1, 1,  0, 0, 1, 1,  0, 1, 1, 1
                0xc880, // 0, 0, 0, 0,  0, 0, 0, 1,  0, 0, 0, 1,  0, 0, 1, 1
                0xfeec, // 0, 0, 1, 1,  0, 1, 1, 1,  0, 1, 1, 1,  1, 1, 1, 1
                0xfec8, // 0, 0, 0, 1,  0, 0, 1, 1,  0, 1, 1, 1,  1, 1, 1, 1
                0xec80, // 0, 0, 0, 0,  0, 0, 0, 1,  0, 0, 1, 1,  0, 1, 1, 1
                0xc800, // 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 1,  0, 0, 1, 1
                0xffec, // 0, 0, 1, 1,  0, 1, 1, 1,  1, 1, 1, 1,  1, 1, 1, 1
                0xfe80, // 0, 0, 0, 0,  0, 0, 0, 1,  0, 1, 1, 1,  1, 1, 1, 1
                0xe800, // 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 1,  0, 1, 1, 1
                0xffe8, // 0, 0, 0, 1,  0, 1, 1, 1,  1, 1, 1, 1,  1, 1, 1, 1
                0xff00, // 0, 0, 0, 0,  0, 0, 0, 0,  1, 1, 1, 1,  1, 1, 1, 1
                0xfff0, // 0, 0, 0, 0,  1, 1, 1, 1,  1, 1, 1, 1,  1, 1, 1, 1
                0xf000, // 0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  1, 1, 1, 1
                0xf710, // 0, 0, 0, 0,  1, 0, 0, 0,  1, 1, 1, 0,  1, 1, 1, 1
                0x008e, // 0, 1, 1, 1,  0, 0, 0, 1,  0, 0, 0, 0,  0, 0, 0, 0
                0x7100, // 0, 0, 0, 0,  0, 0, 0, 0,  1, 0, 0, 0,  1, 1, 1, 0
                0x08ce, // 0, 1, 1, 1,  0, 0, 1, 1,  0, 0, 0, 1,  0, 0, 0, 0
                0x008c, // 0, 0, 1, 1,  0, 0, 0, 1,  0, 0, 0, 0,  0, 0, 0, 0
                0x7310, // 0, 0, 0, 0,  1, 0, 0, 0,  1, 1, 0, 0,  1, 1, 1, 0
                0x3100, // 0, 0, 0, 0,  0, 0, 0, 0,  1, 0, 0, 0,  1, 1, 0, 0
                0x8cce, // 0, 1, 1, 1,  0, 0, 1, 1,  0, 0, 1, 1,  0, 0, 0, 1
                0x088c, // 0, 0, 1, 1,  0, 0, 0, 1,  0, 0, 0, 1,  0, 0, 0, 0
                0x3110, // 0, 0, 0, 0,  1, 0, 0, 0,  1, 0, 0, 0,  1, 1, 0, 0
                0x6666, // 0, 1, 1, 0,  0, 1, 1, 0,  0, 1, 1, 0,  0, 1, 1, 0
                0x366c, // 0, 0, 1, 1,  0, 1, 1, 0,  0, 1, 1, 0,  1, 1, 0, 0
                0x17e8, // 0, 0, 0, 1,  0, 1, 1, 1,  1, 1, 1, 0,  1, 0, 0, 0
                0x0ff0, // 0, 0, 0, 0,  1, 1, 1, 1,  1, 1, 1, 1,  0, 0, 0, 0
                0x718e, // 0, 1, 1, 1,  0, 0, 0, 1,  1, 0, 0, 0,  1, 1, 1, 0
                0x399c, // 0, 0, 1, 1,  1, 0, 0, 1,  1, 0, 0, 1,  1, 1, 0, 0
                0xaaaa, // 0, 1, 0, 1,  0, 1, 0, 1,  0, 1, 0, 1,  0, 1, 0, 1
                0xf0f0, // 0, 0, 0, 0,  1, 1, 1, 1,  0, 0, 0, 0,  1, 1, 1, 1
                0x5a5a, // 0, 1, 0, 1,  1, 0, 1, 0,  0, 1, 0, 1,  1, 0, 1, 0
                0x33cc, // 0, 0, 1, 1,  0, 0, 1, 1,  1, 1, 0, 0,  1, 1, 0, 0
                0x3c3c, // 0, 0, 1, 1,  1, 1, 0, 0,  0, 0, 1, 1,  1, 1, 0, 0
                0x55aa, // 0, 1, 0, 1,  0, 1, 0, 1,  1, 0, 1, 0,  1, 0, 1, 0
                0x9696, // 0, 1, 1, 0,  1, 0, 0, 1,  0, 1, 1, 0,  1, 0, 0, 1
                0xa55a, // 0, 1, 0, 1,  1, 0, 1, 0,  1, 0, 1, 0,  0, 1, 0, 1
                0x73ce, // 0, 1, 1, 1,  0, 0, 1, 1,  1, 1, 0, 0,  1, 1, 1, 0
                0x13c8, // 0, 0, 0, 1,  0, 0, 1, 1,  1, 1, 0, 0,  1, 0, 0, 0
                0x324c, // 0, 0, 1, 1,  0, 0, 1, 0,  0, 1, 0, 0,  1, 1, 0, 0
                0x3bdc, // 0, 0, 1, 1,  1, 0, 1, 1,  1, 1, 0, 1,  1, 1, 0, 0
                0x6996, // 0, 1, 1, 0,  1, 0, 0, 1,  1, 0, 0, 1,  0, 1, 1, 0
                0xc33c, // 0, 0, 1, 1,  1, 1, 0, 0,  1, 1, 0, 0,  0, 0, 1, 1
                0x9966, // 0, 1, 1, 0,  0, 1, 1, 0,  1, 0, 0, 1,  1, 0, 0, 1
                0x0660, // 0, 0, 0, 0,  0, 1, 1, 0,  0, 1, 1, 0,  0, 0, 0, 0
                0x0272, // 0, 1, 0, 0,  1, 1, 1, 0,  0, 1, 0, 0,  0, 0, 0, 0
                0x04e4, // 0, 0, 1, 0,  0, 1, 1, 1,  0, 0, 1, 0,  0, 0, 0, 0
                0x4e40, // 0, 0, 0, 0,  0, 0, 1, 0,  0, 1, 1, 1,  0, 0, 1, 0
                0x2720, // 0, 0, 0, 0,  0, 1, 0, 0,  1, 1, 1, 0,  0, 1, 0, 0
                0xc936, // 0, 1, 1, 0,  1, 1, 0, 0,  1, 0, 0, 1,  0, 0, 1, 1
                0x936c, // 0, 0, 1, 1,  0, 1, 1, 0,  1, 1, 0, 0,  1, 0, 0, 1
                0x39c6, // 0, 1, 1, 0,  0, 0, 1, 1,  1, 0, 0, 1,  1, 1, 0, 0
                0x639c, // 0, 0, 1, 1,  1, 0, 0, 1,  1, 1, 0, 0,  0, 1, 1, 0
                0x9336, // 0, 1, 1, 0,  1, 1, 0, 0,  1, 1, 0, 0,  1, 0, 0, 1
                0x9cc6, // 0, 1, 1, 0,  0, 0, 1, 1,  0, 0, 1, 1,  1, 0, 0, 1
                0x817e, // 0, 1, 1, 1,  1, 1, 1, 0,  1, 0, 0, 0,  0, 0, 0, 1
                0xe718, // 0, 0, 0, 1,  1, 0, 0, 0,  1, 1, 1, 0,  0, 1, 1, 1
                0xccf0, // 0, 0, 0, 0,  1, 1, 1, 1,  0, 0, 1, 1,  0, 0, 1, 1
                0x0fcc, // 0, 0, 1, 1,  0, 0, 1, 1,  1, 1, 1, 1,  0, 0, 0, 0
                0x7744, // 0, 0, 1, 0,  0, 0, 1, 0,  1, 1, 1, 0,  1, 1, 1, 0
                0xee22, // 0, 1, 0, 0,  0, 1, 0, 0,  0, 1, 1, 1,  0, 1, 1, 1
            };

            static byte[] s_bptcA2 = new byte[]
            {
                15, 15, 15, 15, 15, 15, 15, 15,
                15, 15, 15, 15, 15, 15, 15, 15,
                15,  2,  8,  2,  2,  8,  8, 15,
                 2,  8,  2,  2,  8,  8,  2,  2,
                15, 15,  6,  8,  2,  8, 15, 15,
                 2,  8,  2,  2,  2, 15, 15,  6,
                 6,  2,  6,  8, 15, 15,  2,  2,
                15, 15, 15, 15, 15,  2,  2, 15,
            };

            private static byte[][] s_bptcFactors = new byte[3][]
            {
                new byte[16] { 0, 21, 43, 64,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },
                new byte[16] { 0,  9, 18, 27, 37, 46, 55, 64,  0,  0,  0,  0,  0,  0,  0,  0 },
                new byte[16] { 0,  4,  9, 13, 17, 21, 26, 30, 34, 38, 43, 47, 51, 55, 60, 64 },
            };

            private static Bc6hModeInfo[] s_bc6hModeInfo = new Bc6hModeInfo[32]
            {   //                +--------------------------- transformed
                //                |  +------------------------ partition bits
                //                |  |  +--------------------- endpoint bits
                //                |  |  |      +-------------- delta bits
                new Bc6hModeInfo( 1, 5, 10,   5,  5,  5  ), // 00    2-bits
                new Bc6hModeInfo( 1, 5,  7,   6,  6,  6  ), // 01
                new Bc6hModeInfo( 1, 5, 11,   5,  4,  4  ), // 00010 5-bits
                new Bc6hModeInfo( 0, 0, 10,  10, 10, 10  ), // 00011
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 1, 5, 11,   4,  5,  4  ), // 00110
                new Bc6hModeInfo( 1, 0, 11,   9,  9,  9  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 1, 5, 11,   4,  4,  5  ), // 00010
                new Bc6hModeInfo( 1, 0, 12,   8,  8,  8  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 1, 5,  9,   5,  5,  5  ), // 00010
                new Bc6hModeInfo( 1, 0, 16,   4,  4,  4  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 1, 5,  8,   6,  5,  5  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 1, 5,  8,   5,  6,  5  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 1, 5,  8,   5,  5,  6  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
                new Bc6hModeInfo( 0, 5,  6,   6,  6,  6  ), // 00010
                new Bc6hModeInfo( 0, 0,  0,   0,  0,  0  ), // -
            };

            private static Bc7ModeInfo[] s_bp7ModeInfo = new Bc7ModeInfo[8]
            {
                //              +----------------------------- num subsets
                //              |  +-------------------------- partition bits
                //              |  |  +----------------------- rotation bits
                //              |  |  |  +-------------------- index selection bits
                //              |  |  |  |  +----------------- color bits
                //              |  |  |  |  |  +-------------- alpha bits
                //              |  |  |  |  |  |  +----------- endpoint P-bits
                //              |  |  |  |  |  |  |  +-------- shared P-bits
                //              |  |  |  |  |  |  |  |  +----- index bits 0
                //              |  |  |  |  |  |  |  |  |  +-- index bits 1
                new Bc7ModeInfo(3, 4, 0, 0, 4, 0, 1, 0, 3, 0), // 0
                new Bc7ModeInfo(2, 6, 0, 0, 6, 0, 0, 1, 3, 0), // 1
                new Bc7ModeInfo(3, 6, 0, 0, 5, 0, 0, 0, 2, 0), // 2
                new Bc7ModeInfo(2, 6, 0, 0, 7, 0, 1, 0, 2, 0), // 3
                new Bc7ModeInfo(1, 0, 2, 1, 5, 6, 0, 0, 2, 3), // 4
                new Bc7ModeInfo(1, 0, 2, 0, 7, 8, 0, 0, 2, 2), // 5
                new Bc7ModeInfo(1, 0, 0, 0, 7, 7, 1, 0, 4, 0), // 6
                new Bc7ModeInfo(2, 6, 0, 0, 5, 5, 1, 0, 2, 0), // 7
            };


        }
    }
}
