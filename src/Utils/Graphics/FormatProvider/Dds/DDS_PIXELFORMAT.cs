using System.Runtime.InteropServices;
using DWORD = System.UInt32;

namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]
    internal struct DDS_PIXELFORMAT
    {
        [FieldOffset(0x0)]
        public DWORD dwSize;
        [FieldOffset(0x4)]
        public DWORD dwFlags;
        [FieldOffset(0x8)]
        public DWORD dwFourCC;
        [FieldOffset(0xC)]
        public DWORD dwRGBBitCount;
        [FieldOffset(0x10)]
        public DWORD dwRBitMask;
        [FieldOffset(0x14)]
        public DWORD dwGBitMask;
        [FieldOffset(0x18)]
        public DWORD dwBBitMask;
        [FieldOffset(0x1C)]
        public DWORD dwABitMask;

        public const DWORD DDPF_ALPHAPIXELS = 0x1;
        public const DWORD DDPF_ALPHA = 0x2;
        public const DWORD DDPF_FOURCC = 0x4;
        public const DWORD DDPF_RGB = 0x40;
        public const DWORD DDPF_YUV = 0x200;
        public const DWORD DDPF_LUMINANCE = 0x20000;

        public static readonly DWORD Dx10 = MakeFourCc('D', 'X', '1', '0'); // dds10
        public static readonly DWORD Dxt1 = MakeFourCc('D', 'X', 'T', '1'); // DXT1
        public static readonly DWORD Dxt2 = MakeFourCc('D', 'X', 'T', '2'); // DXT3
        public static readonly DWORD Dxt3 = MakeFourCc('D', 'X', 'T', '3'); // DXT3
        public static readonly DWORD Dxt4 = MakeFourCc('D', 'X', 'T', '4'); // DXT5
        public static readonly DWORD Dxt5 = MakeFourCc('D', 'X', 'T', '5'); // DXT5
        public static readonly DWORD Ati1 = MakeFourCc('A', 'T', 'I', '1'); // BC4
        public static readonly DWORD Ati2 = MakeFourCc('A', 'T', 'I', '2'); // BC5
        public static readonly DWORD Atc = MakeFourCc('A', 'T', 'C', ' '); // ATC
        public static readonly DWORD Atci = MakeFourCc('A', 'T', 'C', 'I'); // ATCExplicit
        public static readonly DWORD Atca = MakeFourCc('A', 'T', 'C', 'A'); // ATCInterpolated
        public static readonly DWORD Bc4S = MakeFourCc('B', 'C', '4', 'S'); // BC4
        public static readonly DWORD Bc4U = MakeFourCc('B', 'C', '4', 'U'); // BC4
        public static readonly DWORD Bc5S = MakeFourCc('B', 'C', '5', 'S'); // BC5
        public static readonly DWORD Bc5U = MakeFourCc('B', 'C', '5', 'U'); // BC5

        private static uint MakeFourCc(char c0, char c1, char c2, char c3)
        {
            uint result = c0;
            result |= (uint)c1 << 8;
            result |= (uint)c2 << 16;
            result |= (uint)c3 << 24;
            return result;
        }
    }
}
