using LibWindPop.Utils.Extension;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using System;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Dds
{
    internal static unsafe class DdsEncoder
    {
        public static void EncodeDds(Stream stream, RefBitmap bitmap, IImageEncoderArgument? args)
        {
            DDS_HEADER header = new DDS_HEADER();
            header.dwSize = (uint)sizeof(DDS_HEADER);
            header.dwFlags = DDS_HEADER.DDSD_CAPS | DDS_HEADER.DDSD_WIDTH | DDS_HEADER.DDSD_HEIGHT | DDS_HEADER.DDSD_PIXELFORMAT;
            header.dwHeight = (uint)bitmap.Height;
            header.dwWidth = (uint)bitmap.Width;
            header.dwPitchOrLinearSize = 0u;
            header.dwDepth = 1u;
            header.dwMipMapCount = 1u;
            header.dwReserved1[0] = 0u;
            header.dwReserved1[1] = 0u;
            header.dwReserved1[2] = 0u;
            header.dwReserved1[3] = 0u;
            header.dwReserved1[4] = 0u;
            header.dwReserved1[5] = 0u;
            header.dwReserved1[6] = 0u;
            header.dwReserved1[7] = 0u;
            header.dwReserved1[8] = 0u;
            header.dwReserved1[9] = 861165134u;
            header.dwReserved1[10] = 1u;
            header.ddspf.dwSize = (uint)sizeof(DDS_PIXELFORMAT);
            header.ddspf.dwFlags = 0u;
            header.ddspf.dwFourCC = 0u;
            header.ddspf.dwRGBBitCount = 0u;
            header.ddspf.dwRBitMask = 0u;
            header.ddspf.dwGBitMask = 0u;
            header.ddspf.dwBBitMask = 0u;
            header.ddspf.dwABitMask = 0u;
            header.dwCaps = DDS_HEADER.DDSCAPS_TEXTURE;
            header.dwCaps2 = 0u;
            header.dwCaps3 = 0u;
            header.dwCaps4 = 0u;
            header.dwReserved2 = 0u;
            if (args is DdsEncoderArgument ddsArgs)
            {
                if (ddsArgs.UseDX10Header)
                {
                    DDS_HEADER_DXT10 headerDx10 = new DDS_HEADER_DXT10();
                    header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                    header.ddspf.dwFourCC = DDS_PIXELFORMAT.DX10;
                    if (ddsArgs.Format == DdsEncodingFormat.None)
                    {
                        header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                        header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                        headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                        headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                        headerDx10.miscFlag = 0u;
                        headerDx10.arraySize = 1u;
                        headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                        WriteMagic(stream);
                        WriteDdsHeader(stream, &header);
                        WriteDx10Header(stream, &headerDx10);
                        int w = bitmap.Width;
                        int h = bitmap.Height;
                        nuint pitch = (uint)bitmap.Width * 4u;
                        for (int y = 0; y < h; y++)
                        {
                            fixed (YFColor* rowPtr = bitmap[y])
                            {
                                stream.Write(rowPtr, pitch);
                            }
                        }
                    }
                    else
                    {
                        IPitchableTextureCoder coder;
                        if (ddsArgs.Format == DdsEncodingFormat.L8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_OPAQUE;
                            coder = new L8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.A8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_A8_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                            coder = new A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.R8_G8_B8_A8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                            coder = new R8_G8_B8_A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.B8_G8_R8_A8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                            coder = new B8_G8_R8_A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.B8_G8_R8_X8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_OPAQUE;
                            coder = new B8_G8_R8_A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGB_BC1_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap) / 2;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_OPAQUE;
                            coder = new RGB_BC1_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_BC1_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap) / 2;
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                            coder = new RGBA_BC1_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_BC2_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap);
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                            coder = new RGBA_BC2_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_BC3_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap);
                            headerDx10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                            headerDx10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                            headerDx10.miscFlag = 0u;
                            headerDx10.arraySize = 1u;
                            headerDx10.miscFlags2 = DDS_HEADER_DXT10.DDS_ALPHA_MODE_STRAIGHT;
                            coder = new RGBA_BC3_UByte();
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        bool pitch = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                        uint memSize = pitch ? header.dwPitchOrLinearSize * header.dwHeight : header.dwPitchOrLinearSize;
                        WriteMagic(stream);
                        WriteDdsHeader(stream, &header);
                        WriteDx10Header(stream, &headerDx10);
                        using (NativeMemoryOwner owner = new NativeMemoryOwner(memSize))
                        {
                            Span<byte> dataSpan = owner.AsSpan();
                            if (pitch)
                            {
                                coder.Encode(bitmap, dataSpan, (int)header.dwWidth, (int)header.dwHeight, (int)header.dwPitchOrLinearSize);
                            }
                            else
                            {
                                coder.Encode(bitmap, dataSpan, (int)header.dwWidth, (int)header.dwHeight);
                            }
                            stream.Write(owner.Pointer, owner.Size);
                        }
                    }
                }
                else
                {
                    if (ddsArgs.Format == DdsEncodingFormat.None)
                    {
                        header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                        header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                        header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB | DDS_PIXELFORMAT.DDPF_ALPHAPIXELS;
                        header.ddspf.dwRGBBitCount = 32u;
                        header.ddspf.dwRBitMask = 0xFF0000u;
                        header.ddspf.dwGBitMask = 0xFF00u;
                        header.ddspf.dwBBitMask = 0xFFu;
                        header.ddspf.dwABitMask = 0xFF000000u;
                        WriteMagic(stream);
                        WriteDdsHeader(stream, &header);
                        int w = bitmap.Width;
                        int h = bitmap.Height;
                        nuint pitch = (uint)bitmap.Width * 4u;
                        for (int y = 0; y < h; y++)
                        {
                            fixed (YFColor* rowPtr = bitmap[y])
                            {
                                stream.Write(rowPtr, pitch);
                            }
                        }
                    }
                    else
                    {
                        IPitchableTextureCoder coder;
                        if (ddsArgs.Format == DdsEncodingFormat.L8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB;
                            header.ddspf.dwRGBBitCount = 8u;
                            header.ddspf.dwRBitMask = 0xFFu;
                            coder = new L8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.A8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_ALPHA;
                            header.ddspf.dwRGBBitCount = 8u;
                            header.ddspf.dwABitMask = 0xFFu;
                            coder = new A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.R8_G8_B8_A8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB | DDS_PIXELFORMAT.DDPF_ALPHAPIXELS;
                            header.ddspf.dwRGBBitCount = 32u;
                            header.ddspf.dwRBitMask = 0xFFu;
                            header.ddspf.dwGBitMask = 0xFF00u;
                            header.ddspf.dwBBitMask = 0xFF0000u;
                            header.ddspf.dwABitMask = 0xFF000000u;
                            coder = new R8_G8_B8_A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.R8_G8_B8_X8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB;
                            header.ddspf.dwRGBBitCount = 32u;
                            header.ddspf.dwRBitMask = 0xFFu;
                            header.ddspf.dwGBitMask = 0xFF00u;
                            header.ddspf.dwBBitMask = 0xFF0000u;
                            header.ddspf.dwABitMask = 0x0u;
                            coder = new R8_G8_B8_X8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.B8_G8_R8_A8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB | DDS_PIXELFORMAT.DDPF_ALPHAPIXELS;
                            header.ddspf.dwRGBBitCount = 32u;
                            header.ddspf.dwRBitMask = 0xFF0000u;
                            header.ddspf.dwGBitMask = 0xFF00u;
                            header.ddspf.dwBBitMask = 0xFFu;
                            header.ddspf.dwABitMask = 0xFF000000u;
                            coder = new B8_G8_R8_A8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.B8_G8_R8_X8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB;
                            header.ddspf.dwRGBBitCount = 32u;
                            header.ddspf.dwRBitMask = 0xFF0000u;
                            header.ddspf.dwGBitMask = 0xFF00u;
                            header.ddspf.dwBBitMask = 0xFFu;
                            header.ddspf.dwABitMask = 0x0u;
                            coder = new B8_G8_R8_X8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.R8_G8_B8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 3u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB;
                            header.ddspf.dwRGBBitCount = 24u;
                            header.ddspf.dwRBitMask = 0xFF0000u;
                            header.ddspf.dwGBitMask = 0xFF00u;
                            header.ddspf.dwBBitMask = 0xFFu;
                            header.ddspf.dwABitMask = 0x0u;
                            coder = new R8_G8_B8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.B8_G8_R8_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                            header.dwPitchOrLinearSize = (uint)bitmap.Width * 3u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB;
                            header.ddspf.dwRGBBitCount = 24u;
                            header.ddspf.dwRBitMask = 0xFF0000u;
                            header.ddspf.dwGBitMask = 0xFF00u;
                            header.ddspf.dwBBitMask = 0xFFu;
                            header.ddspf.dwABitMask = 0x0u;
                            coder = new B8_G8_R8_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGB_BC1_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap) / 2u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.DXT1;
                            coder = new RGB_BC1_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_BC1_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap) / 2u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.DXT1;
                            coder = new RGBA_BC1_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_BC2_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap);
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.DXT3;
                            coder = new RGBA_BC2_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_BC3_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap);
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.DXT5;
                            coder = new RGBA_BC3_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGB_ATC_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap) / 2u;
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.ATC_;
                            coder = new RGB_ATC_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_ATC_Explicit_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap);
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.ATCI;
                            coder = new RGBA_ATC_Explicit_UByte();
                        }
                        else if (ddsArgs.Format == DdsEncodingFormat.RGBA_ATC_Interpolated_UByte)
                        {
                            header.dwFlags |= DDS_HEADER.DDSD_LINEARSIZE;
                            header.dwPitchOrLinearSize = GetAlignSize(bitmap);
                            header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_FOURCC;
                            header.ddspf.dwFourCC = DDS_PIXELFORMAT.ATCA;
                            coder = new RGBA_ATC_Interpolated_UByte();
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        bool pitch = (header.dwFlags & DDS_HEADER.DDSD_PITCH) != 0u;
                        uint memSize = pitch ? header.dwPitchOrLinearSize * header.dwHeight : header.dwPitchOrLinearSize;
                        WriteMagic(stream);
                        WriteDdsHeader(stream, &header);
                        using (NativeMemoryOwner owner = new NativeMemoryOwner(memSize))
                        {
                            Span<byte> dataSpan = owner.AsSpan();
                            if (pitch)
                            {
                                coder.Encode(bitmap, dataSpan, (int)header.dwWidth, (int)header.dwHeight, (int)header.dwPitchOrLinearSize);
                            }
                            else
                            {
                                coder.Encode(bitmap, dataSpan, (int)header.dwWidth, (int)header.dwHeight);
                            }
                            stream.Write(owner.Pointer, owner.Size);
                        }
                    }
                }
            }
            else
            {
                header.dwFlags |= DDS_HEADER.DDSD_PITCH;
                header.dwPitchOrLinearSize = (uint)bitmap.Width * 4u;
                header.ddspf.dwFlags |= DDS_PIXELFORMAT.DDPF_RGB | DDS_PIXELFORMAT.DDPF_ALPHAPIXELS;
                header.ddspf.dwRGBBitCount = 32u;
                header.ddspf.dwRBitMask = 0xFF0000u;
                header.ddspf.dwGBitMask = 0xFF00u;
                header.ddspf.dwBBitMask = 0xFFu;
                header.ddspf.dwABitMask = 0xFF000000u;
                WriteMagic(stream);
                WriteDdsHeader(stream, &header);
                int w = bitmap.Width;
                int h = bitmap.Height;
                nuint pitch = (uint)bitmap.Width * 4u;
                for (int y = 0; y < h; y++)
                {
                    fixed (YFColor* rowPtr = bitmap[y])
                    {
                        stream.Write(rowPtr, pitch);
                    }
                }
            }
        }

        private static uint GetAlignSize(RefBitmap bitmap)
        {
            return (((uint)bitmap.Width + 3u) / 4u * 4u) * (((uint)bitmap.Height + 3u) / 4u * 4u);
        }

        private static void WriteDx10Header(Stream stream, DDS_HEADER_DXT10* header)
        {
            stream.Write(header, (nuint)sizeof(DDS_HEADER_DXT10));
        }

        private static void WriteDdsHeader(Stream stream, DDS_HEADER* header)
        {
            stream.Write(header, (nuint)sizeof(DDS_HEADER));
        }

        private static void WriteMagic(Stream stream)
        {
            stream.WriteUInt32LE(DdsDecoder.Magic);
        }
    }
}
