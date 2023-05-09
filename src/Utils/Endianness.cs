namespace LibWindPop.Utils
{
    /// <summary>
    /// endian
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// unknow
        /// </summary>
        None = 0,
        /// <summary>
        /// little endian
        /// </summary>
        Little = 1,
        /// <summary>
        /// big endian
        /// </summary>
        Big = 2,
        /// <summary>
        /// native endian
        /// </summary>
        Native = 3,
        /// <summary>
        /// not native endian
        /// </summary>
        NoneNative = 4,
    }
}
