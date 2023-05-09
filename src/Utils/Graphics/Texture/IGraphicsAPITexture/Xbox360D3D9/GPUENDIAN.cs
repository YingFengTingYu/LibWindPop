namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9
{
    /// <summary>
    /// Describes whether or not to do byte or word swapping to support different endian modes.
    /// </summary>
    public enum GPUENDIAN
    {
        /// <summary>
        /// No swapping.
        /// </summary>
        GPUENDIAN_NONE = 0,
        /// <summary>
        /// Every 8 bits in a 16-bit word are swapped. For example, 0xAABBCCDD would be changed to 0xBBAADDCC.
        /// </summary>
        GPUENDIAN_8IN16 = 1,
        /// <summary>
        /// Every 8 bits in a 32-bit word are swapped. For example, 0xAABBCCDD would be changed to 0xDDCCBBAA.
        /// </summary>
        GPUENDIAN_8IN32 = 2,
        /// <summary>
        /// Every 16 bits in a 32-bit word are swapped. For example, 0xAABBCCDD would be changed to 0xCCDDAABB.
        /// </summary>
        GPUENDIAN_16IN32 = 3,
    }
}
