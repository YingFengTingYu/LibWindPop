using LibWindPop.Utils;
using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Graphics.Bitmap;
using System;

namespace LibWindPop.Images.PtxRsb.AlphaCoder
{
    public static unsafe class A4_Palette_A1_UByte
    {
        public static void Decode(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            int palette_count = src_data[0];
            ReadOnlySpan<byte> alpha_index_data = src_data[(1 + palette_count)..];
            if (palette_count == 0)
            {
                DecodeAlpha1Data(alpha_index_data, width, height, dst_bitmap);
            }
            else
            {
                Span<byte> palette = stackalloc byte[palette_count];
                for (int i = 0; i < palette_count; i++)
                {
                    palette[i] = (byte)BitHelper.FourBitToEightBit(src_data[i + 1]); // In game: 16 * alpha + 1
                }
                DecodeAlpha4PaletteData(alpha_index_data, palette, width, height, dst_bitmap);
            }
        }

        private static void DecodeAlpha4PaletteData(ReadOnlySpan<byte> src_data, ReadOnlySpan<byte> alpha8_palette, int width, int height, RefBitmap dst_bitmap)
        {
            int bit_depth = GetAlphaBit(alpha8_palette.Length);
            int src_index = 0;
            int bit_mask = (1 << bit_depth) - 1;
            int bit_counter = bit_depth;
            int index;
            byte last_byte = 0;
            Span<YFColor> row;
            if (bit_depth == 0)
            {
                for (int y = 0; y < height; y++)
                {
                    row = dst_bitmap[y];
                    for (int x = 0; x < width; x++)
                    {
                        row[x].Alpha = alpha8_palette[0];
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    row = dst_bitmap[y];
                    for (int x = 0; x < width; x++)
                    {
                        index = 0;
                        if (bit_counter != 0) // avoid array index out of range
                        {
                            index |= (src_data[src_index] >> (8 - bit_counter)) & bit_mask;
                        }
                        if (bit_counter < bit_depth)
                        {
                            index |= (last_byte << bit_counter) & bit_mask;
                        }
                        row[x].Alpha = alpha8_palette[index];
                        bit_counter += bit_depth;
                        if (bit_counter >= 8)
                        {
                            bit_counter -= 8; // set zero
                            last_byte = src_data[src_index++];
                        }
                    }
                }
            }
        }

        private static void DecodeAlpha1Data(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap)
        {
            int src_index = 0;
            int bit_counter = 0;
            Span<YFColor> row;
            for (int y = 0; y < height; y++)
            {
                row = dst_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    row[x].Alpha = (byte)BitHelper.OneBitToEightBit(src_data[src_index] >> (7 - bit_counter));
                    bit_counter++;
                    if (bit_counter >= 8)
                    {
                        bit_counter -= 8;
                        src_index++;
                    }
                }
            }
        }

        public static void Encode(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            Span<byte> palette = stackalloc byte[16];
            PeekAlphaPalette(src_bitmap, width, height, out int paletteCount, palette);
            dst_data[0] = (byte)paletteCount;
            for (int i = 0; i < paletteCount; i++)
            {
                dst_data[i + 1] = palette[i];
            }
            Span<byte> alpha_index_data = dst_data[(1 + paletteCount)..];
            if (paletteCount == 0)
            {
                EncodeAlpha1Data(src_bitmap, alpha_index_data, width, height);
            }
            else
            {
                EncodeAlpha4PaletteData(src_bitmap, alpha_index_data, palette[..paletteCount], width, height);
            }
        }

        private static void EncodeAlpha4PaletteData(RefBitmap src_bitmap, Span<byte> dst_data, ReadOnlySpan<byte> alpha8_palette, int width, int height)
        {
            dst_data.Clear();
            int bit_depth = GetAlphaBit(alpha8_palette.Length);
            if (bit_depth != 0)
            {
                int src_index = 0;
                int lastIndex = -1;
                int bit_mask = (1 << bit_depth) - 1;
                int bit_counter = bit_depth;
                int index;
                Span<byte> get_index_palette = stackalloc byte[16];
                for (int i = 0; i < alpha8_palette.Length; i++)
                {
                    get_index_palette[alpha8_palette[i]] = (byte)i;
                }
                Span<YFColor> row;
                for (int y = 0; y < height; y++)
                {
                    row = src_bitmap[y];
                    for (int x = 0; x < width; x++)
                    {
                        index = get_index_palette[BitHelper.EightBitToFourBit(row[x].Alpha)] & bit_mask;
                        if (bit_counter < bit_depth)
                        {
                            dst_data[lastIndex] |= (byte)(index >> bit_counter);
                        }
                        if (bit_counter != 0)
                        {
                            dst_data[src_index] |= (byte)(index << (8 - bit_counter));
                        }
                        bit_counter += bit_depth;
                        if (bit_counter >= 8)
                        {
                            bit_counter -= 8;
                            lastIndex = src_index++;
                        }
                    }
                }
            }
        }

        private static void EncodeAlpha1Data(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height)
        {
            int src_index = 0;
            int bit_counter = 0;
            Span<YFColor> row;
            dst_data.Clear();
            for (int y = 0; y < height; y++)
            {
                row = src_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    if (BitHelper.EightBitToOneBit(row[x].Alpha) != 0)
                    {
                        dst_data[src_index] |= (byte)(1 << (7 - bit_counter));
                    }
                    bit_counter++;
                    if (bit_counter >= 8)
                    {
                        bit_counter -= 8;
                        src_index++;
                    }
                }
            }
        }

        private static int GetAlphaBit(int palette_count)
        {
            if (palette_count == 0)
            {
                return 1;
            }
            int index = 1;
            int bits = 0;
            while (index < palette_count)
            {
                index <<= 1;
                bits++;
            }
            return bits;
        }

        public static int PeekAlphaSize(RefBitmap src_bitmap, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            Span<bool> palette_buffer = stackalloc bool[16];
            Span<YFColor> row;
            for (int y = 0; y < height; y++)
            {
                row = src_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    palette_buffer[BitHelper.EightBitToFourBit(row[x].Alpha)] = true;
                }
            }
            // get palette count
            int palette_count = 0;
            for (int i = 0; i < 16; i++)
            {
                if (palette_buffer[i])
                {
                    palette_count++;
                }
            }
            if ((palette_count == 1 && (palette_buffer[0] || palette_buffer[15])) || (palette_count == 2 && palette_buffer[0] && palette_buffer[15]))
            {
                palette_count = 0;
            }
            return 1 + palette_count + ((width * height * GetAlphaBit(palette_count) + 7) >> 3);
        }

        private static void PeekAlphaPalette(RefBitmap src_bitmap, int width, int height, out int palette_count, Span<byte> palette)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            Span<bool> palette_buffer = stackalloc bool[16];
            Span<YFColor> row;
            palette_count = 0;
            for (int y = 0; y < height; y++)
            {
                row = src_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    int alpha_in_palette_value = BitHelper.EightBitToFourBit(row[x].Alpha);
                    if (!palette_buffer[alpha_in_palette_value])
                    {
                        palette_buffer[alpha_in_palette_value] = true;
                        palette[palette_count++] = (byte)alpha_in_palette_value;
                    }
                }
            }
            if ((palette_count == 1 && (palette_buffer[0] || palette_buffer[15])) || (palette_count == 2 && palette_buffer[0] && palette_buffer[15]))
            {
                palette_count = 0;
            }
        }
    }
}
