using System;
using System.Runtime.CompilerServices;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static unsafe class EACCoder
    {
        private static bool HasInit = false;

        private static void Init()
        {
            if (HasInit) return;
            HasInit = true;
            int count = 0;
            for (int m_base = 0; m_base < 256; m_base++)
            {
                for (int tab = 0; tab < 16; tab++)
                {
                    for (int mul = 0; mul < 16; mul++)
                    {
                        for (int index = 0; index < 8; index++)
                        {
                            valtab[count] = get16bits11bits(m_base, tab, mul, index);
                            count++;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int getPremulIndex(int m_base, int tab, int mul, int index)
        {
            return (m_base << 11) + (tab << 7) + (mul << 3) + index;
        }

        private static double calcError(YFColor* data, int m_base, int tab, int mul, double prevbest, int colorKind)
        {
            int offset = getPremulIndex(m_base, tab, mul, 0);
            double error = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    double besthere = (1 << 20);
                    besthere *= besthere;
                    byte m_byte = data[(y << 2) | x][colorKind];
                    int alpha = (m_byte << 8) | m_byte;
                    for (int index = 0; index < 8; index++)
                    {
                        double indexError;
                        indexError = alpha - valtab[offset + index];
                        indexError *= indexError;
                        if (indexError < besthere)
                            besthere = indexError;
                    }
                    error += besthere;
                    if (error >= prevbest)
                        return prevbest + (1 << 30);
                }
            }
            return error;
        }

        /// <summary>
        /// 非常非常慢，做好心理准备再用
        /// </summary>
        /// <param name="data"></param>
        /// <param name="img"></param>
        /// <param name="colorKind1"></param>
        /// <param name="colorKind2"></param>
        public static void EncodeR11G11EACBlock(Span<byte> eacword, ReadOnlySpan<YFColor> encode_data)
        {
            fixed (byte* data = eacword)
            {
                fixed (YFColor* img = encode_data)
                {
                    EncodeSingleWord(data, img, YFColor.RED);
                    EncodeSingleWord(data + 8, img, YFColor.GREEN);
                }
            }
        }

        public static void EncodeR11EACBlock(Span<byte> eacword, ReadOnlySpan<YFColor> encode_data)
        {
            fixed (byte* data = eacword)
            {
                fixed (YFColor* img = encode_data)
                {
                    EncodeSingleWord(data, img, YFColor.RED);
                }
            }
        }

        /// <summary>
        /// 非常非常慢，做好心理准备再用
        /// </summary>
        /// <param name="data"></param>
        /// <param name="img"></param>
        /// <param name="colorKind"></param>
        private static void EncodeSingleWord(byte* data, YFColor* img, int colorKind)
        {
            Init();
            uint bestbase = 0, besttable = 0, bestmul = 0;
            double besterror;
            besterror = 1 << 20;
            besterror *= besterror;
            for (int m_base = 0; m_base < 256; m_base++)
            {
                for (int table = 0; table < 16; table++)
                {
                    for (int mul = 0; mul < 16; mul++)
                    {
                        double e = calcError(img, m_base, table, mul, besterror, colorKind);
                        if (e < besterror)
                        {
                            bestbase = (uint)m_base;
                            besttable = (uint)table;
                            bestmul = (uint)mul;
                            besterror = e;
                        }
                    }
                }
            }
            data[0] = (byte)bestbase;
            data[1] = (byte)((bestmul << 4) + besttable);

            for (int i = 2; i < 8; i++)
            {
                data[i] = 0;
            }

            int m_byte2 = 2;
            int bit = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    besterror = 255 * 255;
                    besterror *= besterror;
                    int bestindex = 99;
                    byte m_byte = img[(y << 2) | x][colorKind];
                    int alpha = (m_byte << 8) | m_byte;
                    for (uint index = 0; index < 8; index++)
                    {
                        double indexError = alpha - get16bits11bits((int)bestbase, (int)besttable, (int)bestmul, (int)index);

                        indexError *= indexError;
                        if (indexError < besterror)
                        {
                            besterror = indexError;
                            bestindex = (int)index;
                        }
                    }

                    for (int numbit = 0; numbit < 3; numbit++)
                    {
                        data[m_byte2] |= getbit((byte)bestindex, 2 - numbit, 7 - bit);
                        bit++;
                        if (bit > 7)
                        {
                            bit = 0;
                            m_byte2++;
                        }
                    }
                }
            }
        }

        public static void DecodeR11G11EACBlock(ReadOnlySpan<byte> eacword, Span<YFColor> decode_data)
        {
            fixed (byte* data = eacword)
            {
                fixed (YFColor* img = decode_data)
                {
                    DecodeSingleWord(data, (uint*)img, YFColor.RED);
                    DecodeSingleWord(data + 8, (uint*)img, YFColor.GREEN);
                }
            }
        }

        public static void DecodeR11EACBlock(ReadOnlySpan<byte> eacword, Span<YFColor> decode_data)
        {
            fixed (byte* data = eacword)
            {
                fixed (YFColor* img = decode_data)
                {
                    DecodeSingleWord(data, (uint*)img, YFColor.RED);
                }
            }
        }

        private static void DecodeSingleWord(byte* data, uint* img, int colorKind)
        {
            byte* color = (byte*)img;
            int alpha = data[0];
            int table = data[1];
            int bit = 0;
            int m_byte = 2;
            //extract an alpha value for each pixel.
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    //Extract table index
                    int index = 0;
                    for (int bitpos = 0; bitpos < 3; bitpos++)
                    {
                        index |= getbit(data[m_byte], 7 - bit, 2 - bitpos);
                        bit++;
                        if (bit > 7)
                        {
                            bit = 0;
                            m_byte++;
                        }
                    }
                    //make data compatible with the .pgm format. See the comment in compressBlockAlpha16() for details.
                    ushort uSixteen = get16bits11bits(alpha, table & 15, table >> 4, index);
                    //byte swap for pgm
                    color[(((y << 2) | x) << 2) | colorKind] = (byte)(uSixteen >> 8);

                }
            }
        }

        private static ushort get16bits11bits(int m_base, int table, int mul, int index)
        {
            int elevenbase = m_base * 8 + 4;

            //i want the positive value here
            int tabVal = -alphaBase[table, 3 - index % 4] - 1;
            //and the sign, please
            int sign = 1 - (index / 4);

            if (sign != 0)
                tabVal = tabVal + 1;
            int elevenTabVal = tabVal * 8;

            if (mul != 0)
                elevenTabVal *= mul;
            else
                elevenTabVal /= 8;

            if (sign != 0)
                elevenTabVal = -elevenTabVal;

            //calculate sum
            int elevenbits = elevenbase + elevenTabVal;

            //clamp..
            if (elevenbits >= 256 * 8)
                elevenbits = 256 * 8 - 1;
            else if (elevenbits < 0)
                elevenbits = 0;
            //elevenbits now contains the 11 bit alpha value as defined in the spec.

            //extend to 16 bits before returning, since we don't have any good 11-bit file formats.
            ushort sixteenbits = (ushort)((elevenbits << 5) + (elevenbits >> 6));

            return sixteenbits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte getbit(byte input, int frompos, int topos)
        {
            if (frompos > topos)
            {
                return (byte)(((1 << frompos) & input) >> (frompos - topos));
            }
            return (byte)(((1 << frompos) & input) << (topos - frompos));
        }

        private static int[] valtab = new int[1024 * 512];

        private static int[,] alphaBase = new int[16, 4]
        {
            { -15, -9, -6, -3 },
            { -13, -10, -7, -3 },
            { -13, -8, -5, -2 },
            { -13, -6, -4, -2 },
            { -12, -8, -6, -3 },
            { -11, -9, -7, -3 },
            { -11, -8, -7, -4 },
            { -11, -8, -5, -3 },
            { -10, -8, -6, -2 },
            { -10, -8, -5, -2 },
            { -10, -8, -4, -2 },
            { -10, -7, -5, -2 },
            { -10, -7, -4, -3 },
            { -10, -3, -2, -1 },
            { -9, -8, -6, -4 },
            { -9, -7, -5, -3 }
        };
    }
}
