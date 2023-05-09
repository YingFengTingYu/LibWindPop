namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9
{
    /// <summary>
    /// Describes how components of a texel are mapped to the elements of a register into which that texel is fetched.
    /// </summary>
    public enum GPUSWIZZLE
    {
        /// <summary>
        /// Describes the X component of a register.
        /// </summary>
        GPUSWIZZLE_X = 0,
        /// <summary>
        /// Describes the Y component of a register.
        /// </summary>
        GPUSWIZZLE_Y = 1,
        /// <summary>
        /// Describes the Z component of a register.
        /// </summary>
        GPUSWIZZLE_Z = 2,
        /// <summary>
        /// Describes the W component of a register.
        /// </summary>
        GPUSWIZZLE_W = 3,
        /// <summary>
        /// Describes the 0 component of a register.
        /// </summary>
        GPUSWIZZLE_0 = 4,
        /// <summary>
        /// Describes the 1 component of a register.
        /// </summary>
        GPUSWIZZLE_1 = 5,
        /// <summary>
        /// NULL
        /// </summary>
        GPUSWIZZLE_KEEP = 7, // Fetch instructions only

        GPUSWIZZLE_ARGB = GPUSWIZZLE_Z | GPUSWIZZLE_Y << 3 | GPUSWIZZLE_X << 6 | GPUSWIZZLE_W << 9,
        GPUSWIZZLE_ORGB = GPUSWIZZLE_Z | GPUSWIZZLE_Y << 3 | GPUSWIZZLE_X << 6 | GPUSWIZZLE_1 << 9,
        GPUSWIZZLE_ABGR = GPUSWIZZLE_X | GPUSWIZZLE_Y << 3 | GPUSWIZZLE_Z << 6 | GPUSWIZZLE_W << 9,
        GPUSWIZZLE_OBGR = GPUSWIZZLE_X | GPUSWIZZLE_Y << 3 | GPUSWIZZLE_Z << 6 | GPUSWIZZLE_1 << 9,
        GPUSWIZZLE_OOGR = GPUSWIZZLE_X | GPUSWIZZLE_Y << 3 | GPUSWIZZLE_1 << 6 | GPUSWIZZLE_1 << 9,
        GPUSWIZZLE_OZGR = GPUSWIZZLE_X | GPUSWIZZLE_Y << 3 | GPUSWIZZLE_0 << 6 | GPUSWIZZLE_1 << 9,
        GPUSWIZZLE_RZZZ = GPUSWIZZLE_0 | GPUSWIZZLE_0 << 3 | GPUSWIZZLE_0 << 6 | GPUSWIZZLE_X << 9,
        GPUSWIZZLE_OOOR = GPUSWIZZLE_X | GPUSWIZZLE_1 << 3 | GPUSWIZZLE_1 << 6 | GPUSWIZZLE_1 << 9,
        GPUSWIZZLE_ORRR = GPUSWIZZLE_X | GPUSWIZZLE_X << 3 | GPUSWIZZLE_X << 6 | GPUSWIZZLE_1 << 9,
        GPUSWIZZLE_GRRR = GPUSWIZZLE_X | GPUSWIZZLE_X << 3 | GPUSWIZZLE_X << 6 | GPUSWIZZLE_Y << 9,
        GPUSWIZZLE_RGBA = GPUSWIZZLE_W | GPUSWIZZLE_Z << 3 | GPUSWIZZLE_Y << 6 | GPUSWIZZLE_X << 9,
    }
}
