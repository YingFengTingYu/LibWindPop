using System;
using System.Buffers;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public class ArrayPoolBitmap : IDisposableBitmap
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
        private YFColor[] data_owner;
        private ArraySegment<YFColor> data_ptr;
        private bool disposedValue;

        public ArrayPoolBitmap(int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            this.width = width;
            this.height = height;
            area = width * height;
            data_owner = ArrayPool<YFColor>.Shared.Rent(area);
            data_ptr = new ArraySegment<YFColor>(data_owner, 0, area);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ArrayPool<YFColor>.Shared.Return(data_owner);
                disposedValue = true;
            }
        }

        ~ArrayPoolBitmap()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
