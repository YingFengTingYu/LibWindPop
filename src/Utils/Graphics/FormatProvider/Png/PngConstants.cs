namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal static class PngConstants
    {
        public static readonly byte[] Magic;
        public static readonly byte[] Chunk_IHDR;
        public static readonly byte[] Chunk_PLTE;
        public static readonly byte[] Chunk_IDAT;
        public static readonly byte[] Chunk_IEND;
        public static readonly byte[] Chunk_tEXt;
        public static readonly byte[] Chunk_tRNS;
        public static readonly byte[] Chunk_CgBI;

        /// <summary>
        /// Table of CRCs of all 8-bit messages.
        /// </summary>
        public static readonly uint[] CrcTable;

        static PngConstants()
        {
            Magic = new byte[8] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            Chunk_IHDR = new byte[4] { (byte)'I', (byte)'H', (byte)'D', (byte)'R' };
            Chunk_PLTE = new byte[4] { (byte)'P', (byte)'L', (byte)'T', (byte)'E' };
            Chunk_IDAT = new byte[4] { (byte)'I', (byte)'D', (byte)'A', (byte)'T' };
            Chunk_IEND = new byte[4] { (byte)'I', (byte)'E', (byte)'N', (byte)'D' };
            Chunk_tEXt = new byte[4] { (byte)'t', (byte)'E', (byte)'X', (byte)'t' };
            Chunk_tRNS = new byte[4] { (byte)'t', (byte)'R', (byte)'N', (byte)'S' };
            Chunk_CgBI = new byte[4] { (byte)'C', (byte)'g', (byte)'B', (byte)'I' };
            CrcTable = new uint[256];
            uint c;
            uint n, k;
            for (n = 0; n < 256; n++)
            {
                c = n;
                for (k = 0; k < 8; k++)
                {
                    if ((c & 1) != 0)
                    {
                        c = 0xedb88320u ^ (c >> 1);
                    }
                    else
                    {
                        c >>= 1;
                    }
                }
                CrcTable[n] = c;
            }
        }
    }
}
