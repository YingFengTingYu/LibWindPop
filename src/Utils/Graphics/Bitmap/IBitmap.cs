using System;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public interface IBitmap
    {
        int Width { get; }
        int Height { get; }
        int Area { get; }

        Span<YFColor> AsSpan();

        RefBitmap AsRefBitmap();
    }
}
