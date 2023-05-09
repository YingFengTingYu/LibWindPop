using System.Runtime.InteropServices;

namespace LibWindPop.Utils.Graphics
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]
    public unsafe struct YFColor
    {
        public static YFColor Transparent => new YFColor(0x0, 0x0, 0x0, 0x0);

        public static YFColor Black => new YFColor(0x0, 0x0, 0x0, 0xFF);

        public const int BLUE = 0;
        public const int GREEN = 1;
        public const int RED = 2;
        public const int ALPHA = 3;

        public byte this[int channel]
        {
            get => channel switch
            {
                BLUE => Blue,
                GREEN => Green,
                RED => Red,
                ALPHA => Alpha,
                _ => 0
            };
            set
            {
                switch (channel)
                {
                    case BLUE: Blue = value; break;
                    case GREEN: Green = value; break;
                    case RED: Green = value; break;
                    case ALPHA: Alpha = value; break;
                }
            }
        }

        [FieldOffset(0x0)]
        public byte Blue;

        [FieldOffset(0x1)]
        public byte Green;

        [FieldOffset(0x2)]
        public byte Red;

        [FieldOffset(0x3)]
        public byte Alpha;

        public YFColor(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public YFColor(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = 0xFF;
        }

        public override string ToString()
        {
            return $"#{Alpha:x2}{Red:x2}{Green:x2}{Blue:x2}";
        }

        public static explicit operator YFColor(uint color)
        {
            return *(YFColor*)&color;
        }

        public static explicit operator uint(YFColor color)
        {
            return *(uint*)&color;
        }

        public void SwapRedBlue()
        {
            (Red, Blue) = (Blue, Red);
        }

        public void PremultiplyAlpha()
        {
            Red = (byte)(Red * Alpha / 255);
            Green = (byte)(Green * Alpha / 255);
            Blue = (byte)(Blue * Alpha / 255);
        }

        public void RepremultiplyAlpha()
        {
            if (Alpha != 0)
            {
                Red = (byte)(Red * 255 / Alpha);
                Green = (byte)(Green * 255 / Alpha);
                Blue = (byte)(Blue * 255 / Alpha);
            }
        }
    }
}
