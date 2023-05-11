using LibWindPop.Utils;
using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Graphics.Bitmap;
using System;

namespace LibWindPop.Images.PtxRsb.AlphaCoder
{
    public static unsafe class A8_UByte
    {
        public static void Decode(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
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

        public static void Encode(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
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
    }
}
