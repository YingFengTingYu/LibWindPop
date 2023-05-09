namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm
{
    /// <summary>
    /// Specifies the swizzle pattern in a Texture.
    /// <para>This enumeration specifies the source of a single data channel (when sampling from a Texture), and/or the destination channel (when writing to an RW_Texture).</para>
    /// </summary>
    public enum TextureChannel
    {
        /// <summary>
        /// Destination channel will contain a constant 0.0.
        /// </summary>
        kTextureChannelConstant0 = 0x00000000,
        /// <summary>
        /// Destination channel will contain a constant 1.0.
        /// </summary>
        kTextureChannelConstant1 = 0x00000001,
        /// <summary>
        /// Destination channel will contain the source's X channel.
        /// </summary>
        kTextureChannelX = 0x00000004,
        /// <summary>
        /// Destination channel will contain the source's Y channel.
        /// </summary>
        kTextureChannelY = 0x00000005,
        /// <summary>
        /// Destination channel will contain the source's Z channel.
        /// </summary>
        kTextureChannelZ = 0x00000006,
        /// <summary>
        /// Destination channel will contain the source's W channel.
        /// </summary>
        kTextureChannelW = 0x00000007,
    }
}
