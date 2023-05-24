using System.Runtime.InteropServices;
using UINT = System.UInt32;

namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x14)]
    internal struct DDS_HEADER_DXT10
    {
        [FieldOffset(0x0)]
        public DXGI_FORMAT dxgiFormat;
        [FieldOffset(0x4)]
        public D3D10_RESOURCE_DIMENSION resourceDimension;
        [FieldOffset(0x8)]
        public UINT miscFlag;
        [FieldOffset(0xC)]
        public UINT arraySize;
        [FieldOffset(0x10)]
        public UINT miscFlags2;

        public const UINT DDS_RESOURCE_MISC_TEXTURECUBE = 0x4;

        public const UINT DDS_ALPHA_MODE_UNKNOWN = 0x0;
        public const UINT DDS_ALPHA_MODE_STRAIGHT = 0x1;
        public const UINT DDS_ALPHA_MODE_PREMULTIPLIED = 0x2;
        public const UINT DDS_ALPHA_MODE_OPAQUE = 0x3;
        public const UINT DDS_ALPHA_MODE_CUSTOM = 0x4;
    }
}
