using System;
using System.Runtime.CompilerServices;

namespace LibWindPop.Utils.Graphics.Bitmap
{
    public readonly ref struct RefBitmap
    {
        public readonly int Width;
        public readonly int Height;
        public readonly int Area;
        public readonly Span<YFColor> Data;

        public Span<YFColor> this[int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Data.Slice(y * Width, Width);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                int data_len = Math.Min(Width, value.Length);
                value[..data_len].CopyTo(Data.Slice(y * Width, data_len));
            }
        }

        public ref YFColor this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Data[y * Width + x];
            }
        }

        internal RefBitmap(int width, int height, Span<YFColor> data)
        {
            Width = width;
            Height = height;
            Area = width * height;
            Data = data;
        }

        public Span<YFColor> AsSpan()
        {
            return Data;
        }
    }
}
