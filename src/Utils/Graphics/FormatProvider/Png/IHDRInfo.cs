namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal struct IHDRInfo
    {
        public uint width;
        public uint height;
        public byte bitDepth;
        public PngColorType colorType;
        public byte compressionMethod;
        public byte filterMethod;
        public byte interlaceMethod;

        public IHDRInfo(uint width, uint height, byte bitDepth, PngColorType colorType, byte compressionMethod, byte filterMethod, byte interlaceMethod)
        {
            this.width = width;
            this.height = height;
            this.bitDepth = bitDepth;
            this.colorType = colorType;
            this.compressionMethod = compressionMethod;
            this.filterMethod = filterMethod;
            this.interlaceMethod = interlaceMethod;
        }

        public override string ToString()
        {
            return $"IHDRChunk(width = {width}, height = {height}, bitDepth = {bitDepth}, colorType = {colorType}, compressionMethod = {compressionMethod}, filterMethod = {filterMethod}, interlaceMethod = {interlaceMethod})";
        }
    }
}
