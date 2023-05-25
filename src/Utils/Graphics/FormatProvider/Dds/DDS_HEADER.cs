using System.Runtime.InteropServices;
using DWORD = System.UInt32;

namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x7C)]
    internal unsafe struct DDS_HEADER
    {
        [FieldOffset(0x0)]
        public DWORD dwSize;
        [FieldOffset(0x4)]
        public DWORD dwFlags;
        [FieldOffset(0x8)]
        public DWORD dwHeight;
        [FieldOffset(0xC)]
        public DWORD dwWidth;
        [FieldOffset(0x10)]
        public DWORD dwPitchOrLinearSize;
        [FieldOffset(0x14)]
        public DWORD dwDepth;
        [FieldOffset(0x18)]
        public DWORD dwMipMapCount;
        [FieldOffset(0x1C)]
        public fixed DWORD dwReserved1[11];
        [FieldOffset(0x48)]
        public DDS_PIXELFORMAT ddspf;
        [FieldOffset(0x68)]
        public DWORD dwCaps;
        [FieldOffset(0x6C)]
        public DWORD dwCaps2;
        [FieldOffset(0x70)]
        public DWORD dwCaps3;
        [FieldOffset(0x74)]
        public DWORD dwCaps4;
        [FieldOffset(0x78)]
        public DWORD dwReserved2;

        public const DWORD DDSD_CAPS = 0x1;
        public const DWORD DDSD_HEIGHT = 0x2;
        public const DWORD DDSD_WIDTH = 0x4;
        public const DWORD DDSD_PITCH = 0x8;
        public const DWORD DDSD_PIXELFORMAT = 0x1000;
        public const DWORD DDSD_MIPMAPCOUNT = 0x20000;
        public const DWORD DDSD_LINEARSIZE = 0x80000;
        public const DWORD DDSD_DEPTH = 0x800000;

        public const DWORD DDSCAPS_COMPLEX = 0x8;
        public const DWORD DDSCAPS_MIPMAP = 0x400000;
        public const DWORD DDSCAPS_TEXTURE = 0x1000;
    }
}
