using System;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public struct ArraySegmentBitmap : IBitmap
    {
        public int Width => width;
        public int Height => height;
        public int Area => area;

        public Span<YFColor> AsSpan()
        {
            return data_ptr.AsSpan();
        }

        public RefBitmap AsRefBitmap()
        {
            return new RefBitmap(width, height, AsSpan());
        }

        private readonly int width;
        private readonly int height;
        private readonly int area;
        private ArraySegment<YFColor> data_ptr;

        public ArraySegmentBitmap(int width, int height, YFColor[] data_ptr)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            this.width = width;
            this.height = height;
            area = width * height;
            this.data_ptr = new ArraySegment<YFColor>(data_ptr, 0, area);
        }

        public ArraySegmentBitmap(int width, int height, ArraySegment<YFColor> data_ptr)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            this.width = width;
            this.height = height;
            area = width * height;
            this.data_ptr = data_ptr[..area];
        }
    }
}
