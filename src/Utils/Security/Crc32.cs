using System;

namespace LibWindPop.Utils.Security
{
    public struct Crc32 : IHashAlgorithm, I32BitsHashAlgorithm
    {
        private const uint CRC32_MASK = 0xFFFFFFFFu;

        private uint _value;

        public uint Value
        {
            get
            {
                return _value ^ CRC32_MASK;
            }
            set
            {
                _value = value ^ CRC32_MASK;
            }
        }

        public int ByteSize => 4;

        private static readonly uint[] CrcTable;

        static Crc32()
        {
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
                        c = 0xEDB88320u ^ (c >> 1);
                    }
                    else
                    {
                        c >>= 1;
                    }
                }
                CrcTable[n] = c;
            }
        }

        public void Init()
        {
            _value = CRC32_MASK;
        }

        public void Update(byte data)
        {
            _value = CrcTable[(_value ^ data) & 0xFFu] ^ _value >> 8;
        }

        public void Update(ReadOnlySpan<byte> data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                _value = CrcTable[(_value ^ data[i]) & 0xFFu] ^ _value >> 8;
            }
        }
    }
}
