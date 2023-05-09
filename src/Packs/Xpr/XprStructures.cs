using System.Runtime.InteropServices;

namespace LibWindPop.Packs.Xpr.XprStructures
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
    public struct XprFileInfo
    {
        [FieldOffset(0x0)]
        public uint Type;

        [FieldOffset(0x4)]
        public uint FileOffset;

        [FieldOffset(0x8)]
        public uint FileSize;

        [FieldOffset(0xC)]
        public uint PathOffset;
    }
}
