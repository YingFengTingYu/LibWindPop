using System.Runtime.InteropServices;

namespace LibWindPop.PopCap.Packs.Rsb.Map
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x14)]
    public struct CompiledMapEncodePair
    {
        [FieldOffset(0x0)]
        public uint KeySize;

        [FieldOffset(0x4)]
        public uint KeyOffset;

        [FieldOffset(0x8)]
        public uint ValueSize;

        [FieldOffset(0xC)]
        public uint ValueOffset;

        [FieldOffset(0x10)]
        public uint repeat_len;

        public static uint PeekSize(string key, uint value_len)
        {
            return (uint)key.Length + 1 + value_len;
        }
    }
}
