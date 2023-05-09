namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm
{
    /// <summary>
    /// Channel format used by Textures. 
    /// <para>There are three types of integer encoding: U, S, and UB. U means "unsigned" and indicates that the number is stored as an unsigned integer. S means "signed" and indicates that the number is stored as a signed integer. UB means "unsigned biased" and indicates that the integer is stored as unsigned, but the lowest encodable value of a signed integer with the same number of bits is added when decoding. That is to say, 8-bit UBNorm encodings for float -1 and 1 are the bit patterns 00000000 (0) and 11111111 (255,) while 8-bit SNorm encodings for float -1 and 1 are the bit patterns 10000000 (-128) and 01111111 (127).</para>
    /// <para>There are three means for linear scale and offset of a decoded integer during conversion to and from float: "Norm", "Scaled", and "NormNoZero". "Norm" indicates that the integer range is normalized to the floating-point range 0..1 or -1..1 inclusive. In the case of SNorm, the lowest two negative integer values both decode as float -1, so that the float value 0 is also represented. "Scaled" indicates that an integer is converted to a float with the same value as the integer. "NormNoZero" indicates that the integer range is normalized to -1..1, but with no duplication of the lowest two encodings as float -1, and no representation of float zero either.</para>
    /// <para>There are two means to indicate that there is no integer-float conversion: "Int" and "Float." Both indicate that the encoded bits are to be copied directly to or from a shader register, with extension to or from the register's 32 bits.</para>
    /// <para>"Srgb" encoding indicates that an unsigned integer encodes the float range 0..1 with a function applied in an attempt to approximate the gamma response of an sRGB image.</para>
    /// <para>Two-bit channels never decode to negative values, even when ten-bit channels in the same element do.</para>
    /// </summary>
    public enum TextureChannelType
    {
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as float X/(N-1).
        /// </summary>
        kTextureChannelTypeUNorm = 0x00000000,
        /// <summary>
        /// Stored as int -N/2&lt;=X&lt;N/2, interpreted as float MAX(-1,X/(N/2-1)).
        /// </summary>
        kTextureChannelTypeSNorm = 0x00000001,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as float X.
        /// </summary>
        kTextureChannelTypeUScaled = 0x00000002,
        /// <summary>
        /// Stored as int -N/2&lt;=X&lt;N/2, interpreted as float X.
        /// </summary>
        kTextureChannelTypeSScaled = 0x00000003,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as uint X. Not filterable.
        /// </summary>
        kTextureChannelTypeUInt = 0x00000004,
        /// <summary>
        /// Stored as int -N/2&lt;=X&lt;N/2, interpreted as int X. Not filterable.
        /// </summary>
        kTextureChannelTypeSInt = 0x00000005,
        /// <summary>
        /// Stored as int -N/2&lt;=X&lt;N/2, interpreted as float ((X+N/2)/(N-1))*2-1.
        /// </summary>
        kTextureChannelTypeSNormNoZero = 0x00000006,
        /// <summary>
        /// Stored as float, interpreted as float.
        /// <para> • 32-bit: SE8M23, bias 127, range (-2^129..2^129)</para>
        /// <para> • 16-bit: SE5M10, bias 15, range (-2^17..2^17)</para>
        /// <para> • 11-bit: E5M6 bias 15, range [0..2^17)</para>
        /// <para> • 10-bit: E5M5 bias 15, range [0..2^17)</para>
        /// </summary>
        kTextureChannelTypeFloat = 0x00000007,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as float sRGB(X/(N-1)). Srgb only applies to the XYZ channels of the texture; the W channel is always linear. 
        /// </summary>
        kTextureChannelTypeSrgb = 0x00000009,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as float MAX(-1,(X-N/2)/(N/2-1)).
        /// </summary>
        kTextureChannelTypeUBNorm = 0x0000000A,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as float (X/(N-1))*2-1.
        /// </summary>
        kTextureChannelTypeUBNormNoZero = 0x0000000B,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as int X-N/2. Not blendable or filterable.
        /// </summary>
        kTextureChannelTypeUBInt = 0x0000000C,
        /// <summary>
        /// Stored as uint X&lt;N, interpreted as float X-N/2.
        /// </summary>
        kTextureChannelTypeUBScaled = 0x0000000D,
    }
}
