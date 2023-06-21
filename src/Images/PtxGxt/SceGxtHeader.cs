using System.Runtime.InteropServices;
using uint32_t = System.UInt32;

namespace LibWindPop.Images.PtxGxt
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]
    public struct SceGxtHeader
    {
        /// <summary>
        /// The GXT identifier.
        /// </summary>
        [FieldOffset(0x0)]
        public uint32_t tag;
        /// <summary>
        /// The version number.
        /// </summary>
        [FieldOffset(0x4)]
        public uint32_t version;
        /// <summary>
        /// The number of textures.
        /// </summary>
        [FieldOffset(0x8)]
        public uint32_t numTextures;
        /// <summary>
        /// The offset to the texture data.
        /// </summary>
        [FieldOffset(0xC)]
        public uint32_t dataOffset;
        /// <summary>
        /// The total size of the texture data.
        /// </summary>
        [FieldOffset(0x10)]
        public uint32_t dataSize;
        /// <summary>
        /// The number of 16 entry palettes.
        /// </summary>
        [FieldOffset(0x14)]
        public uint32_t numP4Palettes;
        /// <summary>
        /// The number of 256 entry palettes.
        /// </summary>
        [FieldOffset(0x18)]
        public uint32_t numP8Palettes;
        /// <summary>
        /// Padding.
        /// </summary>
        [FieldOffset(0x1C)]
        public uint32_t pad;
    }
}
