using System;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public struct MemoryBitmap : IBitmap
    {
        public int Width => width;
        public int Height => height;
        public int Area => area;

        public Span<YFColor> AsSpan()
        {
            return data_ptr.Span;
        }

        public RefBitmap AsRefBitmap()
        {
            return new RefBitmap(width, height, AsSpan());
        }

        private readonly int width;
        private readonly int height;
        private readonly int area;
        private Memory<YFColor> data_ptr;

        public MemoryBitmap(int width, int height, Memory<YFColor> data_ptr)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            this.width = width;
            this.height = height;
            area = width * height;
            this.data_ptr = data_ptr[..area];
        }
    }
}
