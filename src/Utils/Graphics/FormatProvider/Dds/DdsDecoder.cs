using LibWindPop.Utils.Extension;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using System;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    internal static unsafe class DdsDecoder
    {
        private const uint Magic = 0x20534444u;

        public static void DecodeDds(Stream stream, RefBitmap bitmap)
        {
            if (CheckDdsMagic(stream))
            {
                DDS_HEADER header;
                stream.Read(&header, (nuint)sizeof(DDS_HEADER));
                IPitchableTextureCoder? pitchableCoder = null;
                uint memSize = 0u;
                bool preMultiplyAlpha = false;
                bool pitchable = false;
                if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_FOURCC) != 0)
                {
                    uint fourCC = header.ddspf.dwFourCC;
                    if (fourCC == DDS_PIXELFORMAT.Dx10)
                    {
                        // dx10
                        DDS_HEADER_DXT10 dx10Header;
                        stream.Read(&dx10Header, (uint)sizeof(DDS_HEADER_DXT10));
                        switch (dx10Header.dxgiFormat)
                        {
                            case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new R8_G8_B8_A1_DXT1_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight) / 2);
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new R8_G8_B8_A4_DXT3_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight));
                                break;
                            case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
                            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                                pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                                pitchableCoder = new R8_G8_B8_A8_DXT5_UByte();
                                memSize = pitchable
                                    ? header.dwPitchOrLinearSize
                                    : (Align4(header.dwWidth) * Align4(header.dwHeight));
                                break;
                        }
                    }
                    else
                    {
                        pitchable = (header.dwFlags & DDS_HEADER.DDSD_LINEARSIZE) != 0u;
                        if (fourCC == DDS_PIXELFORMAT.Dxt1)
                        {
                            pitchableCoder = new R8_G8_B8_A1_DXT1_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight) / 2);
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Dxt2 || fourCC == DDS_PIXELFORMAT.Dxt3)
                        {
                            pitchableCoder = new R8_G8_B8_A4_DXT3_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                            preMultiplyAlpha = fourCC == DDS_PIXELFORMAT.Dxt2;
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Dxt4 || fourCC == DDS_PIXELFORMAT.Dxt5)
                        {
                            pitchableCoder = new R8_G8_B8_A8_DXT5_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                            preMultiplyAlpha = fourCC == DDS_PIXELFORMAT.Dxt4;
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Ati1 || fourCC == DDS_PIXELFORMAT.Bc4S || fourCC == DDS_PIXELFORMAT.Bc4U)
                        {
                            throw new NotImplementedException();
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Ati2 || fourCC == DDS_PIXELFORMAT.Bc5S || fourCC == DDS_PIXELFORMAT.Bc5U)
                        {
                            throw new NotImplementedException();
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Atc)
                        {
                            pitchableCoder = new R8_G8_B8_ATC_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight) / 2);
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Atci)
                        {
                            pitchableCoder = new R8_G8_B8_A4_ATC_Explicit_UByte();
                            memSize = pitchable
                                ? header.dwPitchOrLinearSize
                                : (Align4(header.dwWidth) * Align4(header.dwHeight));
                        }
                        else if (fourCC == DDS_PIXELFORMAT.Atca)
                        {
                            pitchableCoder = new R8_G8_B8_A8_ATC_Interpolated_UByte();
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
                            // RGBA
                            if (header.ddspf.dwRGBBitCount == 32u)
                            {
                                if (header.ddspf.dwRBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwBBitMask == 0xFF0000u
                                    && header.ddspf.dwABitMask == 0xFF000000u)
                                {
                                    pitchableCoder = new A8_B8_G8_R8_UInt();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                                else if (header.ddspf.dwBBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwRBitMask == 0xFF0000u
                                    && header.ddspf.dwABitMask == 0xFF000000u)
                                {
                                    pitchableCoder = new A8_R8_G8_B8_UInt();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                            }
                        }
                        else
                        {
                            // RGB
                            if (header.ddspf.dwRGBBitCount == 32u)
                            {
                                if (header.ddspf.dwRBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwBBitMask == 0xFF0000u)
                                {
                                    pitchableCoder = new X8_B8_G8_R8_UInt();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
                                else if (header.ddspf.dwBBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwRBitMask == 0xFF0000u)
                                {
                                    pitchableCoder = new X8_R8_G8_B8_UInt();
                                    memSize = pitchable
                                        ? (header.dwPitchOrLinearSize * header.dwHeight)
                                        : (header.dwWidth * header.dwHeight * 4);
                                }
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
