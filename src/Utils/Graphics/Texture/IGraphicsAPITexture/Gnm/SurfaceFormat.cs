namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm
{
    /// <summary>
    /// Specifies the format of data in a Texture object.
    /// </summary>
    public enum SurfaceFormat
    {
        /// <summary>
        /// Invalid surface format.
        /// </summary>
        kSurfaceFormatInvalid = 0x00000000,
        /// <summary>
        /// One 8-bit channel. X=0xFF.
        /// </summary>
        kSurfaceFormat8 = 0x00000001,
        /// <summary>
        /// One 16-bit channel. X=0xFFFF.
        /// </summary>
        kSurfaceFormat16 = 0x00000002,
        /// <summary>
        /// Two 8-bit channels. X=0x00FF, Y=0xFF00.
        /// </summary>
        kSurfaceFormat8_8 = 0x00000003,
        /// <summary>
        /// One 32-bit channel. X=0xFFFFFFFF.
        /// </summary>
        kSurfaceFormat32 = 0x00000004,
        /// <summary>
        /// Two 16-bit channels. X=0x0000FFFF, Y=0xFFFF0000.
        /// </summary>
        kSurfaceFormat16_16 = 0x00000005,
        /// <summary>
        /// One 10-bit channel (Z) and two 11-bit channels (Y,X). X=0x000007FF, Y=0x003FF800, Z=0xFFC00000 Interpreted only as floating-point by texture unit, but also as integer by rasterizer.
        /// </summary>
        kSurfaceFormat10_11_11 = 0x00000006,
        /// <summary>
        /// Two 11-bit channels (Z,Y) and one 10-bit channel (X). X=0x000003FF, Y=0x001FFC00, Z=0xFFE00000 Interpreted only as floating-point by texture unit, but also as integer by rasterizer.
        /// </summary>
        kSurfaceFormat11_11_10 = 0x00000007,
        /// <summary>
        /// Three 10-bit channels (W,Z,Y) and one 2-bit channel (X). X=0x00000003, Y=0x00000FFC, Z=0x003FF000, W=0xFFC00000 X is never negative, even when YZW are.
        /// </summary>
        kSurfaceFormat10_10_10_2 = 0x00000008,
        /// <summary>
        /// One 2-bit channel (W) and three 10-bit channels (Z,Y,X). X=0x000003FF, Y=0x000FFC00, Z=0x3FF00000, W=0xC0000000 W is never negative, even when XYZ are.
        /// </summary>
        kSurfaceFormat2_10_10_10 = 0x00000009,
        /// <summary>
        /// Four 8-bit channels. X=0x000000FF, Y=0x0000FF00, Z=0x00FF0000, W=0xFF000000.
        /// </summary>
        kSurfaceFormat8_8_8_8 = 0x0000000A,
        /// <summary>
        /// Two 32-bit channels.
        /// </summary>
        kSurfaceFormat32_32 = 0x0000000B,
        /// <summary>
        /// Four 16-bit channels.
        /// </summary>
        kSurfaceFormat16_16_16_16 = 0x0000000C,
        /// <summary>
        /// Three 32-bit channels.
        /// </summary>
        kSurfaceFormat32_32_32 = 0x0000000D,
        /// <summary>
        /// Four 32-bit channels.
        /// </summary>
        kSurfaceFormat32_32_32_32 = 0x0000000E,
        /// <summary>
        /// One 5-bit channel (Z), one 6-bit channel (Y), and a second 5-bit channel (X). X=0x001F, Y=0x07E0, Z=0xF800.
        /// </summary>
        kSurfaceFormat5_6_5 = 0x00000010,
        /// <summary>
        /// One 1-bit channel (W) and three 5-bit channels (Z,Y,X). X=0x001F, Y=0x03E0, Z=0x7C00, W=0x8000.
        /// </summary>
        kSurfaceFormat1_5_5_5 = 0x00000011,
        /// <summary>
        /// Three 5-bit channels (W,Z,Y) and one 1-bit channel (X). X=0x0001, Y=0x003E, Z=0x07C0, W=0xF800.
        /// </summary>
        kSurfaceFormat5_5_5_1 = 0x00000012,
        /// <summary>
        /// Four 4-bit channels. X=0x000F, Y=0x00F0, Z=0x0F00, W=0xF000.
        /// </summary>
        kSurfaceFormat4_4_4_4 = 0x00000013,
        /// <summary>
        /// One 8-bit channel and one 24-bit channel.
        /// </summary>
        kSurfaceFormat8_24 = 0x00000014,
        /// <summary>
        /// One 24-bit channel and one 8-bit channel.
        /// </summary>
        kSurfaceFormat24_8 = 0x00000015,
        /// <summary>
        /// One 24-bit channel, one 8-bit channel, and one 32-bit channel.
        /// </summary>
        kSurfaceFormatX24_8_32 = 0x00000016,
        /// <summary>
        /// Description to be specified.
        /// </summary>
        kSurfaceFormatGB_GR = 0x00000020,
        /// <summary>
        /// Description to be specified.
        /// </summary>
        kSurfaceFormatBG_RG = 0x00000021,
        /// <summary>
        /// One 5-bit channel (W) and three 9-bit channels (Z,Y,X). X=0x000001FF, Y=0x0003FE00, Z=0x07FC0000, W=0xF8000000. Interpreted only as three 9-bit denormalized mantissas, and one shared 5-bit exponent.
        /// </summary>
        kSurfaceFormat5_9_9_9 = 0x00000022,
        /// <summary>
        /// BC1 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc1 = 0x00000023,
        /// <summary>
        /// BC2 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc2 = 0x00000024,
        /// <summary>
        /// BC3 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc3 = 0x00000025,
        /// <summary>
        /// BC4 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc4 = 0x00000026,
        /// <summary>
        /// BC5 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc5 = 0x00000027,
        /// <summary>
        /// BC6 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc6 = 0x00000028,
        /// <summary>
        /// BC7 block-compressed surface.
        /// </summary>
        kSurfaceFormatBc7 = 0x00000029,
        /// <summary>
        /// 8 bits-per-element FMASK surface (2 samples, 1 fragment).
        /// </summary>
        kSurfaceFormatFmask8_S2_F1 = 0x0000002C,
        /// <summary>
        /// 8 bits-per-element FMASK surface (4 samples, 1 fragment).
        /// </summary>
        kSurfaceFormatFmask8_S4_F1 = 0x0000002D,
        /// <summary>
        /// 8 bits-per-element FMASK surface (8 samples, 1 fragment).
        /// </summary>
        kSurfaceFormatFmask8_S8_F1 = 0x0000002E,
        /// <summary>
        /// 8 bits-per-element FMASK surface (2 samples, 2 fragments).
        /// </summary>
        kSurfaceFormatFmask8_S2_F2 = 0x0000002F,
        /// <summary>
        /// 8 bits-per-element FMASK surface (8 samples, 2 fragments).
        /// </summary>
        kSurfaceFormatFmask8_S4_F2 = 0x00000030,
        /// <summary>
        /// 8 bits-per-element FMASK surface (4 samples, 4 fragments).
        /// </summary>
        kSurfaceFormatFmask8_S4_F4 = 0x00000031,
        /// <summary>
        /// 16 bits-per-element FMASK surface (16 samples, 1 fragment).
        /// </summary>
        kSurfaceFormatFmask16_S16_F1 = 0x00000032,
        /// <summary>
        /// 16 bits-per-element FMASK surface (8 samples, 2 fragments).
        /// </summary>
        kSurfaceFormatFmask16_S8_F2 = 0x00000033,
        /// <summary>
        /// 32 bits-per-element FMASK surface (16 samples, 2 fragments).
        /// </summary>
        kSurfaceFormatFmask32_S16_F2 = 0x00000034,
        /// <summary>
        /// 32 bits-per-element FMASK surface (8 samples, 4 fragments).
        /// </summary>
        kSurfaceFormatFmask32_S8_F4 = 0x00000035,
        /// <summary>
        /// 32 bits-per-element FMASK surface (8 samples, 8 fragments).
        /// </summary>
        kSurfaceFormatFmask32_S8_F8 = 0x00000036,
        /// <summary>
        /// 64 bits-per-element FMASK surface (16 samples, 4 fragments).
        /// </summary>
        kSurfaceFormatFmask64_S16_F4 = 0x00000037,
        /// <summary>
        /// 64 bits-per-element FMASK surface (16 samples, 8 fragments).
        /// </summary>
        kSurfaceFormatFmask64_S16_F8 = 0x00000038,
        /// <summary>
        /// Two 4-bit channels (Y,X). X=0x0F, Y=0xF0.
        /// </summary>
        kSurfaceFormat4_4 = 0x00000039,
        /// <summary>
        /// One 6-bit channel (Z) and two 5-bit channels (Y,X). X=0x001F, Y=0x03E0, Z=0xFC00.
        /// </summary>
        kSurfaceFormat6_5_5 = 0x0000003A,
        /// <summary>
        /// One 1-bit channel. 8 pixels per byte, with pixel index increasing from LSB to MSB.
        /// </summary>
        kSurfaceFormat1 = 0x0000003B,
        /// <summary>
        /// One 1-bit channel. 8 pixels per byte, with pixel index increasing from MSB to LSB.
        /// </summary>
        kSurfaceFormat1Reversed = 0x0000003C,
    }
}
