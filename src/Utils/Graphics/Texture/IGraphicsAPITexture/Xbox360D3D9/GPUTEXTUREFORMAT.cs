namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9
{
    /// <summary>
    /// Describes the format of a texture surface.
    /// </summary>
    public enum GPUTEXTUREFORMAT
    {
        /// <summary>
        /// Texture format with a single 1-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_1_REVERSE = 0,
        /// <summary>
        /// Texture format with a single 1-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_1 = 1,
        /// <summary>
        /// Texture format with a single 8 bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_8 = 2,
        /// <summary>
        /// Texture format with a single 1-bit channel and three 5-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_1_5_5_5 = 3,
        /// <summary>
        /// Texture format with one 6-bit channel and two 5-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_5_6_5 = 4,
        /// <summary>
        /// Texture format with one 6-bit channel and two 5-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_6_5_5 = 5,
        /// <summary>
        /// Texture format with four 8-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_8_8_8_8 = 6,
        /// <summary>
        /// Texture format with a single 2-bit channel and three 10-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_2_10_10_10 = 7,
        /// <summary>
        /// Texture format with a single 8-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_8_A = 8,
        /// <summary>
        /// Texture format with a single 8-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_8_B = 9,
        /// <summary>
        /// Texture format with two 8-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_8_8 = 10,
        /// <summary>
        /// Texture format with two texels stored in each word. Each data value (Cr, Y1, Cb, Y0) is 8 bits. The Cr and Cb specify the chrominance and are shared by both texels. The Y0 and Y1 values specify the luminance of the respective texels.
        /// </summary>
        GPUTEXTUREFORMAT_Cr_Y1_Cb_Y0_REP = 11,
        /// <summary>
        /// Texture format with two texels stored in each word. Each data value (Y1, Cr, Y0, Cb) is 8 bits. The Cr and Cb specify the chrominance and are shared by both texels. The Y0 and Y1 values specify the luminance of the respective texels.
        /// </summary>
        GPUTEXTUREFORMAT_Y1_Cr_Y0_Cb_REP = 12,
        /// <summary>
        /// Format with two 16-bit channels. Format is not applicable to textures and only applies to render targets in EDRAM.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_EDRAM = 13, // EDRAM render target only
        /// <summary>
        /// Texture format with four 8-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_8_8_8_8_A = 14,
        /// <summary>
        /// Texture format with four 4-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_4_4_4_4 = 15,
        /// <summary>
        /// Texture format with a single 10-bit channel and two 11-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_10_11_11 = 16,
        /// <summary>
        /// Texture format with a single 10-bit channel and two 11-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_11_11_10 = 17,
        /// <summary>
        /// DXT1 compression texture format with 4 bits per texel.
        /// </summary>
        GPUTEXTUREFORMAT_DXT1 = 18,
        /// <summary>
        /// DXT2 compression texture format with 8 bits per texel.
        /// </summary>
        GPUTEXTUREFORMAT_DXT2_3 = 19,
        /// <summary>
        /// DXT4 compression texture format with 8 bits per texel.
        /// </summary>
        GPUTEXTUREFORMAT_DXT4_5 = 20,
        /// <summary>
        /// Format with four 16-bit channels. Format is not applicable to textures and only applies to render targets in EDRAM.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_16_16_EDRAM = 21, // EDRAM render target only
        /// <summary>
        /// Texture format with a 24-bit channel and an 8-bit channel. When applied to textures the 8-bit channel is ignored. When applied to depth-stencil targets the 8-bit channel maps to stencil.
        /// </summary>
        GPUTEXTUREFORMAT_24_8 = 22,
        /// <summary>
        /// Texture format with a 24-bit float channel and an 8-bit integer channel. When applied to textures the 8-bit channel is ignored. When applied to depth-stencil targets the 8-bit channel maps to stencil.
        /// </summary>
        GPUTEXTUREFORMAT_24_8_FLOAT = 23,
        /// <summary>
        /// Texture format with a single 16-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_16 = 24,
        /// <summary>
        /// Texture format with two 16-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_16_16 = 25,
        /// <summary>
        /// Texture format with four 16-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_16_16 = 26,
        /// <summary>
        /// 16-bit float texture format that is expanded to 32-bit (16.16) fixed point during sampling to allow filtering. This format samples at half the rate of GPUTEXTUREFORMAT_16.
        /// </summary>
        GPUTEXTUREFORMAT_16_EXPAND = 27,
        /// <summary>
        /// 32-bit float texture format that is expanded to 64-bit (16.16, 16.16) fixed point during sampling to allow filtering. This format samples at half the rate of GPUTEXTUREFORMAT_16_16.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_EXPAND = 28,
        /// <summary>
        /// 64-bit float texture format that is expanded to 128-bit (16.16, 16.16, 16.16, 16.16) fixed point during sampling to allow filtering. This format samples at half the rate of GPUTEXTUREFORMAT_16_16_16_16.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_16_16_EXPAND = 29,
        /// <summary>
        /// Texture format with a single 16-bit float channel.
        /// </summary>
        GPUTEXTUREFORMAT_16_FLOAT = 30,
        /// <summary>
        /// Texture format with two 16-bit float channels.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_FLOAT = 31,
        /// <summary>
        /// Texture format with four 16-bit float channels.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_16_16_FLOAT = 32,
        /// <summary>
        /// Texture format with a single 32-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_32 = 33,
        /// <summary>
        /// Texture format with two 32-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_32_32 = 34,
        /// <summary>
        /// Texture format with four 32-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_32_32_32_32 = 35,
        /// <summary>
        /// Texture format with a single 32-bit float channel.
        /// </summary>
        GPUTEXTUREFORMAT_32_FLOAT = 36,
        /// <summary>
        /// Texture format with two 32-bit float channels.
        /// </summary>
        GPUTEXTUREFORMAT_32_32_FLOAT = 37,
        /// <summary>
        /// Texture format with four 32-bit float channels.
        /// </summary>
        GPUTEXTUREFORMAT_32_32_32_32_FLOAT = 38,
        /// <summary>
        /// Texture format with a single 32-bit channel that is stored as an 8-bit channel in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_32_AS_8 = 39,
        /// <summary>
        /// Texture format with a single 32-bit channel that is stored as two 8-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_32_AS_8_8 = 40,
        /// <summary>
        /// Texture format with a single 16-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_16_MPEG = 41,
        /// <summary>
        /// Texture format with two 16-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_MPEG = 42,
        /// <summary>
        /// Texture format with a single 8-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_8_INTERLACED = 43,
        /// <summary>
        /// Texture format with a single 32-bit channel that is stored as an 8-bit channel in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_32_AS_8_INTERLACED = 44,
        /// <summary>
        /// Texture format with a single 32-bit channel that is stored as two 8-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_32_AS_8_8_INTERLACED = 45,
        /// <summary>
        /// Texture format with a single 16-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_16_INTERLACED = 46,
        /// <summary>
        /// Texture format with a single 16-bit channel.
        /// </summary>
        GPUTEXTUREFORMAT_16_MPEG_INTERLACED = 47,
        /// <summary>
        /// Texture format with two 16-bit channels.
        /// </summary>
        GPUTEXTUREFORMAT_16_16_MPEG_INTERLACED = 48,
        /// <summary>
        /// Two-component 8-bit per texel format made up of two DXT4/5 alpha blocks.
        /// </summary>
        GPUTEXTUREFORMAT_DXN = 49,
        /// <summary>
        /// GPUTEXTUREFORMAT_8_8_8_8 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_8_8_8_8_AS_16_16_16_16 = 50,
        /// <summary>
        /// GPUTEXTUREFORMAT_DXT1 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_DXT1_AS_16_16_16_16 = 51,
        /// <summary>
        /// GPUTEXTUREFORMAT_DXT2_3 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_DXT2_3_AS_16_16_16_16 = 52,
        /// <summary>
        /// GPUTEXTUREFORMAT_DXT4_5 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_DXT4_5_AS_16_16_16_16 = 53,
        /// <summary>
        /// GPUTEXTUREFORMAT_2_10_10_10 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16 = 54,
        /// <summary>
        /// GPUTEXTUREFORMAT_10_11_11 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_10_11_11_AS_16_16_16_16 = 55,
        /// <summary>
        /// GPUTEXTUREFORMAT_11_11_10 texture format that is stored as four 16-bit channels in the texture cache.
        /// </summary>
        GPUTEXTUREFORMAT_11_11_10_AS_16_16_16_16 = 56,
        /// <summary>
        /// Texture format with three 32-bit float channels.
        /// </summary>
        GPUTEXTUREFORMAT_32_32_32_FLOAT = 57,
        /// <summary>
        /// Single-component 4-bit per texel format made up of a DXT2/3 alpha block.
        /// </summary>
        GPUTEXTUREFORMAT_DXT3A = 58,
        /// <summary>
        /// Single-component 4-bit per texel format made up of a DXT4/5 alpha block.
        /// </summary>
        GPUTEXTUREFORMAT_DXT5A = 59,
        /// <summary>
        /// Two-component 4-bit per texel format similar to DXT1 but with 8:8 colors instead of 5:6:5 colors.
        /// </summary>
        GPUTEXTUREFORMAT_CTX1 = 60,
        /// <summary>
        /// Four-component format encoded in a DXT2/3 alpha block where each bit is expanded into a separate channel.
        /// </summary>
        GPUTEXTUREFORMAT_DXT3A_AS_1_1_1_1 = 61,
        /// <summary>
        /// Format with four 8-bit channels. Format is not applicable to textures and only applies to render targets in EDRAM.
        /// </summary>
        GPUTEXTUREFORMAT_8_8_8_8_GAMMA_EDRAM = 62, // EDRAM render target only
        /// <summary>
        /// Format with a single 2-bit float channel and three 10-bit float channels. Format is not applicable to textures and only applies to render targets in EDRAM.
        /// </summary>
        GPUTEXTUREFORMAT_2_10_10_10_FLOAT_EDRAM = 63, // EDRAM render target only
    }
}
