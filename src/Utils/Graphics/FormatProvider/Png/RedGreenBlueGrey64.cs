namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal struct RedGreenBlueGrey64
    {
        public ushort Red;
        public ushort Green;
        public ushort Blue;
        public ushort Grey;

        public bool EqualRGB(ushort r, ushort g, ushort b)
        {
            return r == Red && g == Green && b == Blue;
        }

        public bool EqualGrey(int grey)
        {
            return grey == Grey;
        }
    }
}
