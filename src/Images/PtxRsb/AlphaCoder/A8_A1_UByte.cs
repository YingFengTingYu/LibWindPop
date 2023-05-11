using LibWindPop.Utils;
using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Graphics.Bitmap;
using System;

namespace LibWindPop.Images.PtxRsb.AlphaCoder
{
    public static unsafe class A8_A1_UByte
    {
        public static void Decode(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            ReadOnlySpan<byte> alpha_data = src_data[1..];
            if (src_data[0] == 0)
            {
                DecodeAlpha1Data(alpha_data, width, height, dst_bitmap);
            }
            else
            {
                DecodeAlpha8Data(alpha_data, width, height, dst_bitmap);
            }
        }

        private static void DecodeAlpha8Data(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap)
        {
            int temp_data_ptr = 0;
            Span<YFColor> row;
            for (int y = 0; y < height; y++)
            {
                row = dst_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    row[x].Alpha = src_data[temp_data_ptr++];
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
            Span<byte> alpha_index_data = dst_data[1..];
            if (PeekUseA1(src_bitmap, width, height))
            {
                dst_data[0] = 0;
                EncodeAlpha1Data(src_bitmap, alpha_index_data, width, height);
            }
            else
            {
                dst_data[0] = 1;
                EncodeAlpha8Data(src_bitmap, alpha_index_data, width, height);
            }
        }

        private static void EncodeAlpha8Data(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height)
        {
            int temp_data_ptr = 0;
            Span<YFColor> row;
            for (int y = 0; y < height; y++)
            {
                row = src_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    dst_data[temp_data_ptr++] = row[x].Alpha;
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

        public static int PeekAlphaSize(RefBitmap src_bitmap, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            if (PeekUseA1(src_bitmap, width, height))
            {
                return 1 + ((width * height + 7) >> 3);
            }
            return 1 + width * height;
        }

        private static bool PeekUseA1(RefBitmap src_bitmap, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            Span<YFColor> row;
            for (int y = 0; y < height; y++)
            {
                row = src_bitmap[y];
                for (int x = 0; x < width; x++)
                {
                    if (row[x].Alpha != 0x0 && row[x].Alpha != 0xFF)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
