using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    /// <summary>
    /// 这个类提供的解码方法是移植爱立信的ETCPACK中的c/c++代码的
    /// C#没有那种预编译指令，所以我把一些东西变成了函数
    /// </summary>
    internal static unsafe class ETCCoder
    {
        public static void DecodeR8G8B8A8ETC2EACBlock(ReadOnlySpan<byte> etcword, Span<YFColor> decode_data)
        {
            fixed (byte* texPtr = etcword)
            {
                fixed (YFColor* dataPtr = decode_data)
                {
                    ETCDecoder.DecodeBlock_RGBA_ETC2_EAC(texPtr, dataPtr);
                }
            }
        }

        public static void DecodeR8G8B8A1ETC2Block(ReadOnlySpan<byte> etcword, Span<YFColor> decode_data)
        {
            fixed (byte* texPtr = etcword)
            {
                fixed (YFColor* dataPtr = decode_data)
                {
                    ETCDecoder.DecodeBlock_RGB_A1_ETC2(texPtr, dataPtr);
                }
            }
        }

        public static void DecodeR8G8B8ETC2Block(ReadOnlySpan<byte> etcword, Span<YFColor> decode_data)
        {
            fixed (byte* texPtr = etcword)
            {
                fixed (YFColor* dataPtr = decode_data)
                {
                    ETCDecoder.DecodeBlock_RGB_ETC2(texPtr, dataPtr);
                }
            }
        }

        public static void DecodeR8G8B8ETC1Block(ReadOnlySpan<byte> etcword, Span<YFColor> decode_data)
        {
            fixed (byte* texPtr = etcword)
            {
                fixed (YFColor* dataPtr = decode_data)
                {
                    ETCDecoder.DecodeBlock_RGB_ETC1(texPtr, dataPtr);
                }
            }
        }

        public static void EncodeR8G8B8A8ETC2EACBlock(Span<byte> etcword, ReadOnlySpan<YFColor> encode_data)
        {
            //fixed (byte* texPtr = etcword)
            //{
            //    fixed (YFColor* dataPtr = encode_data)
            //    {
            //        ETCDecoder.DecodeBlock_RGBA_ETC2_EAC(texPtr, dataPtr);
            //    }
            //}
        }

        public static void EncodeR8G8B8A1ETC2Block(Span<byte> etcword, ReadOnlySpan<YFColor> encode_data)
        {
            //fixed (byte* texPtr = etcword)
            //{
            //    fixed (YFColor* dataPtr = encode_data)
            //    {
            //        ETCDecoder.DecodeBlock_RGB_A1_ETC2(texPtr, dataPtr);
            //    }
            //}
        }

        public static void EncodeR8G8B8ETC2Block(Span<byte> etcword, ReadOnlySpan<YFColor> encode_data)
        {
            //fixed (byte* texPtr = etcword)
            //{
            //    fixed (YFColor* dataPtr = encode_data)
            //    {
            //        ETCDecoder.DecodeBlock_RGB_ETC2(texPtr, dataPtr);
            //    }
            //}
        }

        public static void EncodeR8G8B8ETC1Block(Span<byte> etcword, ReadOnlySpan<YFColor> encode_data)
        {
            fixed (YFColor* dataPtr = encode_data)
            {
                ulong data = ETC1Encoder.GenETC1(dataPtr);
                BinaryPrimitives.WriteUInt64BigEndian(etcword, data);
            }
        }

        private static class ETCDecoder
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static uint GETBITS(uint source, int size, int startpos)
            {
                return (((source) >> (startpos - size + 1)) & ((1U << (size)) - 1));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static uint GETBITSHIGH(uint source, int size, int startpos)
            {
                return ((source) >> (startpos - size - 31)) & ((1U << (size)) - 1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int SHIFT(int size, int startpos)
            {
                return startpos - size + 1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int SHIFTHIGH(int size, int startpos)
            {
                return startpos - size - 31;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int MASK(int size, int startpos)
            {
                return ((2 << (size - 1)) - 1) << SHIFT(size, startpos);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int MASKHIGH(int size, int startpos)
            {
                return ((1 << (size)) - 1) << SHIFTHIGH(size, startpos);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void PUTBITS(uint* dest, int data, int size, int startpos)
            {
                *dest = ((uint)(((int)*dest & ~MASK(size, startpos)) | ((data << SHIFT(size, startpos)) & MASK(size, startpos))));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void PUTBITSHIGH(uint* dest, int data, int size, int startpos)
            {
                *dest = ((uint)(((int)*dest & ~MASKHIGH(size, startpos)) | ((data << SHIFTHIGH(size, startpos)) & MASKHIGH(size, startpos))));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int CLAMP(int ll, int x, int ul)
            {
                return (((x) < (ll)) ? (ll) : (((x) > (ul)) ? (ul) : (x)));
            }

            static void unstuff57bits(uint planar_word1, uint planar_word2, uint* planar57_word1, uint* planar57_word2)
            {
                byte RO, GO1, GO2, BO1, BO2, BO3, RH1, RH2, GH, BH, RV, GV, BV;

                RO = (byte)GETBITSHIGH(planar_word1, 6, 62);
                GO1 = (byte)GETBITSHIGH(planar_word1, 1, 56);
                GO2 = (byte)GETBITSHIGH(planar_word1, 6, 54);
                BO1 = (byte)GETBITSHIGH(planar_word1, 1, 48);
                BO2 = (byte)GETBITSHIGH(planar_word1, 2, 44);
                BO3 = (byte)GETBITSHIGH(planar_word1, 3, 41);
                RH1 = (byte)GETBITSHIGH(planar_word1, 5, 38);
                RH2 = (byte)GETBITSHIGH(planar_word1, 1, 32);
                GH = (byte)GETBITS(planar_word2, 7, 31);
                BH = (byte)GETBITS(planar_word2, 6, 24);
                RV = (byte)GETBITS(planar_word2, 6, 18);
                GV = (byte)GETBITS(planar_word2, 7, 12);
                BV = (byte)GETBITS(planar_word2, 6, 5);

                *planar57_word1 = 0;
                *planar57_word2 = 0;
                PUTBITSHIGH(planar57_word1, RO, 6, 63);
                PUTBITSHIGH(planar57_word1, GO1, 1, 57);
                PUTBITSHIGH(planar57_word1, GO2, 6, 56);
                PUTBITSHIGH(planar57_word1, BO1, 1, 50);
                PUTBITSHIGH(planar57_word1, BO2, 2, 49);
                PUTBITSHIGH(planar57_word1, BO3, 3, 47);
                PUTBITSHIGH(planar57_word1, RH1, 5, 44);
                PUTBITSHIGH(planar57_word1, RH2, 1, 39);
                PUTBITSHIGH(planar57_word1, GH, 7, 38);
                PUTBITS(planar57_word2, BH, 6, 31);
                PUTBITS(planar57_word2, RV, 6, 25);
                PUTBITS(planar57_word2, GV, 7, 19);
                PUTBITS(planar57_word2, BV, 6, 12);
            }

            static void unstuff58bits(uint thumbH_word1, uint thumbH_word2, uint* thumbH58_word1, uint* thumbH58_word2)
            {
                uint part0, part1, part2, part3;

                // move parts
                part0 = GETBITSHIGH(thumbH_word1, 7, 62);
                part1 = GETBITSHIGH(thumbH_word1, 2, 52);
                part2 = GETBITSHIGH(thumbH_word1, 16, 49);
                part3 = GETBITSHIGH(thumbH_word1, 1, 32);
                *thumbH58_word1 = 0;
                PUTBITSHIGH(thumbH58_word1, (int)part0, 7, 57);
                PUTBITSHIGH(thumbH58_word1, (int)part1, 2, 50);
                PUTBITSHIGH(thumbH58_word1, (int)part2, 16, 48);
                PUTBITSHIGH(thumbH58_word1, (int)part3, 1, 32);

                *thumbH58_word2 = thumbH_word2;
            }

            static void unstuff59bits(uint thumbT_word1, uint thumbT_word2, uint* thumbT59_word1, uint* thumbT59_word2)
            {
                byte R0a;

                // Fix middle part
                *thumbT59_word1 = thumbT_word1 >> 1;
                // Fix db (lowest bit of d)
                PUTBITSHIGH(thumbT59_word1, (int)thumbT_word1, 1, 32);
                // Fix R0a (top two bits of R0)
                R0a = (byte)GETBITSHIGH(thumbT_word1, 2, 60);
                PUTBITSHIGH(thumbT59_word1, R0a, 2, 58);

                // Zero top part (not needed)
                PUTBITSHIGH(thumbT59_word1, 0, 5, 63);

                *thumbT59_word2 = thumbT_word2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void decompressColor(int R_B, int G_B, int B_B, byte[,] colors_RGB444, byte[,] colors)
            {

                colors[0, 0] = (byte)((colors_RGB444[0, 0] << (8 - R_B)) | (colors_RGB444[0, 0] >> (R_B - (8 - R_B))));
                colors[0, 1] = (byte)((colors_RGB444[0, 1] << (8 - G_B)) | (colors_RGB444[0, 1] >> (G_B - (8 - G_B))));
                colors[0, 2] = (byte)((colors_RGB444[0, 2] << (8 - B_B)) | (colors_RGB444[0, 2] >> (B_B - (8 - B_B))));
                colors[1, 0] = (byte)((colors_RGB444[1, 0] << (8 - R_B)) | (colors_RGB444[1, 0] >> (R_B - (8 - R_B))));
                colors[1, 1] = (byte)((colors_RGB444[1, 1] << (8 - G_B)) | (colors_RGB444[1, 1] >> (G_B - (8 - G_B))));
                colors[1, 2] = (byte)((colors_RGB444[1, 2] << (8 - B_B)) | (colors_RGB444[1, 2] >> (B_B - (8 - B_B))));
            }

            static void decompressBlockPlanar57c(uint compressed57_1, uint compressed57_2, YFColor* img)
            {
                byte* colorO = stackalloc byte[3];
                byte* colorH = stackalloc byte[3];
                byte* colorV = stackalloc byte[3];

                colorO[0] = (byte)GETBITSHIGH(compressed57_1, 6, 63);
                colorO[1] = (byte)GETBITSHIGH(compressed57_1, 7, 57);
                colorO[2] = (byte)GETBITSHIGH(compressed57_1, 6, 50);
                colorH[0] = (byte)GETBITSHIGH(compressed57_1, 6, 44);
                colorH[1] = (byte)GETBITSHIGH(compressed57_1, 7, 38);
                colorH[2] = (byte)GETBITS(compressed57_2, 6, 31);
                colorV[0] = (byte)GETBITS(compressed57_2, 6, 25);
                colorV[1] = (byte)GETBITS(compressed57_2, 7, 19);
                colorV[2] = (byte)GETBITS(compressed57_2, 6, 12);

                colorO[0] = (byte)((colorO[0] << 2) | (colorO[0] >> 4));
                colorO[1] = (byte)((colorO[1] << 1) | (colorO[1] >> 6));
                colorO[2] = (byte)((colorO[2] << 2) | (colorO[2] >> 4));

                colorH[0] = (byte)((colorH[0] << 2) | (colorH[0] >> 4));
                colorH[1] = (byte)((colorH[1] << 1) | (colorH[1] >> 6));
                colorH[2] = (byte)((colorH[2] << 2) | (colorH[2] >> 4));

                colorV[0] = (byte)((colorV[0] << 2) | (colorV[0] >> 4));
                colorV[1] = (byte)((colorV[1] << 1) | (colorV[1] >> 6));
                colorV[2] = (byte)((colorV[2] << 2) | (colorV[2] >> 4));

                int xx, yy;

                for (xx = 0; xx < 4; xx++)
                {
                    for (yy = 0; yy < 4; yy++)
                    {
                        img[(yy << 2) | xx].Red = (byte)CLAMP(0, (xx * (colorH[0] - colorO[0]) + yy * (colorV[0] - colorO[0]) + 4 * colorO[0] + 2) >> 2, 255);
                        img[(yy << 2) | xx].Green = (byte)CLAMP(0, (xx * (colorH[1] - colorO[1]) + yy * (colorV[1] - colorO[1]) + 4 * colorO[1] + 2) >> 2, 255);
                        img[(yy << 2) | xx].Blue = (byte)CLAMP(0, (xx * (colorH[2] - colorO[2]) + yy * (colorV[2] - colorO[2]) + 4 * colorO[2] + 2) >> 2, 255);

                        //Equivalent method
                        /*img[channels*width*(starty+yy) + channels*(startx+xx) + 0] = (int)CLAMP(0, JAS_ROUND((xx*(colorH[0]-colorO[0])/4.0 + yy*(colorV[0]-colorO[0])/4.0 + colorO[0])), 255);
                        img[channels*width*(starty+yy) + channels*(startx+xx) + 1] = (int)CLAMP(0, JAS_ROUND((xx*(colorH[1]-colorO[1])/4.0 + yy*(colorV[1]-colorO[1])/4.0 + colorO[1])), 255);
                        img[channels*width*(starty+yy) + channels*(startx+xx) + 2] = (int)CLAMP(0, JAS_ROUND((xx*(colorH[2]-colorO[2])/4.0 + yy*(colorV[2]-colorO[2])/4.0 + colorO[2])), 255);*/

                    }
                }
            }

            static void calculatePaintColors58H(byte d, byte p, byte[,] colors, byte[,] possible_colors)
            {
                possible_colors[3, 0] = (byte)CLAMP(0, colors[1, 0] - table58H[d], 255);
                possible_colors[3, 1] = (byte)CLAMP(0, colors[1, 1] - table58H[d], 255);
                possible_colors[3, 2] = (byte)CLAMP(0, colors[1, 2] - table58H[d], 255);

                if (p == 0)
                {
                    // C1
                    possible_colors[0, 0] = (byte)CLAMP(0, colors[0, 0] + table58H[d], 255);
                    possible_colors[0, 1] = (byte)CLAMP(0, colors[0, 1] + table58H[d], 255);
                    possible_colors[0, 2] = (byte)CLAMP(0, colors[0, 2] + table58H[d], 255);
                    // C2
                    possible_colors[1, 0] = (byte)CLAMP(0, colors[0, 0] - table58H[d], 255);
                    possible_colors[1, 1] = (byte)CLAMP(0, colors[0, 1] - table58H[d], 255);
                    possible_colors[1, 2] = (byte)CLAMP(0, colors[0, 2] - table58H[d], 255);
                    // C3
                    possible_colors[2, 0] = (byte)CLAMP(0, colors[1, 0] + table58H[d], 255);
                    possible_colors[2, 1] = (byte)CLAMP(0, colors[1, 1] + table58H[d], 255);
                    possible_colors[2, 2] = (byte)CLAMP(0, colors[1, 2] + table58H[d], 255);
                }
                else
                {
                    throw new Exception("Invalid pattern. Terminating");
                }
            }

            static void decompressBlockTHUMB58Hc(uint block_part1, uint block_part2, YFColor* img)
            {
                uint col0, col1;
                byte[,] colors = new byte[2, 3];
                byte[,] colorsRGB444 = new byte[2, 3];
                byte[,] paint_colors = new byte[4, 3];
                byte distance;
                byte[,] block_mask = new byte[4, 4];

                // First decode left part of block.
                colorsRGB444[0, 0] = (byte)GETBITSHIGH(block_part1, 4, 57);
                colorsRGB444[0, 1] = (byte)GETBITSHIGH(block_part1, 4, 53);
                colorsRGB444[0, 2] = (byte)GETBITSHIGH(block_part1, 4, 49);

                colorsRGB444[1, 0] = (byte)GETBITSHIGH(block_part1, 4, 45);
                colorsRGB444[1, 1] = (byte)GETBITSHIGH(block_part1, 4, 41);
                colorsRGB444[1, 2] = (byte)GETBITSHIGH(block_part1, 4, 37);

                distance = 0;
                distance = (byte)((GETBITSHIGH(block_part1, 2, 33)) << 1);

                col0 = GETBITSHIGH(block_part1, 12, 57);
                col1 = GETBITSHIGH(block_part1, 12, 45);

                if (col0 >= col1)
                {
                    distance |= 1;
                }

                // Extend the two colors to RGB888	
                decompressColor(4, 4, 4, colorsRGB444, colors);

                calculatePaintColors58H(distance, 0, colors, paint_colors);

                // Choose one of the four paint colors for each texel
                for (byte x = 0; x < 4; ++x)
                {
                    for (byte y = 0; y < 4; ++y)
                    {
                        //block_mask[x][y] = GETBITS(block_part2,2,31-(y*4+x)*2);
                        block_mask[x, y] = (byte)(GETBITS(block_part2, 1, (y + x * 4) + 16) << 1);
                        block_mask[x, y] |= (byte)GETBITS(block_part2, 1, (y + x * 4));
                        img[(y << 2) | x].Red =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 0], 255); // RED
                        img[(y << 2) | x].Green =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 1], 255); // GREEN
                        img[(y << 2) | x].Blue =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 2], 255); // BLUE
                    }
                }
            }

            static void calculatePaintColors59T(byte d, byte p, byte[,] colors, byte[,] possible_colors)
            {
                possible_colors[3, 0] = (byte)CLAMP(0, colors[1, 0] - table59T[d], 255);
                possible_colors[3, 1] = (byte)CLAMP(0, colors[1, 1] - table59T[d], 255);
                possible_colors[3, 2] = (byte)CLAMP(0, colors[1, 2] - table59T[d], 255);

                if (p == 1)
                {
                    possible_colors[0, 0] = colors[0, 0];
                    possible_colors[0, 1] = colors[0, 1];
                    possible_colors[0, 2] = colors[0, 2];
                    // C2
                    possible_colors[1, 0] = (byte)CLAMP(0, colors[1, 0] + table59T[d], 255);
                    possible_colors[1, 1] = (byte)CLAMP(0, colors[1, 1] + table59T[d], 255);
                    possible_colors[1, 2] = (byte)CLAMP(0, colors[1, 2] + table59T[d], 255);
                    // C1
                    possible_colors[2, 0] = colors[1, 0];
                    possible_colors[2, 1] = colors[1, 1];
                    possible_colors[2, 2] = colors[1, 2];

                }
                else
                {
                    throw new Exception("Invalid pattern. Terminating");
                }
            }

            static void decompressBlockTHUMB59Tc(uint block_part1, uint block_part2, YFColor* img)
            {
                byte[,] colorsRGB444 = new byte[2, 3];
                byte[,] colors = new byte[2, 3];
                byte[,] paint_colors = new byte[4, 3];
                byte distance;
                byte[,] block_mask = new byte[4, 4];

                // First decode left part of block.
                colorsRGB444[0, 0] = (byte)GETBITSHIGH(block_part1, 4, 58);
                colorsRGB444[0, 1] = (byte)GETBITSHIGH(block_part1, 4, 54);
                colorsRGB444[0, 2] = (byte)GETBITSHIGH(block_part1, 4, 50);

                colorsRGB444[1, 0] = (byte)GETBITSHIGH(block_part1, 4, 46);
                colorsRGB444[1, 1] = (byte)GETBITSHIGH(block_part1, 4, 42);
                colorsRGB444[1, 2] = (byte)GETBITSHIGH(block_part1, 4, 38);

                distance = (byte)GETBITSHIGH(block_part1, 3, 34);

                // Extend the two colors to RGB888	
                decompressColor(4, 4, 4, colorsRGB444, colors);
                calculatePaintColors59T(distance, 1, colors, paint_colors);

                // Choose one of the four paint colors for each texel
                for (byte x = 0; x < 4; ++x)
                {
                    for (byte y = 0; y < 4; ++y)
                    {
                        //block_mask[x][y] = GETBITS(block_part2,2,31-(y*4+x)*2);
                        block_mask[x, y] = (byte)(GETBITS(block_part2, 1, y + x * 4 + 16) << 1);
                        block_mask[x, y] |= (byte)GETBITS(block_part2, 1, y + x * 4);
                        img[(y << 2) | x].Red = (byte)CLAMP(0, paint_colors[block_mask[x, y], 0], 255); // RED
                        img[(y << 2) | x].Green = (byte)CLAMP(0, paint_colors[block_mask[x, y], 1], 255); // GREEN
                        img[(y << 2) | x].Blue = (byte)CLAMP(0, paint_colors[block_mask[x, y], 2], 255); // BLUE
                    }
                }
            }

            static void decompressBlockETC2c(uint block_part1, uint block_part2, YFColor* img)
            {
                int diffbit;
                sbyte* color1 = stackalloc sbyte[3];
                sbyte* diff = stackalloc sbyte[3];
                sbyte red, green, blue;

                diffbit = (int)GETBITSHIGH(block_part1, 1, 33);

                if (diffbit != 0)
                {
                    // We have diffbit = 1;

                    // Base color
                    color1[0] = (sbyte)GETBITSHIGH(block_part1, 5, 63);
                    color1[1] = (sbyte)GETBITSHIGH(block_part1, 5, 55);
                    color1[2] = (sbyte)GETBITSHIGH(block_part1, 5, 47);

                    // Diff color
                    diff[0] = (sbyte)GETBITSHIGH(block_part1, 3, 58);
                    diff[1] = (sbyte)GETBITSHIGH(block_part1, 3, 50);
                    diff[2] = (sbyte)GETBITSHIGH(block_part1, 3, 42);

                    // Extend sign bit to entire byte. 
                    diff[0] <<= 5;
                    diff[1] <<= 5;
                    diff[2] <<= 5;
                    diff[0] >>= 5;
                    diff[1] >>= 5;
                    diff[2] >>= 5;

                    red = (sbyte)(color1[0] + diff[0]);
                    green = (sbyte)(color1[1] + diff[1]);
                    blue = (sbyte)(color1[2] + diff[2]);

                    if (red < 0 || red > 31)
                    {
                        uint block59_part1, block59_part2;
                        unstuff59bits(block_part1, block_part2, &block59_part1, &block59_part2);
                        decompressBlockTHUMB59Tc(block59_part1, block59_part2, img);
                    }
                    else if (green < 0 || green > 31)
                    {
                        uint block58_part1, block58_part2;
                        unstuff58bits(block_part1, block_part2, &block58_part1, &block58_part2);
                        decompressBlockTHUMB58Hc(block58_part1, block58_part2, img);
                    }
                    else if (blue < 0 || blue > 31)
                    {
                        uint block57_part1, block57_part2;

                        unstuff57bits(block_part1, block_part2, &block57_part1, &block57_part2);
                        decompressBlockPlanar57c(block57_part1, block57_part2, img);
                    }
                    else
                    {
                        decompressBlockDiffFlipC(block_part1, block_part2, img);
                    }
                }
                else
                {
                    // We have diffbit = 0;
                    decompressBlockDiffFlipC(block_part1, block_part2, img);
                }
            }

            static void decompressBlockDifferentialWithAlphaC(uint block_part1, uint block_part2, YFColor* img)
            {
                byte* avg_color = stackalloc byte[3];
                byte* enc_color1 = stackalloc byte[3];
                byte* enc_color2 = stackalloc byte[3];
                sbyte* diff = stackalloc sbyte[3];
                int table;
                int index, shift;
                int r, g, b;
                int diffbit;
                int flipbit;

                //the diffbit now encodes whether or not the entire alpha channel is 255.
                diffbit = (int)GETBITSHIGH(block_part1, 1, 33);
                flipbit = (int)GETBITSHIGH(block_part1, 1, 32);

                // First decode left part of block.
                enc_color1[0] = (byte)GETBITSHIGH(block_part1, 5, 63);
                enc_color1[1] = (byte)GETBITSHIGH(block_part1, 5, 55);
                enc_color1[2] = (byte)GETBITSHIGH(block_part1, 5, 47);

                // Expand from 5 to 8 bits
                avg_color[0] = (byte)((enc_color1[0] << 3) | (enc_color1[0] >> 2));
                avg_color[1] = (byte)((enc_color1[1] << 3) | (enc_color1[1] >> 2));
                avg_color[2] = (byte)((enc_color1[2] << 3) | (enc_color1[2] >> 2));

                table = (int)(GETBITSHIGH(block_part1, 3, 39) << 1);

                uint pixel_indices_MSB, pixel_indices_LSB;

                pixel_indices_MSB = GETBITS(block_part2, 16, 31);
                pixel_indices_LSB = GETBITS(block_part2, 16, 15);

                if ((flipbit) == 0)
                {
                    // We should not flip
                    shift = 0;
                    for (int x = 0; x < 2; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                            index |= (int)((pixel_indices_LSB >> shift) & 1);
                            shift++;
                            index = unscramble[index];

                            int mod = compressParams[table, index];
                            if (diffbit == 0 && (index == 1 || index == 2))
                            {
                                mod = 0;
                            }

                            r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + mod, 255);
                            g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + mod, 255);
                            b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + mod, 255);
                            if (diffbit == 0 && index == 1)
                            {
                                img[(y << 2) | x].Alpha = 0;
                                r = img[(y << 2) | x].Red = 0;
                                g = img[(y << 2) | x].Green = 0;
                                b = img[(y << 2) | x].Blue = 0;
                            }
                            else
                            {
                                img[(y << 2) | x].Alpha = 255;
                            }

                        }
                    }
                }
                else
                {
                    // We should flip
                    shift = 0;
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 2; y++)
                        {
                            index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                            index |= (int)((pixel_indices_LSB >> shift) & 1);
                            shift++;
                            index = unscramble[index];
                            int mod = compressParams[table, index];
                            if (diffbit == 0 && (index == 1 || index == 2))
                            {
                                mod = 0;
                            }
                            r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + mod, 255);
                            g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + mod, 255);
                            b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + mod, 255);
                            if (diffbit == 0 && index == 1)
                            {
                                img[(y << 2) | x].Alpha = 0;
                                r = img[(y << 2) | x].Red = 0;
                                g = img[(y << 2) | x].Green = 0;
                                b = img[(y << 2) | x].Blue = 0;
                            }
                            else
                            {
                                img[(y << 2) | x].Alpha = 255;
                            }
                        }
                        shift += 2;
                    }
                }
                // Now decode right part of block. 
                diff[0] = (sbyte)GETBITSHIGH(block_part1, 3, 58);
                diff[1] = (sbyte)GETBITSHIGH(block_part1, 3, 50);
                diff[2] = (sbyte)GETBITSHIGH(block_part1, 3, 42);

                // Extend sign bit to entire byte. 
                diff[0] <<= 5;
                diff[1] <<= 5;
                diff[2] <<= 5;
                diff[0] >>= 5;
                diff[1] >>= 5;
                diff[2] >>= 5;

                //  Calculate second color
                enc_color2[0] = (byte)(enc_color1[0] + diff[0]);
                enc_color2[1] = (byte)(enc_color1[1] + diff[1]);
                enc_color2[2] = (byte)(enc_color1[2] + diff[2]);

                // Expand from 5 to 8 bits
                avg_color[0] = (byte)((enc_color2[0] << 3) | (enc_color2[0] >> 2));
                avg_color[1] = (byte)((enc_color2[1] << 3) | (enc_color2[1] >> 2));
                avg_color[2] = (byte)((enc_color2[2] << 3) | (enc_color2[2] >> 2));

                table = (int)(GETBITSHIGH(block_part1, 3, 36) << 1);
                pixel_indices_MSB = GETBITS(block_part2, 16, 31);
                pixel_indices_LSB = GETBITS(block_part2, 16, 15);

                if ((flipbit) == 0)
                {
                    // We should not flip
                    shift = 8;
                    for (int x = 2; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                            index |= (int)((pixel_indices_LSB >> shift) & 1);
                            shift++;
                            index = unscramble[index];
                            int mod = compressParams[table, index];
                            if (diffbit == 0 && (index == 1 || index == 2))
                            {
                                mod = 0;
                            }

                            r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + mod, 255);
                            g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + mod, 255);
                            b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + mod, 255);
                            if (diffbit == 0 && index == 1)
                            {
                                img[(y << 2) | x].Alpha = 0;
                                r = img[(y << 2) | x].Red = 0;
                                g = img[(y << 2) | x].Green = 0;
                                b = img[(y << 2) | x].Blue = 0;
                            }
                            else
                            {
                                img[(y << 2) | x].Alpha = 255;
                            }
                        }
                    }
                }
                else
                {
                    // We should flip
                    shift = 2;
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 2; y < 4; y++)
                        {
                            index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                            index |= (int)((pixel_indices_LSB >> shift) & 1);
                            shift++;
                            index = unscramble[index];
                            int mod = compressParams[table, index];
                            if (diffbit == 0 && (index == 1 || index == 2))
                            {
                                mod = 0;
                            }

                            r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + mod, 255);
                            g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + mod, 255);
                            b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + mod, 255);
                            if (diffbit == 0 && index == 1)
                            {
                                img[(y << 2) | x].Alpha = 0;
                                r = img[(y << 2) | x].Red = 0;
                                g = img[(y << 2) | x].Green = 0;
                                b = img[(y << 2) | x].Blue = 0;
                            }
                            else
                            {
                                img[(y << 2) | x].Alpha = 255;
                            }
                        }
                        shift += 2;
                    }
                }
            }

            static void decompressBlockTHUMB58HAlphaC(uint block_part1, uint block_part2, YFColor* img)
            {
                uint col0, col1;
                byte[,] colors = new byte[2, 3];
                byte[,] colorsRGB444 = new byte[2, 3];
                byte[,] paint_colors = new byte[4, 3];
                byte distance;
                byte[,] block_mask = new byte[4, 4];

                // First decode left part of block.
                colorsRGB444[0, 0] = (byte)GETBITSHIGH(block_part1, 4, 57);
                colorsRGB444[0, 1] = (byte)GETBITSHIGH(block_part1, 4, 53);
                colorsRGB444[0, 2] = (byte)GETBITSHIGH(block_part1, 4, 49);

                colorsRGB444[1, 0] = (byte)GETBITSHIGH(block_part1, 4, 45);
                colorsRGB444[1, 1] = (byte)GETBITSHIGH(block_part1, 4, 41);
                colorsRGB444[1, 2] = (byte)GETBITSHIGH(block_part1, 4, 37);

                distance = 0;
                distance = (byte)((GETBITSHIGH(block_part1, 2, 33)) << 1);

                col0 = GETBITSHIGH(block_part1, 12, 57);
                col1 = GETBITSHIGH(block_part1, 12, 45);

                if (col0 >= col1)
                {
                    distance |= 1;
                }

                // Extend the two colors to RGB888	
                decompressColor(4, 4, 4, colorsRGB444, colors);

                calculatePaintColors58H(distance, 0, colors, paint_colors);

                // Choose one of the four paint colors for each texel
                for (byte x = 0; x < 4; ++x)
                {
                    for (byte y = 0; y < 4; ++y)
                    {
                        //block_mask[x][y] = GETBITS(block_part2,2,31-(y*4+x)*2);
                        block_mask[x, y] = (byte)(GETBITS(block_part2, 1, (y + x * 4) + 16) << 1);
                        block_mask[x, y] |= (byte)GETBITS(block_part2, 1, (y + x * 4));
                        img[(y << 2) | x].Red =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 0], 255); // RED
                        img[(y << 2) | x].Green =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 1], 255); // GREEN
                        img[(y << 2) | x].Blue =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 2], 255); // BLUE

                        if (block_mask[x, y] == 2)
                        {
                            img[(y << 2) | x].Alpha = 0;
                            img[(y << 2) | x].Red = 0;
                            img[(y << 2) | x].Green = 0;
                            img[(y << 2) | x].Blue = 0;
                        }
                        else
                        {
                            img[(y << 2) | x].Alpha = 255;
                        }
                    }
                }
            }

            static void decompressBlockTHUMB59TAlphaC(uint block_part1, uint block_part2, YFColor* img)
            {

                byte[,] colorsRGB444 = new byte[2, 3];
                byte[,] colors = new byte[2, 3];
                byte[,] paint_colors = new byte[4, 3];
                byte distance;
                byte[,] block_mask = new byte[4, 4];

                // First decode left part of block.
                colorsRGB444[0, 0] = (byte)GETBITSHIGH(block_part1, 4, 58);
                colorsRGB444[0, 1] = (byte)GETBITSHIGH(block_part1, 4, 54);
                colorsRGB444[0, 2] = (byte)GETBITSHIGH(block_part1, 4, 50);

                colorsRGB444[1, 0] = (byte)GETBITSHIGH(block_part1, 4, 46);
                colorsRGB444[1, 1] = (byte)GETBITSHIGH(block_part1, 4, 42);
                colorsRGB444[1, 2] = (byte)GETBITSHIGH(block_part1, 4, 38);

                distance = (byte)GETBITSHIGH(block_part1, 3, 34);

                // Extend the two colors to RGB888	
                decompressColor(4, 4, 4, colorsRGB444, colors);
                calculatePaintColors59T(distance, 1, colors, paint_colors);

                // Choose one of the four paint colors for each texel
                for (byte x = 0; x < 4; ++x)
                {
                    for (byte y = 0; y < 4; ++y)
                    {
                        //block_mask[x][y] = GETBITS(block_part2,2,31-(y*4+x)*2);
                        block_mask[x, y] = (byte)(GETBITS(block_part2, 1, (y + x * 4) + 16) << 1);
                        block_mask[x, y] |= (byte)GETBITS(block_part2, 1, (y + x * 4));
                        img[(y << 2) | x].Red =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 0], 255); // RED
                        img[(y << 2) | x].Green =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 1], 255); // GREEN
                        img[(y << 2) | x].Blue =
                            (byte)CLAMP(0, paint_colors[block_mask[x, y], 2], 255); // BLUE
                        if (block_mask[x, y] == 2)
                        {
                            img[(y << 2) | x].Alpha = 0;
                            img[(y << 2) | x].Red = 0;
                            img[(y << 2) | x].Green = 0;
                            img[(y << 2) | x].Blue = 0;
                        }
                        else
                        {
                            img[(y << 2) | x].Alpha = 0xFF;
                        }
                    }
                }
            }

            static void decompressBlockETC21BitAlphaC(uint block_part1, uint block_part2, YFColor* img)
            {
                int diffbit;
                sbyte* color1 = stackalloc sbyte[3];
                sbyte* diff = stackalloc sbyte[3];
                sbyte red, green, blue;

                diffbit = (int)GETBITSHIGH(block_part1, 1, 33);

                if (diffbit != 0)
                {
                    // We have diffbit = 1, meaning no transparent pixels. regular decompression.

                    // Base color
                    color1[0] = (sbyte)GETBITSHIGH(block_part1, 5, 63);
                    color1[1] = (sbyte)GETBITSHIGH(block_part1, 5, 55);
                    color1[2] = (sbyte)GETBITSHIGH(block_part1, 5, 47);

                    // Diff color
                    diff[0] = (sbyte)GETBITSHIGH(block_part1, 3, 58);
                    diff[1] = (sbyte)GETBITSHIGH(block_part1, 3, 50);
                    diff[2] = (sbyte)GETBITSHIGH(block_part1, 3, 42);

                    // Extend sign bit to entire byte. 
                    diff[0] <<= 5;
                    diff[1] <<= 5;
                    diff[2] <<= 5;
                    diff[0] >>= 5;
                    diff[1] >>= 5;
                    diff[2] >>= 5;

                    red = (sbyte)(color1[0] + diff[0]);
                    green = (sbyte)(color1[1] + diff[1]);
                    blue = (sbyte)(color1[2] + diff[2]);

                    if (red < 0 || red > 31)
                    {
                        uint block59_part1, block59_part2;
                        unstuff59bits(block_part1, block_part2, &block59_part1, &block59_part2);
                        decompressBlockTHUMB59Tc(block59_part1, block59_part2, img);
                    }
                    else if (green < 0 || green > 31)
                    {
                        uint block58_part1, block58_part2;
                        unstuff58bits(block_part1, block_part2, &block58_part1, &block58_part2);
                        decompressBlockTHUMB58Hc(block58_part1, block58_part2, img);
                    }
                    else if (blue < 0 || blue > 31)
                    {
                        uint block57_part1, block57_part2;

                        unstuff57bits(block_part1, block_part2, &block57_part1, &block57_part2);
                        decompressBlockPlanar57c(block57_part1, block57_part2, img);
                    }
                    else
                    {
                        decompressBlockDifferentialWithAlphaC(block_part1, block_part2, img);
                    }
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            img[(y << 2) | x].Alpha = 0xFF;
                        }
                    }
                }
                else
                {
                    // We have diffbit = 0, transparent pixels. Only T-, H- or regular diff-mode possible.

                    // Base color
                    color1[0] = (sbyte)GETBITSHIGH(block_part1, 5, 63);
                    color1[1] = (sbyte)GETBITSHIGH(block_part1, 5, 55);
                    color1[2] = (sbyte)GETBITSHIGH(block_part1, 5, 47);

                    // Diff color
                    diff[0] = (sbyte)GETBITSHIGH(block_part1, 3, 58);
                    diff[1] = (sbyte)GETBITSHIGH(block_part1, 3, 50);
                    diff[2] = (sbyte)GETBITSHIGH(block_part1, 3, 42);

                    // Extend sign bit to entire byte. 
                    diff[0] <<= 5;
                    diff[1] <<= 5;
                    diff[2] <<= 5;
                    diff[0] >>= 5;
                    diff[1] >>= 5;
                    diff[2] >>= 5;

                    red = (sbyte)(color1[0] + diff[0]);
                    green = (sbyte)(color1[1] + diff[1]);
                    blue = (sbyte)(color1[2] + diff[2]);
                    if (red < 0 || red > 31)
                    {
                        uint block59_part1, block59_part2;
                        unstuff59bits(block_part1, block_part2, &block59_part1, &block59_part2);
                        decompressBlockTHUMB59TAlphaC(block59_part1, block59_part2, img);
                    }
                    else if (green < 0 || green > 31)
                    {
                        uint block58_part1, block58_part2;
                        unstuff58bits(block_part1, block_part2, &block58_part1, &block58_part2);
                        decompressBlockTHUMB58HAlphaC(block58_part1, block58_part2, img);
                    }
                    else if (blue < 0 || blue > 31)
                    {
                        uint block57_part1, block57_part2;

                        unstuff57bits(block_part1, block_part2, &block57_part1, &block57_part2);
                        decompressBlockPlanar57c(block57_part1, block57_part2, img);
                        for (int x = 0; x < 4; x++)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                img[(y << 2) | x].Alpha = 0xFF;
                            }
                        }
                    }
                    else
                    {
                        decompressBlockDifferentialWithAlphaC(block_part1, block_part2, img);
                    }
                }
            }

            static void decompressBlockDiffFlipC(uint block_part1, uint block_part2, YFColor* img)
            {
                byte* avg_color = stackalloc byte[3];
                byte* enc_color1 = stackalloc byte[3];
                byte* enc_color2 = stackalloc byte[3];
                sbyte* diff = stackalloc sbyte[3];
                int table;
                int index, shift;
                int r, g, b;
                int diffbit;
                int flipbit;

                diffbit = (int)GETBITSHIGH(block_part1, 1, 33);
                flipbit = (int)GETBITSHIGH(block_part1, 1, 32);

                if (diffbit == 0)
                {
                    // We have diffbit = 0.

                    // First decode left part of block.
                    avg_color[0] = (byte)GETBITSHIGH(block_part1, 4, 63);
                    avg_color[1] = (byte)GETBITSHIGH(block_part1, 4, 55);
                    avg_color[2] = (byte)GETBITSHIGH(block_part1, 4, 47);

                    // Here, we should really multiply by 17 instead of 16. This can
                    // be done by just copying the four lower bits to the upper ones
                    // while keeping the lower bits.
                    avg_color[0] |= (byte)(avg_color[0] << 4);
                    avg_color[1] |= (byte)(avg_color[1] << 4);
                    avg_color[2] |= (byte)(avg_color[2] << 4);

                    table = (int)GETBITSHIGH(block_part1, 3, 39) << 1;

                    uint pixel_indices_MSB, pixel_indices_LSB;

                    pixel_indices_MSB = GETBITS(block_part2, 16, 31);
                    pixel_indices_LSB = GETBITS(block_part2, 16, 15);

                    if ((flipbit) == 0)
                    {
                        // We should not flip
                        shift = 0;
                        for (int x = 0; x < 2; x++)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                        }
                    }
                    else
                    {
                        // We should flip
                        shift = 0;
                        for (int x = 0; x < 4; x++)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                            shift += 2;
                        }
                    }

                    // Now decode other part of block. 
                    avg_color[0] = (byte)GETBITSHIGH(block_part1, 4, 59);
                    avg_color[1] = (byte)GETBITSHIGH(block_part1, 4, 51);
                    avg_color[2] = (byte)GETBITSHIGH(block_part1, 4, 43);

                    // Here, we should really multiply by 17 instead of 16. This can
                    // be done by just copying the four lower bits to the upper ones
                    // while keeping the lower bits.
                    avg_color[0] |= (byte)(avg_color[0] << 4);
                    avg_color[1] |= (byte)(avg_color[1] << 4);
                    avg_color[2] |= (byte)(avg_color[2] << 4);

                    table = (int)(GETBITSHIGH(block_part1, 3, 36) << 1);
                    pixel_indices_MSB = GETBITS(block_part2, 16, 31);
                    pixel_indices_LSB = GETBITS(block_part2, 16, 15);

                    if ((flipbit) == 0)
                    {
                        // We should not flip
                        shift = 8;
                        for (int x = 2; x < 4; x++)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                        }
                    }
                    else
                    {
                        // We should flip
                        shift = 2;
                        for (int x = 0; x < 4; x++)
                        {
                            for (int y = 2; y < 4; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                            shift += 2;
                        }
                    }
                }
                else
                {
                    // We have diffbit = 1.

                    // First decode left part of block.
                    enc_color1[0] = (byte)GETBITSHIGH(block_part1, 5, 63);
                    enc_color1[1] = (byte)GETBITSHIGH(block_part1, 5, 55);
                    enc_color1[2] = (byte)GETBITSHIGH(block_part1, 5, 47);

                    // Expand from 5 to 8 bits
                    avg_color[0] = (byte)((enc_color1[0] << 3) | (enc_color1[0] >> 2));
                    avg_color[1] = (byte)((enc_color1[1] << 3) | (enc_color1[1] >> 2));
                    avg_color[2] = (byte)((enc_color1[2] << 3) | (enc_color1[2] >> 2));

                    table = (int)(GETBITSHIGH(block_part1, 3, 39) << 1);

                    uint pixel_indices_MSB, pixel_indices_LSB;

                    pixel_indices_MSB = GETBITS(block_part2, 16, 31);
                    pixel_indices_LSB = GETBITS(block_part2, 16, 15);

                    if ((flipbit) == 0)
                    {
                        // We should not flip
                        shift = 0;
                        for (int x = 0; x < 2; x++)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                        }
                    }
                    else
                    {
                        // We should flip
                        shift = 0;
                        for (int x = 0; x < 4; x++)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                            shift += 2;
                        }
                    }

                    // Now decode right part of block. 
                    diff[0] = (sbyte)GETBITSHIGH(block_part1, 3, 58);
                    diff[1] = (sbyte)GETBITSHIGH(block_part1, 3, 50);
                    diff[2] = (sbyte)GETBITSHIGH(block_part1, 3, 42);

                    // Extend sign bit to entire byte. 
                    diff[0] = (sbyte)(diff[0] << 5);
                    diff[1] = (sbyte)(diff[1] << 5);
                    diff[2] = (sbyte)(diff[2] << 5);
                    diff[0] >>= 5;
                    diff[1] >>= 5;
                    diff[2] >>= 5;

                    //  Calculale second color
                    enc_color2[0] = (byte)(enc_color1[0] + diff[0]);
                    enc_color2[1] = (byte)(enc_color1[1] + diff[1]);
                    enc_color2[2] = (byte)(enc_color1[2] + diff[2]);

                    // Expand from 5 to 8 bits
                    avg_color[0] = (byte)((enc_color2[0] << 3) | (enc_color2[0] >> 2));
                    avg_color[1] = (byte)((enc_color2[1] << 3) | (enc_color2[1] >> 2));
                    avg_color[2] = (byte)((enc_color2[2] << 3) | (enc_color2[2] >> 2));

                    table = (int)(GETBITSHIGH(block_part1, 3, 36) << 1);
                    pixel_indices_MSB = GETBITS(block_part2, 16, 31);
                    pixel_indices_LSB = GETBITS(block_part2, 16, 15);

                    if ((flipbit) == 0)
                    {
                        // We should not flip
                        shift = 8;
                        for (int x = 2; x < 4; x++)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                        }
                    }
                    else
                    {
                        // We should flip
                        shift = 2;
                        for (int x = 0; x < 4; x++)
                        {
                            for (int y = 2; y < 4; y++)
                            {
                                index = (int)(((pixel_indices_MSB >> shift) & 1) << 1);
                                index |= (int)((pixel_indices_LSB >> shift) & 1);
                                shift++;
                                index = unscramble[index];

                                r = img[(y << 2) | x].Red = (byte)CLAMP(0, avg_color[0] + compressParams[table, index], 255);
                                g = img[(y << 2) | x].Green = (byte)CLAMP(0, avg_color[1] + compressParams[table, index], 255);
                                b = img[(y << 2) | x].Blue = (byte)CLAMP(0, avg_color[2] + compressParams[table, index], 255);
                            }
                            shift += 2;
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static byte getbit(byte input, int frompos, int topos)
            {
                if (frompos > topos)
                    return (byte)(((1 << frompos) & input) >> (frompos - topos));
                return (byte)(((1 << frompos) & input) << (topos - frompos));
            }

            static void decompressBlockAlphaC(byte* data, YFColor* img)
            {
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
                        img[(y << 2) | x].Alpha = (byte)CLAMP(0, alpha + alphaTable[table, index], 255);
                    }
                }
            }

            static int[,] alphaTable = new int[256, 8]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { -3, -6, -9, -15, 2, 5, 8, 14 },
                { -3, -7, -10, -13, 2, 6, 9, 12 },
                { -2, -5, -8, -13, 1, 4, 7, 12 },
                { -2, -4, -6, -13, 1, 3, 5, 12 },
                { -3, -6, -8, -12, 2, 5, 7, 11 },
                { -3, -7, -9, -11, 2, 6, 8, 10 },
                { -4, -7, -8, -11, 3, 6, 7, 10 },
                { -3, -5, -8, -11, 2, 4, 7, 10 },
                { -2, -6, -8, -10, 1, 5, 7, 9 },
                { -2, -5, -8, -10, 1, 4, 7, 9 },
                { -2, -4, -8, -10, 1, 3, 7, 9 },
                { -2, -5, -7, -10, 1, 4, 6, 9 },
                { -3, -4, -7, -10, 2, 3, 6, 9 },
                { -1, -2, -3, -10, 0, 1, 2, 9 },
                { -4, -6, -8, -9, 3, 5, 7, 8 },
                { -3, -5, -7, -9, 2, 4, 6, 8 },
                { -6, -12, -18, -30, 4, 10, 16, 28 },
                { -6, -14, -20, -26, 4, 12, 18, 24 },
                { -4, -10, -16, -26, 2, 8, 14, 24 },
                { -4, -8, -12, -26, 2, 6, 10, 24 },
                { -6, -12, -16, -24, 4, 10, 14, 22 },
                { -6, -14, -18, -22, 4, 12, 16, 20 },
                { -8, -14, -16, -22, 6, 12, 14, 20 },
                { -6, -10, -16, -22, 4, 8, 14, 20 },
                { -4, -12, -16, -20, 2, 10, 14, 18 },
                { -4, -10, -16, -20, 2, 8, 14, 18 },
                { -4, -8, -16, -20, 2, 6, 14, 18 },
                { -4, -10, -14, -20, 2, 8, 12, 18 },
                { -6, -8, -14, -20, 4, 6, 12, 18 },
                { -2, -4, -6, -20, 0, 2, 4, 18 },
                { -8, -12, -16, -18, 6, 10, 14, 16 },
                { -6, -10, -14, -18, 4, 8, 12, 16 },
                { -9, -18, -27, -45, 6, 15, 24, 42 },
                { -9, -21, -30, -39, 6, 18, 27, 36 },
                { -6, -15, -24, -39, 3, 12, 21, 36 },
                { -6, -12, -18, -39, 3, 9, 15, 36 },
                { -9, -18, -24, -36, 6, 15, 21, 33 },
                { -9, -21, -27, -33, 6, 18, 24, 30 },
                { -12, -21, -24, -33, 9, 18, 21, 30 },
                { -9, -15, -24, -33, 6, 12, 21, 30 },
                { -6, -18, -24, -30, 3, 15, 21, 27 },
                { -6, -15, -24, -30, 3, 12, 21, 27 },
                { -6, -12, -24, -30, 3, 9, 21, 27 },
                { -6, -15, -21, -30, 3, 12, 18, 27 },
                { -9, -12, -21, -30, 6, 9, 18, 27 },
                { -3, -6, -9, -30, 0, 3, 6, 27 },
                { -12, -18, -24, -27, 9, 15, 21, 24 },
                { -9, -15, -21, -27, 6, 12, 18, 24 },
                { -12, -24, -36, -60, 8, 20, 32, 56 },
                { -12, -28, -40, -52, 8, 24, 36, 48 },
                { -8, -20, -32, -52, 4, 16, 28, 48 },
                { -8, -16, -24, -52, 4, 12, 20, 48 },
                { -12, -24, -32, -48, 8, 20, 28, 44 },
                { -12, -28, -36, -44, 8, 24, 32, 40 },
                { -16, -28, -32, -44, 12, 24, 28, 40 },
                { -12, -20, -32, -44, 8, 16, 28, 40 },
                { -8, -24, -32, -40, 4, 20, 28, 36 },
                { -8, -20, -32, -40, 4, 16, 28, 36 },
                { -8, -16, -32, -40, 4, 12, 28, 36 },
                { -8, -20, -28, -40, 4, 16, 24, 36 },
                { -12, -16, -28, -40, 8, 12, 24, 36 },
                { -4, -8, -12, -40, 0, 4, 8, 36 },
                { -16, -24, -32, -36, 12, 20, 28, 32 },
                { -12, -20, -28, -36, 8, 16, 24, 32 },
                { -15, -30, -45, -75, 10, 25, 40, 70 },
                { -15, -35, -50, -65, 10, 30, 45, 60 },
                { -10, -25, -40, -65, 5, 20, 35, 60 },
                { -10, -20, -30, -65, 5, 15, 25, 60 },
                { -15, -30, -40, -60, 10, 25, 35, 55 },
                { -15, -35, -45, -55, 10, 30, 40, 50 },
                { -20, -35, -40, -55, 15, 30, 35, 50 },
                { -15, -25, -40, -55, 10, 20, 35, 50 },
                { -10, -30, -40, -50, 5, 25, 35, 45 },
                { -10, -25, -40, -50, 5, 20, 35, 45 },
                { -10, -20, -40, -50, 5, 15, 35, 45 },
                { -10, -25, -35, -50, 5, 20, 30, 45 },
                { -15, -20, -35, -50, 10, 15, 30, 45 },
                { -5, -10, -15, -50, 0, 5, 10, 45 },
                { -20, -30, -40, -45, 15, 25, 35, 40 },
                { -15, -25, -35, -45, 10, 20, 30, 40 },
                { -18, -36, -54, -90, 12, 30, 48, 84 },
                { -18, -42, -60, -78, 12, 36, 54, 72 },
                { -12, -30, -48, -78, 6, 24, 42, 72 },
                { -12, -24, -36, -78, 6, 18, 30, 72 },
                { -18, -36, -48, -72, 12, 30, 42, 66 },
                { -18, -42, -54, -66, 12, 36, 48, 60 },
                { -24, -42, -48, -66, 18, 36, 42, 60 },
                { -18, -30, -48, -66, 12, 24, 42, 60 },
                { -12, -36, -48, -60, 6, 30, 42, 54 },
                { -12, -30, -48, -60, 6, 24, 42, 54 },
                { -12, -24, -48, -60, 6, 18, 42, 54 },
                { -12, -30, -42, -60, 6, 24, 36, 54 },
                { -18, -24, -42, -60, 12, 18, 36, 54 },
                { -6, -12, -18, -60, 0, 6, 12, 54 },
                { -24, -36, -48, -54, 18, 30, 42, 48 },
                { -18, -30, -42, -54, 12, 24, 36, 48 },
                { -21, -42, -63, -105, 14, 35, 56, 98 },
                { -21, -49, -70, -91, 14, 42, 63, 84 },
                { -14, -35, -56, -91, 7, 28, 49, 84 },
                { -14, -28, -42, -91, 7, 21, 35, 84 },
                { -21, -42, -56, -84, 14, 35, 49, 77 },
                { -21, -49, -63, -77, 14, 42, 56, 70 },
                { -28, -49, -56, -77, 21, 42, 49, 70 },
                { -21, -35, -56, -77, 14, 28, 49, 70 },
                { -14, -42, -56, -70, 7, 35, 49, 63 },
                { -14, -35, -56, -70, 7, 28, 49, 63 },
                { -14, -28, -56, -70, 7, 21, 49, 63 },
                { -14, -35, -49, -70, 7, 28, 42, 63 },
                { -21, -28, -49, -70, 14, 21, 42, 63 },
                { -7, -14, -21, -70, 0, 7, 14, 63 },
                { -28, -42, -56, -63, 21, 35, 49, 56 },
                { -21, -35, -49, -63, 14, 28, 42, 56 },
                { -24, -48, -72, -120, 16, 40, 64, 112 },
                { -24, -56, -80, -104, 16, 48, 72, 96 },
                { -16, -40, -64, -104, 8, 32, 56, 96 },
                { -16, -32, -48, -104, 8, 24, 40, 96 },
                { -24, -48, -64, -96, 16, 40, 56, 88 },
                { -24, -56, -72, -88, 16, 48, 64, 80 },
                { -32, -56, -64, -88, 24, 48, 56, 80 },
                { -24, -40, -64, -88, 16, 32, 56, 80 },
                { -16, -48, -64, -80, 8, 40, 56, 72 },
                { -16, -40, -64, -80, 8, 32, 56, 72 },
                { -16, -32, -64, -80, 8, 24, 56, 72 },
                { -16, -40, -56, -80, 8, 32, 48, 72 },
                { -24, -32, -56, -80, 16, 24, 48, 72 },
                { -8, -16, -24, -80, 0, 8, 16, 72 },
                { -32, -48, -64, -72, 24, 40, 56, 64 },
                { -24, -40, -56, -72, 16, 32, 48, 64 },
                { -27, -54, -81, -135, 18, 45, 72, 126 },
                { -27, -63, -90, -117, 18, 54, 81, 108 },
                { -18, -45, -72, -117, 9, 36, 63, 108 },
                { -18, -36, -54, -117, 9, 27, 45, 108 },
                { -27, -54, -72, -108, 18, 45, 63, 99 },
                { -27, -63, -81, -99, 18, 54, 72, 90 },
                { -36, -63, -72, -99, 27, 54, 63, 90 },
                { -27, -45, -72, -99, 18, 36, 63, 90 },
                { -18, -54, -72, -90, 9, 45, 63, 81 },
                { -18, -45, -72, -90, 9, 36, 63, 81 },
                { -18, -36, -72, -90, 9, 27, 63, 81 },
                { -18, -45, -63, -90, 9, 36, 54, 81 },
                { -27, -36, -63, -90, 18, 27, 54, 81 },
                { -9, -18, -27, -90, 0, 9, 18, 81 },
                { -36, -54, -72, -81, 27, 45, 63, 72 },
                { -27, -45, -63, -81, 18, 36, 54, 72 },
                { -30, -60, -90, -150, 20, 50, 80, 140 },
                { -30, -70, -100, -130, 20, 60, 90, 120 },
                { -20, -50, -80, -130, 10, 40, 70, 120 },
                { -20, -40, -60, -130, 10, 30, 50, 120 },
                { -30, -60, -80, -120, 20, 50, 70, 110 },
                { -30, -70, -90, -110, 20, 60, 80, 100 },
                { -40, -70, -80, -110, 30, 60, 70, 100 },
                { -30, -50, -80, -110, 20, 40, 70, 100 },
                { -20, -60, -80, -100, 10, 50, 70, 90 },
                { -20, -50, -80, -100, 10, 40, 70, 90 },
                { -20, -40, -80, -100, 10, 30, 70, 90 },
                { -20, -50, -70, -100, 10, 40, 60, 90 },
                { -30, -40, -70, -100, 20, 30, 60, 90 },
                { -10, -20, -30, -100, 0, 10, 20, 90 },
                { -40, -60, -80, -90, 30, 50, 70, 80 },
                { -30, -50, -70, -90, 20, 40, 60, 80 },
                { -33, -66, -99, -165, 22, 55, 88, 154 },
                { -33, -77, -110, -143, 22, 66, 99, 132 },
                { -22, -55, -88, -143, 11, 44, 77, 132 },
                { -22, -44, -66, -143, 11, 33, 55, 132 },
                { -33, -66, -88, -132, 22, 55, 77, 121 },
                { -33, -77, -99, -121, 22, 66, 88, 110 },
                { -44, -77, -88, -121, 33, 66, 77, 110 },
                { -33, -55, -88, -121, 22, 44, 77, 110 },
                { -22, -66, -88, -110, 11, 55, 77, 99 },
                { -22, -55, -88, -110, 11, 44, 77, 99 },
                { -22, -44, -88, -110, 11, 33, 77, 99 },
                { -22, -55, -77, -110, 11, 44, 66, 99 },
                { -33, -44, -77, -110, 22, 33, 66, 99 },
                { -11, -22, -33, -110, 0, 11, 22, 99 },
                { -44, -66, -88, -99, 33, 55, 77, 88 },
                { -33, -55, -77, -99, 22, 44, 66, 88 },
                { -36, -72, -108, -180, 24, 60, 96, 168 },
                { -36, -84, -120, -156, 24, 72, 108, 144 },
                { -24, -60, -96, -156, 12, 48, 84, 144 },
                { -24, -48, -72, -156, 12, 36, 60, 144 },
                { -36, -72, -96, -144, 24, 60, 84, 132 },
                { -36, -84, -108, -132, 24, 72, 96, 120 },
                { -48, -84, -96, -132, 36, 72, 84, 120 },
                { -36, -60, -96, -132, 24, 48, 84, 120 },
                { -24, -72, -96, -120, 12, 60, 84, 108 },
                { -24, -60, -96, -120, 12, 48, 84, 108 },
                { -24, -48, -96, -120, 12, 36, 84, 108 },
                { -24, -60, -84, -120, 12, 48, 72, 108 },
                { -36, -48, -84, -120, 24, 36, 72, 108 },
                { -12, -24, -36, -120, 0, 12, 24, 108 },
                { -48, -72, -96, -108, 36, 60, 84, 96 },
                { -36, -60, -84, -108, 24, 48, 72, 96 },
                { -39, -78, -117, -195, 26, 65, 104, 182 },
                { -39, -91, -130, -169, 26, 78, 117, 156 },
                { -26, -65, -104, -169, 13, 52, 91, 156 },
                { -26, -52, -78, -169, 13, 39, 65, 156 },
                { -39, -78, -104, -156, 26, 65, 91, 143 },
                { -39, -91, -117, -143, 26, 78, 104, 130 },
                { -52, -91, -104, -143, 39, 78, 91, 130 },
                { -39, -65, -104, -143, 26, 52, 91, 130 },
                { -26, -78, -104, -130, 13, 65, 91, 117 },
                { -26, -65, -104, -130, 13, 52, 91, 117 },
                { -26, -52, -104, -130, 13, 39, 91, 117 },
                { -26, -65, -91, -130, 13, 52, 78, 117 },
                { -39, -52, -91, -130, 26, 39, 78, 117 },
                { -13, -26, -39, -130, 0, 13, 26, 117 },
                { -52, -78, -104, -117, 39, 65, 91, 104 },
                { -39, -65, -91, -117, 26, 52, 78, 104 },
                { -42, -84, -126, -210, 28, 70, 112, 196 },
                { -42, -98, -140, -182, 28, 84, 126, 168 },
                { -28, -70, -112, -182, 14, 56, 98, 168 },
                { -28, -56, -84, -182, 14, 42, 70, 168 },
                { -42, -84, -112, -168, 28, 70, 98, 154 },
                { -42, -98, -126, -154, 28, 84, 112, 140 },
                { -56, -98, -112, -154, 42, 84, 98, 140 },
                { -42, -70, -112, -154, 28, 56, 98, 140 },
                { -28, -84, -112, -140, 14, 70, 98, 126 },
                { -28, -70, -112, -140, 14, 56, 98, 126 },
                { -28, -56, -112, -140, 14, 42, 98, 126 },
                { -28, -70, -98, -140, 14, 56, 84, 126 },
                { -42, -56, -98, -140, 28, 42, 84, 126 },
                { -14, -28, -42, -140, 0, 14, 28, 126 },
                { -56, -84, -112, -126, 42, 70, 98, 112 },
                { -42, -70, -98, -126, 28, 56, 84, 112 },
                { -45, -90, -135, -225, 30, 75, 120, 210 },
                { -45, -105, -150, -195, 30, 90, 135, 180 },
                { -30, -75, -120, -195, 15, 60, 105, 180 },
                { -30, -60, -90, -195, 15, 45, 75, 180 },
                { -45, -90, -120, -180, 30, 75, 105, 165 },
                { -45, -105, -135, -165, 30, 90, 120, 150 },
                { -60, -105, -120, -165, 45, 90, 105, 150 },
                { -45, -75, -120, -165, 30, 60, 105, 150 },
                { -30, -90, -120, -150, 15, 75, 105, 135 },
                { -30, -75, -120, -150, 15, 60, 105, 135 },
                { -30, -60, -120, -150, 15, 45, 105, 135 },
                { -30, -75, -105, -150, 15, 60, 90, 135 },
                { -45, -60, -105, -150, 30, 45, 90, 135 },
                { -15, -30, -45, -150, 0, 15, 30, 135 },
                { -60, -90, -120, -135, 45, 75, 105, 120 },
                { -45, -75, -105, -135, 30, 60, 90, 120 },
            };
            static byte[] table59T = new byte[8] { 3, 6, 11, 16, 23, 32, 41, 64 };
            static byte[] table58H = new byte[8] { 3, 6, 11, 16, 23, 32, 41, 64 };
            static int[] unscramble = new int[4] { 2, 3, 1, 0 };
            static int[,] compressParams = new int[16, 4]
            {
                { -8, -2, 2, 8 },
                { -8, -2, 2, 8 },
                { -17, -5, 5, 17 },
                { -17, -5, 5, 17 },
                { -29, -9, 9, 29 },
                { -29, -9, 9, 29 },
                { -42, -13, 13, 42 },
                { -42, -13, 13, 42 },
                { -60, -18, 18, 60 },
                { -60, -18, 18, 60 },
                { -80, -24, 24, 80 },
                { -80, -24, 24, 80 },
                { -106, -33, 33, 106 },
                { -106, -33, 33, 106 },
                { -183, -47, 47, 183 },
                { -183, -47, 47, 183 }
            };

            public static void DecodeBlock_RGB_ETC1(byte* texPtr, YFColor* color)
            {
                uint word1 = (uint)((texPtr[0] << 24) | (texPtr[1] << 16) | (texPtr[2] << 8) | texPtr[3]);
                uint word2 = (uint)((texPtr[4] << 24) | (texPtr[5] << 16) | (texPtr[6] << 8) | texPtr[7]);
                decompressBlockDiffFlipC(word1, word2, color);
                for (int i = 0; i < 16; i++)
                {
                    (color++)->Alpha = 0xFF;
                }
            }

            public static void DecodeBlock_RGB_ETC2(byte* texPtr, YFColor* color)
            {
                uint word1 = (uint)((texPtr[0] << 24) | (texPtr[1] << 16) | (texPtr[2] << 8) | texPtr[3]);
                uint word2 = (uint)((texPtr[4] << 24) | (texPtr[5] << 16) | (texPtr[6] << 8) | texPtr[7]);
                decompressBlockETC2c(word1, word2, color);
                for (int i = 0; i < 16; i++)
                {
                    (color++)->Alpha = 0xFF;
                }
            }

            public static void DecodeBlock_RGB_A1_ETC2(byte* texPtr, YFColor* color)
            {
                uint word1 = (uint)((texPtr[0] << 24) | (texPtr[1] << 16) | (texPtr[2] << 8) | texPtr[3]);
                uint word2 = (uint)((texPtr[4] << 24) | (texPtr[5] << 16) | (texPtr[6] << 8) | texPtr[7]);
                decompressBlockETC21BitAlphaC(word1, word2, color);
            }

            public static void DecodeBlock_RGBA_ETC2_EAC(byte* texPtr, YFColor* color)
            {
                decompressBlockAlphaC(texPtr, color);
                texPtr += 8;
                uint word1 = (uint)((texPtr[0] << 24) | (texPtr[1] << 16) | (texPtr[2] << 8) | texPtr[3]);
                uint word2 = (uint)((texPtr[4] << 24) | (texPtr[5] << 16) | (texPtr[6] << 8) | texPtr[7]);
                decompressBlockETC2c(word1, word2, color);
            }
        }

        private static class ETC1Encoder
        {
            public static readonly int[,] ETC1Modifiers =
            {
                { 2, 8 },
                { 5, 17 },
                { 9, 29 },
                { 13, 42 },
                { 18, 60 },
                { 24, 80 },
                { 33, 106 },
                { 47, 183 }
            };

            public static ulong GenETC1(YFColor* Colors)
            {
                ulong Horizontal = GenHorizontal(Colors);
                ulong Vertical = GenVertical(Colors);
                YFColor* sc = stackalloc YFColor[16];
                DecodeETC1(Horizontal, sc);
                int HorizontalScore = GetScore(Colors, sc);
                DecodeETC1(Vertical, sc);
                int VerticalScore = GetScore(Colors, sc);
                return (HorizontalScore < VerticalScore) ? Horizontal : Vertical;
            }

            public static void DecodeETC1(ulong temp, YFColor* Result)
            {
                bool diffbit = ((temp >> 33) & 1) == 1;
                bool flipbit = ((temp >> 32) & 1) == 1;
                int r1, r2, g1, g2, b1, b2;
                if (diffbit)
                {
                    int r = (int)((temp >> 59) & 0x1F);
                    int g = (int)((temp >> 51) & 0x1F);
                    int b = (int)((temp >> 43) & 0x1F);
                    r1 = (r << 3) | ((r & 0x1C) >> 2);
                    g1 = (g << 3) | ((g & 0x1C) >> 2);
                    b1 = (b << 3) | ((b & 0x1C) >> 2);
                    r += (int)((temp >> 56) & 0x7) << 29 >> 29;
                    g += (int)((temp >> 48) & 0x7) << 29 >> 29;
                    b += (int)((temp >> 40) & 0x7) << 29 >> 29;
                    r2 = (r << 3) | ((r & 0x1C) >> 2);
                    g2 = (g << 3) | ((g & 0x1C) >> 2);
                    b2 = (b << 3) | ((b & 0x1C) >> 2);
                }
                else
                {
                    r1 = (int)((temp >> 60) & 0xF) * 0x11;
                    g1 = (int)((temp >> 52) & 0xF) * 0x11;
                    b1 = (int)((temp >> 44) & 0xF) * 0x11;
                    r2 = (int)((temp >> 56) & 0xF) * 0x11;
                    g2 = (int)((temp >> 48) & 0xF) * 0x11;
                    b2 = (int)((temp >> 40) & 0xF) * 0x11;
                }
                int Table1 = (int)((temp >> 37) & 0x7);
                int Table2 = (int)((temp >> 34) & 0x7);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int val = (int)((temp >> ((j << 2) | i)) & 0x1);
                        bool neg = ((temp >> (((j << 2) | i) + 16)) & 0x1) == 1;
                        if ((flipbit && i < 2) || (!flipbit && j < 2))
                        {
                            int add = ETC1Modifiers[Table1, val] * (neg ? -1 : 1);
                            Result[(i << 2) | j] = new YFColor(ColorClamp(r1 + add), ColorClamp(g1 + add), ColorClamp(b1 + add));
                        }
                        else
                        {
                            int add = ETC1Modifiers[Table2, val] * (neg ? -1 : 1);
                            Result[(i << 2) | j] = new YFColor(ColorClamp(r2 + add), ColorClamp(g2 + add), ColorClamp(b2 + add));
                        }
                    }
                }
            }

            private static int GetScore(YFColor* Original, YFColor* Encode)
            {
                int Diff = 0;
                for (int i = 0; i < 4 * 4; i++)
                {
                    Diff += Math.Abs(Encode[i].Red - Original[i].Red);
                    Diff += Math.Abs(Encode[i].Green - Original[i].Green);
                    Diff += Math.Abs(Encode[i].Blue - Original[i].Blue);
                }
                return Diff;
            }

            private static void SetFlipMode(ref ulong Data, bool Mode)
            {
                Data &= ~(1ul << 32);
                Data |= (Mode ? 1ul : 0ul) << 32;
            }

            private static void SetDiffMode(ref ulong Data, bool Mode)
            {
                Data &= ~(1ul << 33);
                Data |= (Mode ? 1ul : 0ul) << 33;
            }

            private static YFColor[] GetLeftColors(YFColor* Pixels)
            {
                YFColor[] Left = new YFColor[8];
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        Left[y * 2 + x] = Pixels[y * 4 + x];
                    }
                }
                return Left;
            }

            private static YFColor[] GetRightColors(YFColor* Pixels)
            {
                YFColor[] Right = new YFColor[8];
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 2; x < 4; x++)
                    {
                        Right[y * 2 + x - 2] = Pixels[y * 4 + x];
                    }
                }
                return Right;
            }

            private static YFColor[] GetTopColors(YFColor* Pixels)
            {
                YFColor[] Top = new YFColor[8];
                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        Top[y * 4 + x] = Pixels[y * 4 + x];
                    }
                }
                return Top;
            }

            private static YFColor[] GetBottomColors(YFColor* Pixels)
            {
                YFColor[] Bottom = new YFColor[8];
                for (int y = 2; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        Bottom[(y - 2) * 4 + x] = Pixels[y * 4 + x];
                    }
                }
                return Bottom;
            }

            private static ulong GenHorizontal(YFColor* Colors)
            {
                ulong data = 0;
                SetFlipMode(ref data, false);
                //Left
                YFColor[] Left = GetLeftColors(Colors);
                YFColor basec1;
                int mod = GenModifier(out basec1, Left);
                SetTable1(ref data, mod);
                GenPixDiff(ref data, Left, basec1, mod, 0, 2, 0, 4);
                //Right
                YFColor[] Right = GetRightColors(Colors);
                YFColor basec2;
                mod = GenModifier(out basec2, Right);
                SetTable2(ref data, mod);
                GenPixDiff(ref data, Right, basec2, mod, 2, 4, 0, 4);
                SetBaseColors(ref data, basec1, basec2);
                return data;
            }

            private static ulong GenVertical(YFColor* Colors)
            {
                ulong data = 0;
                SetFlipMode(ref data, true);
                //Top
                YFColor[] Top = GetTopColors(Colors);
                YFColor basec1;
                int mod = GenModifier(out basec1, Top);
                SetTable1(ref data, mod);
                GenPixDiff(ref data, Top, basec1, mod, 0, 4, 0, 2);
                //Bottom
                YFColor[] Bottom = GetBottomColors(Colors);
                YFColor basec2;
                mod = GenModifier(out basec2, Bottom);
                SetTable2(ref data, mod);
                GenPixDiff(ref data, Bottom, basec2, mod, 0, 4, 2, 4);
                SetBaseColors(ref data, basec1, basec2);
                return data;
            }

            private static void SetBaseColors(ref ulong Data, YFColor Color1, YFColor Color2)
            {
                int R1 = Color1.Red;
                int G1 = Color1.Green;
                int B1 = Color1.Blue;
                int R2 = Color2.Red;
                int G2 = Color2.Green;
                int B2 = Color2.Blue;
                //First look if differencial is possible.
                int RDiff = (R2 - R1) / 8;
                int GDiff = (G2 - G1) / 8;
                int BDiff = (B2 - B1) / 8;
                if (RDiff > -4 && RDiff < 3 && GDiff > -4 && GDiff < 3 && BDiff > -4 && BDiff < 3)
                {
                    SetDiffMode(ref Data, true);
                    R1 /= 8;
                    G1 /= 8;
                    B1 /= 8;
                    Data |= (ulong)R1 << 59;
                    Data |= (ulong)G1 << 51;
                    Data |= (ulong)B1 << 43;
                    Data |= (ulong)(RDiff & 0x7) << 56;
                    Data |= (ulong)(GDiff & 0x7) << 48;
                    Data |= (ulong)(BDiff & 0x7) << 40;
                }
                else
                {
                    Data |= (ulong)(R1 / 0x11) << 60;
                    Data |= (ulong)(G1 / 0x11) << 52;
                    Data |= (ulong)(B1 / 0x11) << 44;

                    Data |= (ulong)(R2 / 0x11) << 56;
                    Data |= (ulong)(G2 / 0x11) << 48;
                    Data |= (ulong)(B2 / 0x11) << 40;
                }
            }

            private static void GenPixDiff(ref ulong Data, YFColor[] Pixels, YFColor BaseColor, int Modifier, int XOffs, int XEnd, int YOffs, int YEnd)
            {
                int BaseMean = (BaseColor.Red + BaseColor.Green + BaseColor.Blue) / 3;
                int i = 0;
                for (int yy = YOffs; yy < YEnd; yy++)
                {
                    for (int xx = XOffs; xx < XEnd; xx++)
                    {
                        int Diff = ((Pixels[i].Red + Pixels[i].Green + Pixels[i].Blue) / 3) - BaseMean;

                        if (Diff < 0) Data |= 1ul << (xx * 4 + yy + 16);
                        int tbldiff1 = Math.Abs(Diff) - ETC1Modifiers[Modifier, 0];
                        int tbldiff2 = Math.Abs(Diff) - ETC1Modifiers[Modifier, 1];

                        if (Math.Abs(tbldiff2) < Math.Abs(tbldiff1)) Data |= 1ul << (xx * 4 + yy);
                        i++;
                    }
                }
            }

            private static int GenModifier(out YFColor BaseColor, YFColor[] Pixels)
            {
                YFColor Max = new YFColor(255, 255, 255);
                YFColor Min = new YFColor(0, 0, 0);
                int MinY = int.MaxValue;
                int MaxY = int.MinValue;
                for (int i = 0; i < 8; i++)
                {
                    if (Pixels[i].Alpha == 0) continue;
                    int Y = (Pixels[i].Red + Pixels[i].Green + Pixels[i].Blue) / 3;
                    if (Y > MaxY)
                    {
                        MaxY = Y;
                        Max = Pixels[i];
                    }
                    if (Y < MinY)
                    {
                        MinY = Y;
                        Min = Pixels[i];
                    }
                }
                int DiffMean = (Max.Red - Min.Red + Max.Green - Min.Green + Max.Blue - Min.Blue) / 3;
                int ModDiff = int.MaxValue;
                int Modifier = -1;
                int Mode = -1;
                for (int i = 0; i < 8; i++)
                {
                    int SS = ETC1Modifiers[i, 0] * 2;
                    int SB = ETC1Modifiers[i, 0] + ETC1Modifiers[i, 1];
                    int BB = ETC1Modifiers[i, 1] * 2;
                    if (SS > 255) SS = 255;
                    if (SB > 255) SB = 255;
                    if (BB > 255) BB = 255;
                    if (Math.Abs(DiffMean - SS) < ModDiff)
                    {
                        ModDiff = Math.Abs(DiffMean - SS);
                        Modifier = i;
                        Mode = 0;
                    }
                    if (Math.Abs(DiffMean - SB) < ModDiff)
                    {
                        ModDiff = Math.Abs(DiffMean - SB);
                        Modifier = i;
                        Mode = 1;
                    }
                    if (Math.Abs(DiffMean - BB) < ModDiff)
                    {
                        ModDiff = Math.Abs(DiffMean - BB);
                        Modifier = i;
                        Mode = 2;
                    }
                }
                if (Mode == 1)
                {
                    float div1 = ETC1Modifiers[Modifier, 0] / (float)ETC1Modifiers[Modifier, 1];
                    float div2 = 1f - div1;
                    BaseColor = new YFColor(ColorClamp(Min.Red * div1 + Max.Red * div2), ColorClamp(Min.Green * div1 + Max.Green * div2), ColorClamp(Min.Blue * div1 + Max.Blue * div2));
                }
                else
                {
                    BaseColor = new YFColor((byte)((Min.Red + Max.Red) >> 1), (byte)((Min.Green + Max.Green) >> 1), (byte)((Min.Blue + Max.Blue) >> 1));
                }
                return Modifier;
            }

            private static void SetTable1(ref ulong Data, int Table)
            {
                Data &= ~(7ul << 37);
                Data |= (ulong)(Table & 0x7) << 37;
            }

            private static void SetTable2(ref ulong Data, int Table)
            {
                Data &= ~(7ul << 34);
                Data |= (ulong)(Table & 0x7) << 34;
            }

            public static byte ColorClamp(float Color) //加颜色可能加出来超过结果的
            {
                int color = (int)Color;
                if (color > 255) return 255;
                if (color < 0) return 0;
                return (byte)color;
            }

            public static byte ColorClamp(int Color) //加颜色可能加出来超过结果的
            {
                if (Color > 255) return 255;
                if (Color < 0) return 0;
                return (byte)Color;
            }
        }
    }
}
