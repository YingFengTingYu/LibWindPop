using System;
using System.Runtime.InteropServices;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public unsafe class NativeBitmap : IDisposableBitmap
    {
        public int Width => width;
        public int Height => height;
        public int Area => area;

        public Span<YFColor> AsSpan()
        {
            return new Span<YFColor>(data_ptr, area);
        }

        public RefBitmap AsRefBitmap()
        {
            return new RefBitmap(width, height, AsSpan());
        }

        private readonly int width;
        private readonly int height;
        private readonly int area;
        private YFColor* data_ptr;
        private bool disposedValue;

        public NativeBitmap(int width, int height)
        {
            this.width = width;
            this.height = height;
            area = width * height;
            data_ptr = (YFColor*)NativeMemory.Alloc((nuint)(area * sizeof(YFColor)));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                NativeMemory.Free(data_ptr);
                data_ptr = null;
                disposedValue = true;
            }
        }

        ~NativeBitmap()
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
