using System;

namespace LibWindPop.Utils.Security
{
    public struct Fnv1_32 : IHashAlgorithm, I32BitsHashAlgorithm
    {
        private const uint FNV1_32_INIT = 2166136261u;

        private uint m_value;

        public uint Value
        {
            get => m_value;
            set => m_value = value;
        }

        public int ByteSize => 4;

        public void Init()
        {
            m_value = FNV1_32_INIT;
        }

        public void Update(byte data)
        {
            m_value = ((m_value << 1) + (m_value << 4) + (m_value << 7) + (m_value << 8) + (m_value << 24) + m_value) ^ data;
        }

        public void Update(ReadOnlySpan<byte> data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                m_value = ((m_value << 1) + (m_value << 4) + (m_value << 7) + (m_value << 8) + (m_value << 24) + m_value) ^ data[i];
            }
        }
    }
}
