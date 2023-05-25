namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    public struct DdsEncoderArgument : IImageEncoderArgument
    {
        public DdsEncodingFormat Format;
        public bool UseDX10Header;

        public DdsEncoderArgument(DdsEncodingFormat format, bool useDX10Header)
        {
            Format = format;
            UseDX10Header = useDX10Header;
        }
    }
}
