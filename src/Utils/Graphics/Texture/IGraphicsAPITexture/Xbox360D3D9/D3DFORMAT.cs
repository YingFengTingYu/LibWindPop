﻿using System;

namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Xbox360D3D9
{
    /// <summary>
    /// Defines the various types of surface formats. 
    /// </summary>
    public readonly struct D3DFORMAT : IEquatable<D3DFORMAT>
    {
        /// <summary>
        /// inner value
        /// </summary>
        private readonly uint value;

        /// <summary>
        /// Describes the texture format.
        /// </summary>
        public GPUTEXTUREFORMAT TextureFormat => (GPUTEXTUREFORMAT)((value >> 0) & 0x3Fu);

        /// <summary>
        /// Describes whether the format is big endian or little endian.
        /// </summary>
        public GPUENDIAN Endian => (GPUENDIAN)((value >> 6) & 0x3u);

        /// <summary>
        /// Describes whether the format is tiled or linear.
        /// </summary>
        public bool Tiled => ((value >> 8) & 0x1u) != 0u;

        /// <summary>
        /// Describes whether the sign of individual texel elements is unsigned, signed, unsigned biased, or unsigned gamma corrected (for select data formats).
        /// </summary>
        public GPUSIGN TextureSignX => (GPUSIGN)((value >> 9) & 0x3u);

        /// <summary>
        /// Describes whether the sign of individual texel elements is unsigned, signed, unsigned biased, or unsigned gamma corrected (for select data formats).
        /// </summary>
        public GPUSIGN TextureSignY => (GPUSIGN)((value >> 11) & 0x3u);

        /// <summary>
        /// Describes whether the sign of individual texel elements is unsigned, signed, unsigned biased, or unsigned gamma corrected (for select data formats).
        /// </summary>
        public GPUSIGN TextureSignZ => (GPUSIGN)((value >> 13) & 0x3u);

        /// <summary>
        /// Describes whether the sign of individual texel elements is unsigned, signed, unsigned biased, or unsigned gamma corrected (for select data formats).
        /// </summary>
        public GPUSIGN TextureSignW => (GPUSIGN)((value >> 15) & 0x3u);

        /// <summary>
        /// Describes whether the number format is a fraction or an integer.
        /// </summary>
        public GPUNUMFORMAT NumFormat => (GPUNUMFORMAT)((value >> 17) & 0x1u);

        /// <summary>
        /// Describes how components of a texel are mapped to the elements of a register into which that texel is fetched.
        /// </summary>
        public GPUSWIZZLE SwizzleX => (GPUSWIZZLE)((value >> 18) & 0x7u);

        /// <summary>
        /// Describes how components of a texel are mapped to the elements of a register into which that texel is fetched.
        /// </summary>
        public GPUSWIZZLE SwizzleY => (GPUSWIZZLE)((value >> 21) & 0x7u);

        /// <summary>
        /// Describes how components of a texel are mapped to the elements of a register into which that texel is fetched.
        /// </summary>
        public GPUSWIZZLE SwizzleZ => (GPUSWIZZLE)((value >> 24) & 0x7u);

        /// <summary>
        /// Describes how components of a texel are mapped to the elements of a register into which that texel is fetched.
        /// </summary>
        public GPUSWIZZLE SwizzleW => (GPUSWIZZLE)((value >> 27) & 0x7u);

        private D3DFORMAT(uint value)
        {
            this.value = value;
        }

        /// <summary>
        /// MAKED3DFMT(TextureFormat, Endian, Tiled, TextureSign, NumFormat, Swizzle)
        /// </summary>
        /// <param name="TextureFormat">Member of the GPUTEXTUREFORMAT enumeration.</param>
        /// <param name="Endian">Member of the GPUENDIAN enumeration.</param>
        /// <param name="Tiled">Boolean. Pass TRUE for a tiled format; otherwise, pass FALSE.</param>
        /// <param name="TextureSign">Member of the GPUSIGN enumeration.</param>
        /// <param name="NumFormat">Member of the GPUNUMFORMAT enumeration.</param>
        /// <param name="Swizzle">Bit mask that describes how texture channels are mapped during a texture fetch. The bit mask consists of four members of the GPUSWIZZLE enumeration.</param>
        public D3DFORMAT(GPUTEXTUREFORMAT TextureFormat, GPUENDIAN Endian, bool Tiled, GPUSIGN TextureSign, GPUNUMFORMAT NumFormat, GPUSWIZZLE Swizzle)
        {
            value = ((((uint)TextureFormat & 0x3Fu) << 0)
                | (((uint)Endian & 0x3u) << 6)
                | ((Tiled ? 1u : 0u) << 8)
                | (((uint)TextureSign & 0xFFu) << 9)
                | (((uint)NumFormat & 0x1u) << 17)
                | (((uint)Swizzle & 0xFFFu) << 18));
        }

        /// <summary>
        /// MAKED3DFMT2(TextureFormat, Endian, Tiled, TextureSignX, TextureSignY, TextureSignZ, TextureSignW, NumFormat, SwizzleX, SwizzleY, SwizzleZ, SwizzleW)
        /// </summary>
        /// <param name="TextureFormat">A member of the GPUTEXTUREFORMAT enumeration.</param>
        /// <param name="Endian">A member of the GPUENDIAN enumeration.</param>
        /// <param name="Tiled">Pass TRUE for a tiled format; otherwise pass FALSE.</param>
        /// <param name="TextureSignX">A member of the GPUSIGN enumeration.</param>
        /// <param name="TextureSignY">A member of the GPUSIGN enumeration.</param>
        /// <param name="TextureSignZ">A member of the GPUSIGN enumeration.</param>
        /// <param name="TextureSignW">A member of the GPUSIGN enumeration.</param>
        /// <param name="NumFormat">A member of the GPUNUMFORMAT enumeration.</param>
        /// <param name="SwizzleX">A member of the GPUSWIZZLE enumeration.</param>
        /// <param name="SwizzleY">A member of the GPUSWIZZLE enumeration.</param>
        /// <param name="SwizzleZ">A member of the GPUSWIZZLE enumeration.</param>
        /// <param name="SwizzleW">A member of the GPUSWIZZLE enumeration.</param>
        public D3DFORMAT(GPUTEXTUREFORMAT TextureFormat, GPUENDIAN Endian, bool Tiled, GPUSIGN TextureSignX, GPUSIGN TextureSignY, GPUSIGN TextureSignZ, GPUSIGN TextureSignW, GPUNUMFORMAT NumFormat, GPUSWIZZLE SwizzleX, GPUSWIZZLE SwizzleY, GPUSWIZZLE SwizzleZ, GPUSWIZZLE SwizzleW)
        {
            value = (((uint)TextureFormat & 0x3Fu) << 0)
                | (((uint)Endian & 0x3u) << 6)
                | ((Tiled ? 1u : 0u) << 8)
                | (((uint)TextureSignX & 0x3u) << 9)
                | (((uint)TextureSignY & 0x3u) << 11)
                | (((uint)TextureSignZ & 0x3u) << 13)
                | (((uint)TextureSignW & 0x3u) << 15)
                | (((uint)NumFormat & 0x1u) << 17)
                | (((uint)SwizzleX & 0x7u) << 18)
                | (((uint)SwizzleY & 0x7u) << 21)
                | (((uint)SwizzleZ & 0x7u) << 24)
                | (((uint)SwizzleW & 0x7u) << 27);
        }

        /// <summary>
        /// MAKEINDEXFMT(Is32Bits, Endian)
        /// </summary>
        /// <param name="Is32Bits"></param>
        /// <param name="Endian"></param>
        public D3DFORMAT(bool Is32Bits, GPUENDIAN Endian)
        {
            value = (Is32Bits ? 1u : 0u) << 2 | ((uint)Endian) << 0;
        }

        /// <summary>
        /// MAKELEFMT(D3dFmt) (((D3dFmt) & ~D3DFORMAT_ENDIAN_MASK) | (GPUENDIAN_NONE << D3DFORMAT_ENDIAN_SHIFT))
        /// </summary>
        /// <returns>Little endian format</returns>
        public readonly D3DFORMAT GetLEFormat()
        {
            return new D3DFORMAT(((value) & ~0x000000C0u) | ((uint)GPUENDIAN.GPUENDIAN_NONE << 6));
        }

        /// <summary>
        /// MAKELINFMT(D3dFmt) ((D3dFmt) & ~D3DFORMAT_TILED_MASK)
        /// </summary>
        /// <returns>Linear format</returns>
        public readonly D3DFORMAT GetLinearFormat()
        {
            return new D3DFORMAT((value) & ~0x00000100u);
        }

        /// <summary>
        /// MAKESRGBFMT(D3dFmt)
        /// </summary>
        /// <returns>sRGB format</returns>
        public readonly D3DFORMAT GetSrgbFormat()
        {
            return new D3DFORMAT(((value) & ~(0x00000600u | 0x00001800u | 0x00006000u))
                | ((uint)GPUSIGN.GPUSIGN_GAMMA << 9)
                | ((uint)GPUSIGN.GPUSIGN_GAMMA << 11)
                | ((uint)GPUSIGN.GPUSIGN_GAMMA << 13));
        }

        public readonly override bool Equals(object? obj)
        {
            return (obj is D3DFORMAT fmt) && value == fmt.value;
        }

        public readonly bool Equals(D3DFORMAT other)
        {
            return value == other.value;
        }

        public readonly override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }

        public static bool operator ==(D3DFORMAT fmt1, D3DFORMAT fmt2)
        {
            return fmt1.value == fmt2.value;
        }

        public static bool operator !=(D3DFORMAT fmt1, D3DFORMAT fmt2)
        {
            return fmt1.value != fmt2.value;
        }

        public static implicit operator uint(D3DFORMAT fmt)
        {
            return fmt.value;
        }

        public static implicit operator D3DFORMAT(uint fmt)
        {
            return new D3DFORMAT(fmt);
        }

        // DXT:

        public static readonly D3DFORMAT D3DFMT_DXT1 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT1, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXT1 = D3DFMT_DXT1.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT2 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT2_3, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXT2 = D3DFMT_DXT2.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT3 = D3DFMT_DXT2;
        public static readonly D3DFORMAT D3DFMT_LIN_DXT3 = D3DFMT_DXT3.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT3A = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT3A, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXT3A = D3DFMT_DXT3A.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT3A_1111 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT3A_AS_1_1_1_1, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXT3A_1111 = D3DFMT_DXT3A_1111.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT4 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT4_5, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXT4 = D3DFMT_DXT4.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT5 = D3DFMT_DXT4;
        public static readonly D3DFORMAT D3DFMT_LIN_DXT5 = D3DFMT_DXT5.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXT5A = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXT5A, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXT5A = D3DFMT_DXT5A.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_DXN = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_DXN, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_DXN = D3DFMT_DXN.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_CTX1 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_CTX1, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_CTX1 = D3DFMT_CTX1.GetLinearFormat();

        // 8bpp:

        public static readonly D3DFORMAT D3DFMT_A8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_RZZZ);
        public static readonly D3DFORMAT D3DFMT_LIN_A8 = D3DFMT_A8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_L8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORRR);
        public static readonly D3DFORMAT D3DFMT_LIN_L8 = D3DFMT_L8.GetLinearFormat();

        // 16bpp:

        public static readonly D3DFORMAT D3DFMT_R5G6B5 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_5_6_5, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_R5G6B5 = D3DFMT_R5G6B5.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R6G5B5 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_6_5_5, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_R6G5B5 = D3DFMT_R6G5B5.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_L6V5U5 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_6_5_5, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_UNSIGNED, GPUSIGN.GPUSIGN_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_X, GPUSWIZZLE.GPUSWIZZLE_Y, GPUSWIZZLE.GPUSWIZZLE_Z, GPUSWIZZLE.GPUSWIZZLE_1);
        public static readonly D3DFORMAT D3DFMT_LIN_L6V5U5 = D3DFMT_L6V5U5.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_X1R5G5B5 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_1_5_5_5, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_X1R5G5B5 = D3DFMT_X1R5G5B5.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A1R5G5B5 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_1_5_5_5, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LIN_A1R5G5B5 = D3DFMT_A1R5G5B5.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A4R4G4B4 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_4_4_4_4, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LIN_A4R4G4B4 = D3DFMT_A4R4G4B4.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_X4R4G4B4 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_4_4_4_4, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_X4R4G4B4 = D3DFMT_X4R4G4B4.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_Q4W4V4U4 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_4_4_4_4, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_Q4W4V4U4 = D3DFMT_Q4W4V4U4.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A8L8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_GRRR);
        public static readonly D3DFORMAT D3DFMT_LIN_A8L8 = D3DFMT_A8L8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G8R8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_G8R8 = D3DFMT_G8R8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_V8U8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_V8U8 = D3DFMT_V8U8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_D16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_D16 = D3DFMT_D16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_L16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORRR);
        public static readonly D3DFORMAT D3DFMT_LIN_L16 = D3DFMT_L16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R16F = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_FLOAT, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_OOOR);
        public static readonly D3DFORMAT D3DFMT_LIN_R16F = D3DFMT_R16F.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R16F_EXPAND = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_EXPAND, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_OOOR);
        public static readonly D3DFORMAT D3DFMT_LIN_R16F_EXPAND = D3DFMT_R16F_EXPAND.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_UYVY = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_Y1_Cr_Y0_Cb_REP, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_UYVY = D3DFMT_UYVY.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_LE_UYVY = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_Y1_Cr_Y0_Cb_REP, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LE_LIN_UYVY = D3DFMT_LE_UYVY.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G8R8_G8B8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_Y1_Cr_Y0_Cb_REP, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LIN_G8R8_G8B8 = D3DFMT_G8R8_G8B8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R8G8_B8G8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_Cr_Y1_Cb_Y0_REP, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LIN_R8G8_B8G8 = D3DFMT_R8G8_B8G8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_YUY2 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_Cr_Y1_Cb_Y0_REP, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_YUY2 = D3DFMT_YUY2.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_LE_YUY2 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_Cr_Y1_Cb_Y0_REP, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LE_LIN_YUY2 = D3DFMT_LE_YUY2.GetLinearFormat();

        // 32bpp:

        public static readonly D3DFORMAT D3DFMT_A8R8G8B8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LIN_A8R8G8B8 = D3DFMT_A8R8G8B8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_X8R8G8B8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_X8R8G8B8 = D3DFMT_X8R8G8B8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A8B8G8R8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A8B8G8R8 = D3DFMT_A8B8G8R8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_X8B8G8R8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OBGR);
        public static readonly D3DFORMAT D3DFMT_LIN_X8B8G8R8 = D3DFMT_X8B8G8R8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_X8L8V8U8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_UNSIGNED, GPUSIGN.GPUSIGN_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_X, GPUSWIZZLE.GPUSWIZZLE_Y, GPUSWIZZLE.GPUSWIZZLE_Z, GPUSWIZZLE.GPUSWIZZLE_1);
        public static readonly D3DFORMAT D3DFMT_LIN_X8L8V8U8 = D3DFMT_X8L8V8U8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_Q8W8V8U8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_Q8W8V8U8 = D3DFMT_Q8W8V8U8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A2R10G10B10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LIN_A2R10G10B10 = D3DFMT_A2R10G10B10.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_X2R10G10B10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_X2R10G10B10 = D3DFMT_X2R10G10B10.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A2B10G10R10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A2B10G10R10 = D3DFMT_A2B10G10R10.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A2W10V10U10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_SIGNED, GPUSIGN.GPUSIGN_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_X, GPUSWIZZLE.GPUSWIZZLE_Y, GPUSWIZZLE.GPUSWIZZLE_Z, GPUSWIZZLE.GPUSWIZZLE_W);
        public static readonly D3DFORMAT D3DFMT_LIN_A2W10V10U10 = D3DFMT_A2W10V10U10.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A16L16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_GRRR);
        public static readonly D3DFORMAT D3DFMT_LIN_A16L16 = D3DFMT_A16L16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G16R16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_G16R16 = D3DFMT_G16R16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_V16U16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_V16U16 = D3DFMT_V16U16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R10G11B11 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_10_11_11_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_R10G11B11 = D3DFMT_R10G11B11.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R11G11B10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_11_11_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LIN_R11G11B10 = D3DFMT_R11G11B10.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_W10V11U11 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_10_11_11_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OBGR);
        public static readonly D3DFORMAT D3DFMT_LIN_W10V11U11 = D3DFMT_W10V11U11.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_W11V11U10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_11_11_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OBGR);
        public static readonly D3DFORMAT D3DFMT_LIN_W11V11U10 = D3DFMT_W11V11U10.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G16R16F = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_FLOAT, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_G16R16F = D3DFMT_G16R16F.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G16R16F_EXPAND = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_EXPAND, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_G16R16F_EXPAND = D3DFMT_G16R16F_EXPAND.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_L32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORRR);
        public static readonly D3DFORMAT D3DFMT_LIN_L32 = D3DFMT_L32.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_R32F = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_FLOAT, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_OOOR);
        public static readonly D3DFORMAT D3DFMT_LIN_R32F = D3DFMT_R32F.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_D24S8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_24_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_D24S8 = D3DFMT_D24S8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_D24X8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_24_8, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOOR);
        public static readonly D3DFORMAT D3DFMT_LIN_D24X8 = D3DFMT_D24X8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_D24FS8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_24_8_FLOAT, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_D24FS8 = D3DFMT_D24FS8.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_D32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_D32 = D3DFMT_D32.GetLinearFormat();

        // 64bpp:

        public static readonly D3DFORMAT D3DFMT_A16B16G16R16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A16B16G16R16 = D3DFMT_A16B16G16R16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_Q16W16V16U16 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_16_16, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_Q16W16V16U16 = D3DFMT_Q16W16V16U16.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A16B16G16R16F = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_16_16_FLOAT, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A16B16G16R16F = D3DFMT_A16B16G16R16F.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A16B16G16R16F_EXPAND = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_16_16_EXPAND, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A16B16G16R16F_EXPAND = D3DFMT_A16B16G16R16F_EXPAND.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A32L32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_GRRR);
        public static readonly D3DFORMAT D3DFMT_LIN_A32L32 = D3DFMT_A32L32.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G32R32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_G32R32 = D3DFMT_G32R32.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_V32U32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_V32U32 = D3DFMT_V32U32.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_G32R32F = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32_FLOAT, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_LIN_G32R32F = D3DFMT_G32R32F.GetLinearFormat();

        // 128bpp:

        public static readonly D3DFORMAT D3DFMT_A32B32G32R32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32_32_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A32B32G32R32 = D3DFMT_A32B32G32R32.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_Q32W32V32U32 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32_32_32, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_Q32W32V32U32 = D3DFMT_Q32W32V32U32.GetLinearFormat();

        public static readonly D3DFORMAT D3DFMT_A32B32G32R32F = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_32_32_32_32_FLOAT, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_LIN_A32B32G32R32F = D3DFMT_A32B32G32R32F.GetLinearFormat();

        // EDRAM only:

        public static readonly D3DFORMAT D3DFMT_A2B10G10R10F_EDRAM = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_FLOAT_EDRAM, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_INTEGER, GPUSWIZZLE.GPUSWIZZLE_ABGR);
        public static readonly D3DFORMAT D3DFMT_G16R16_EDRAM = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_EDRAM, GPUENDIAN.GPUENDIAN_8IN32, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_OOGR);
        public static readonly D3DFORMAT D3DFMT_A16B16G16R16_EDRAM = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_16_16_16_16_EDRAM, GPUENDIAN.GPUENDIAN_8IN16, true, GPUSIGN.GPUSIGN_ALL_SIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ABGR);

        // Front buffer formats have to be little endian:

        public static readonly D3DFORMAT D3DFMT_LE_X8R8G8B8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LE_A8R8G8B8 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_8_8_8_8, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);
        public static readonly D3DFORMAT D3DFMT_LE_X2R10G10B10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ORGB);
        public static readonly D3DFORMAT D3DFMT_LE_A2R10G10B10 = new D3DFORMAT(GPUTEXTUREFORMAT.GPUTEXTUREFORMAT_2_10_10_10_AS_16_16_16_16, GPUENDIAN.GPUENDIAN_NONE, true, GPUSIGN.GPUSIGN_ALL_UNSIGNED, GPUNUMFORMAT.GPUNUMFORMAT_FRACTION, GPUSWIZZLE.GPUSWIZZLE_ARGB);

        // Other:

        public static readonly D3DFORMAT D3DFMT_INDEX16 = new D3DFORMAT(false, GPUENDIAN.GPUENDIAN_8IN16);
        public static readonly D3DFORMAT D3DFMT_INDEX32 = new D3DFORMAT(true, GPUENDIAN.GPUENDIAN_8IN32);
        public static readonly D3DFORMAT D3DFMT_LE_INDEX16 = new D3DFORMAT(false, GPUENDIAN.GPUENDIAN_NONE);
        public static readonly D3DFORMAT D3DFMT_LE_INDEX32 = new D3DFORMAT(true, GPUENDIAN.GPUENDIAN_NONE);

        public static readonly D3DFORMAT D3DFMT_VERTEXDATA = new D3DFORMAT(8u);

        public static readonly D3DFORMAT D3DFMT_UNKNOWN = new D3DFORMAT(0xFFFFFFFFu);

        // The following are not supported on Xbox 360:
        //
        // D3DFMT_A8R3G3B2
        // D3DFMT_R3G3B2
        // D3DFMT_S1D15
        // D3DFMT_S8D24
        // D3DFMT_X8D24
        // D3DFMT_X4S4D24
        // D3DFMT_A8P8
        // D3DFMT_P8
        // D3DFMT_A4L4
        // D3DFMT_R8G8B8
        // D3DFMT_D16_LOCKABLE
        // D3DFMT_D15S1
        // D3DFMT_D24X4S4
        // D3DFMT_D32F_LOCKABLE

        public static readonly D3DFORMAT D3DFMT_FORCE_DWORD = new D3DFORMAT(0x7FFFFFFFu);
    }
}