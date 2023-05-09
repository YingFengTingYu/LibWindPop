using System;
using System.Buffers;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public class MemoryPoolBitmap : IDisposableBitmap
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
        private IMemoryOwner<YFColor> data_owner;
        private Memory<YFColor> data_ptr;

        public MemoryPoolBitmap(int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            this.width = width;
            this.height = height;
            area = width * height;
            data_owner = MemoryPool<YFColor>.Shared.Rent(area);
            data_ptr = data_owner.Memory[..area];
        }

        public void Dispose()
        {
            data_owner.Dispose();
        }
    }
}
