using LibWindPop.Utils.Graphics.Bitmap;
using System;

namespace LibWindPop.Utils.Graphics.Texture
{
    public static class TextureCoder
    {
        public static void Decode<TCoder>(ReadOnlySpan<byte> src_data, int width, int height, int pitch, RefBitmap dst_bitmap)
            where TCoder : IPitchableTextureCoder, new()
        {
            new TCoder().Decode(src_data, width, height, pitch, dst_bitmap);
        }

        public static void Decode<TCoder>(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap)
            where TCoder : ITextureCoder, new()
        {
            new TCoder().Decode(src_data, width, height, dst_bitmap);
        }

        public static void Encode<TCoder>(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height, int pitch)
            where TCoder : IPitchableTextureCoder, new()
        {
            new TCoder().Encode(src_bitmap, dst_data, width, height, pitch);
        }

        public static void Encode<TCoder>(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height)
            where TCoder : ITextureCoder, new()
        {
            new TCoder().Encode(src_bitmap, dst_data, width, height);
        }
    }
}
