using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LibWindPop.Images.PtxGxt
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]
    public unsafe struct SceGxtTextureInfo
    {
        /// <summary>
        /// The offset to the texture data.
        /// </summary>
        [FieldOffset(0x0)]
        public uint32_t dataOffset;
        /// <summary>
        /// The size of the texture data.
        /// </summary>
        [FieldOffset(0x4)]
        public uint32_t dataSize;
        /// <summary>
        /// The index of the palette.
        /// </summary>
        [FieldOffset(0x8)]
        public uint32_t paletteIndex;
        /// <summary>
        /// Texture flags (SceGxtTextureFlags).
        /// </summary>
        [FieldOffset(0xC)]
        public uint32_t flags;
        /// <summary>
        /// The texture type (SceGxmTextureType).
        /// </summary>
        [FieldOffset(0x10)]
        public uint32_t type;
        /// <summary>
        /// The texture format (SceGxmTextureFormat).
        /// </summary>
        [FieldOffset(0x14)]
        public uint32_t format;
        /// <summary>
        /// The texture width.
        /// </summary>
        [FieldOffset(0x18)]
        public uint16_t width;
        /// <summary>
        /// The texture height.
        /// </summary>
        [FieldOffset(0x1A)]
        public uint16_t height;
        /// <summary>
        /// The number of mipmaps.
        /// </summary>
        [FieldOffset(0x1C)]
        public uint8_t mipCount;
        /// <summary>
        /// Padding.
        /// </summary>
        [FieldOffset(0x1D)]
        public fixed uint8_t pad[3];

        /// <summary>
        /// The texture contains border data.
        /// </summary>
        public const uint32_t SCE_GXT_TEXTURE_FLAG_HAS_BORDER_DATA = 0x00000001u;
    }
}
