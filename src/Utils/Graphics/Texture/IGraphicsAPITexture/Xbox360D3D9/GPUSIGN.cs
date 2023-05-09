namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9
{
    /// <summary>
    /// Describes the sign of a value.
    /// </summary>
    public enum GPUSIGN
    {
        /// <summary>
        /// Indicates that a value is unsigned.
        /// </summary>
        GPUSIGN_UNSIGNED = 0,
        /// <summary>
        /// Indicates that a value is signed.
        /// </summary>
        GPUSIGN_SIGNED = 1,
        /// <summary>
        /// Indicates that a value is unsigned biased. GPUSIGN_BIAS maps an UNSIGNED value with a range of 0 to 1 into a range of -1 to 1. The equation used to perform the mapping is 2 * x - 1. For example, the UNSIGNED values 0, 0.5 and 1 would map to -1, 0 and 1.
        /// </summary>
        GPUSIGN_BIAS = 2,
        /// <summary>
        /// Indicates that a value is unsigned gamma corrected.
        /// </summary>
        GPUSIGN_GAMMA = 3,

        GPUSIGN_ALL_UNSIGNED = GPUSIGN_UNSIGNED | GPUSIGN_UNSIGNED << 2 | GPUSIGN_UNSIGNED << 4 | GPUSIGN_UNSIGNED << 6,
        GPUSIGN_ALL_SIGNED = GPUSIGN_SIGNED | GPUSIGN_SIGNED << 2 | GPUSIGN_SIGNED << 4 | GPUSIGN_SIGNED << 6,
    }
}
