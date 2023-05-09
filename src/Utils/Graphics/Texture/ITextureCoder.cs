using LibWindPop.Utils.Graphics.Bitmap;
using System;

namespace LibWindPop.Utils.Graphics.Texture
{
    /// <summary>
    /// Describe a texture decoder/encoder
    /// </summary>
    public interface ITextureCoder
    {
        /// <summary>
        /// Decode Texture Data as BGRA8888 format
        /// </summary>
        /// <param name="src_data">Texture Data</param>
        /// <param name="width">Texture Width</param>
        /// <param name="height">Texture Height</param>
        /// <param name="dst_bitmap">Bitmap to save BGRA8888 data</param>
        void Decode(ReadOnlySpan<byte> src_data, int width, int height, RefBitmap dst_bitmap);

        /// <summary>
        /// Encode BGRA8888 data as Texture format
        /// </summary>
        /// <param name="src_bitmap">Bitmap with BGRA8888 data</param>
        /// <param name="dst_data">Texture Data</param>
        /// <param name="width">Texture Width</param>
        /// <param name="height">Texture Height</param>
        void Encode(RefBitmap src_bitmap, Span<byte> dst_data, int width, int height);
    }
}
