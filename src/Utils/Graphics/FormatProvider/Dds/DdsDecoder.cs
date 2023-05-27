using LibWindPop.Utils.Extension;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using System;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    internal static unsafe class DdsDecoder
    {
        internal const uint Magic = 0x20534444u;

        public static void DecodeDds(Stream stream, RefBitmap bitmap)
        {
            if (CheckDdsMagic(stream))
            {
                DDS_HEADER header;
                stream.Read(&header, (nuint)sizeof(DDS_HEADER));
                IPitchableTextureCoder? pitchableCoder = null;
                uint memSize = 0u;
                bool preMultiplyAlpha = false;
                bool sRGB = false;
                bool pitchable;
                if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_FOURCC) != 0)
                {
                    uint fourCC = header.ddspf.dwFourCC;
                    if (fourCC == DDS_PIXELFORMAT.DX10)
                    {
                        // dx10
                        DDS_HEADER_DXT10 dx10Header;
                        stream.Read(&dx10Header, (uint)sizeof(DDS_HEADER_DXT10));
                        switch (dx10Header.dxgiFormat)
                        {
                            case DXGI_FORMAT.DXGI_FORMAT_R8_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_R8_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_R8_UINT:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                                pitchableCoder = new L8_UByte();
                                memSize = pitchable
                                    ? (header.dwPitchOrLinearSize * Align4(header.dwHeight))
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight));
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_A8_UNORM:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                                pitchableCoder = new A8_UByte();
                                memSize = pitchable
                                    ? (header.dwPitchOrLinearSize * Align4(header.dwHeight))
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight));
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
                            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UINT:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                                pitchableCoder = new R8_G8_B8_A8_UByte();
                                memSize = pitchable
                                    ? (header.dwPitchOrLinearSize * Align4(header.dwHeight))
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) * 4);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                                pitchableCoder = new B8_G8_R8_A8_UByte();
                                memSize = pitchable
                                    ? (header.dwPitchOrLinearSize * Align4(header.dwHeight))
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) * 4);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                                pitchableCoder = new B8_G8_R8_X8_UByte();
                                memSize = pitchable
                                    ? (header.dwPitchOrLinearSize * Align4(header.dwHeight))
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) * 4);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new RGBA_BC1_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) / 2);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new RGBA_BC2_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight));
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new RGBA_BC3_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight));
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new RGB_BC6U_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) * 2);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new RGB_BC6S_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) * 2);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
                            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                                sRGB = dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB;
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new RGBA_BC7_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) * 2);
                                break;
                        }
                    }
                    else
                    {
                        pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                        if (fourCC == DDS_PIXELFORMAT.DXT1)
                        {
                            pitchableCoder = new RGBA_BC1_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight) / 2);
                        }
                        else if (fourCC == DDS_PIXELFORMAT.DXT2 || fourCC == DDS_PIXELFORMAT.DXT3)
                        {
                            pitchableCoder = new RGBA_BC2_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                            preMultiplyAlpha = fourCC == DDS_PIXELFORMAT.DXT2;
                        }
                        else if (fourCC == DDS_PIXELFORMAT.DXT4 || fourCC == DDS_PIXELFORMAT.DXT5)
                        {
                            pitchableCoder = new RGBA_BC3_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                            preMultiplyAlpha = fourCC == DDS_PIXELFORMAT.DXT4;
                        }
                        else if (fourCC == DDS_PIXELFORMAT.ATI1 || fourCC == DDS_PIXELFORMAT.BC4S || fourCC == DDS_PIXELFORMAT.BC4U)
                        {
                            throw new NotImplementedException();
                        }
                        else if (fourCC == DDS_PIXELFORMAT.ATI2 || fourCC == DDS_PIXELFORMAT.BC5S || fourCC == DDS_PIXELFORMAT.BC5U)
                        {
                            throw new NotImplementedException();
                        }
                        else if (fourCC == DDS_PIXELFORMAT.ATC_)
                        {
                            pitchableCoder = new RGB_ATC_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight) / 2);
                        }
                        else if (fourCC == DDS_PIXELFORMAT.ATCI)
                        {
                            pitchableCoder = new RGBA_ATC_Explicit_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                        }
                        else if (fourCC == DDS_PIXELFORMAT.ATCA)
                        {
                            pitchableCoder = new RGBA_ATC_Interpolated_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                        }
                    }
                }
                else
                {
                    pitchable = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                    if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_RGB) != 0u)
                    {
                        if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_ALPHAPIXELS) != 0u)
                        {
                            if (header.ddspf.dwRGBBitCount == 32u)
                            {
                                if (header.ddspf.dwRBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwBBitMask == 0xFF0000u
                                    && header.ddspf.dwABitMask == 0xFF000000u) // RRGGBBAARRGGBBAA...
                                {
                                    pitchableCoder = new R8_G8_B8_A8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                                else if (header.ddspf.dwBBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwRBitMask == 0xFF0000u
                                    && header.ddspf.dwABitMask == 0xFF000000u) // BBGGRRAABBGGRRAA...
                                {
                                    pitchableCoder = new B8_G8_R8_A8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                            }
                        }
                        else
                        {
                            if (header.ddspf.dwRGBBitCount == 32u)
                            {
                                if (header.ddspf.dwRBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwBBitMask == 0xFF0000u) // RRGGBBFFBBGGRRFF...
                                {
                                    pitchableCoder = new R8_G8_B8_X8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                                else if (header.ddspf.dwBBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwRBitMask == 0xFF0000u) // BBGGRRFFBBGGRRFF...
                                {
                                    pitchableCoder = new B8_G8_R8_X8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                            }
                            else if (header.ddspf.dwRGBBitCount == 24u)
                            {
                                if (header.ddspf.dwRBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwBBitMask == 0xFF0000u) // RRGGBBRRGGBB...
                                {
                                    pitchableCoder = new R8_G8_B8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 3);
                                }
                                else if (header.ddspf.dwRBitMask == 0xFF0000u
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwBBitMask == 0xFFu) // BBGGRRBBGGRR...
                                {
                                    pitchableCoder = new B8_G8_R8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 3);
                                }
                            }
                            else if (header.ddspf.dwRGBBitCount == 8u)
                            {
                                if (header.ddspf.dwRBitMask == 0xFFu) // RRRR...
                                {
                                    pitchableCoder = new L8_UByte();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight);
                                }
                            }
                        }
                    }
                    else if ((header.ddspf.dwFlags | DDS_PIXELFORMAT.DDPF_ALPHA) != 0u)
                    {
                        if (header.ddspf.dwRGBBitCount == 8u)
                        {
                            if (header.ddspf.dwABitMask == 0xFFu) // AAAA...
                            {
                                pitchableCoder = new A8_UByte();
                                memSize = pitchable
                                    ? (header.dwPitchOrLinearSize * header.dwHeight)
                                    : (header.dwWidth * header.dwHeight);
                            }
                        }
                    }
                }
                if (pitchableCoder != null)
                {
                    using (NativeMemoryOwner owner = new NativeMemoryOwner(memSize))
                    {
                        stream.Read(owner.Pointer, memSize);
                        ReadOnlySpan<byte> dataSpan = owner.AsSpan();
                        if ((header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u)
                        {
                            pitchableCoder.Decode(dataSpan, (int)header.dwWidth, (int)header.dwHeight, (int)header.dwPitchOrLinearSize, bitmap);
                        }
                        else
                        {
                            pitchableCoder.Decode(dataSpan, (int)header.dwWidth, (int)header.dwHeight, bitmap);
                        }
                    }
                    if (preMultiplyAlpha)
                    {
                        int h = (int)header.dwHeight;
                        int w = (int)header.dwWidth;
                        for (int y = 0; y < h; y++)
                        {
                            Span<YFColor> row = bitmap[y];
                            for (int x = 0; x < w; x++)
                            {
                                ref YFColor color = ref row[x];
                                if (color.Alpha != 0)
                                {
                                    color.Red = (byte)Math.Clamp((color.Red << 8) / color.Alpha, 0, 255);
                                    color.Green = (byte)Math.Clamp((color.Green << 8) / color.Alpha, 0, 255);
                                    color.Blue = (byte)Math.Clamp((color.Blue << 8) / color.Alpha, 0, 255);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static uint Align4(uint value)
        {
            if ((value & 3u) != 0u)
            {
                return (value | 3u) + 1u;
            }
            return value;
        }

        public static bool IsDds(Stream stream)
        {
            long pos = stream.Position;
            uint magic = stream.ReadUInt32LE();
            stream.Seek(pos, SeekOrigin.Begin);
            return magic == Magic;
        }

        public static bool PeekDdsWidthHeight(Stream stream, out uint width, out uint height)
        {
            long pos = stream.Position;
            if (CheckDdsMagic(stream))
            {
                DDS_HEADER header;
                stream.Read(&header, (nuint)sizeof(DDS_HEADER));
                if (header.dwSize == (uint)sizeof(DDS_HEADER))
                {
                    width = header.dwWidth;
                    height = header.dwHeight;
                    stream.Seek(pos, SeekOrigin.Begin);
                    return true;
                }
            }
            width = 0u;
            height = 0u;
            stream.Seek(pos, SeekOrigin.Begin);
            return false;
        }

        private static bool CheckDdsMagic(Stream stream)
        {
            return stream.ReadUInt32LE() == Magic;
        }
    }
}
