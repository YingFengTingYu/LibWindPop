namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    public struct PngEncoderArgument : IImageEncoderArgument
    {
        public int ZlibLevel;

        public PngEncoderArgument(int zlibLevel)
        {
            ZlibLevel = zlibLevel;
        }
    }
}
