using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Graphics.FormatProvider.Dds;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.DirectX;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Images.PtxDds
{
    public static class PtxDdsCoder
    {
        public static void Encode(string pngPath, string ptxPath, IFileSystem fileSystem, ILogger logger, PtxDdsPixelFormat ddsFormat)
        {
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            using (Stream pngStream = fileSystem.OpenRead(pngPath))
            {
                using (Stream ptxStream = fileSystem.Create(ptxPath))
                {
                    Encode(pngStream, ptxStream, logger, ddsFormat);
                }
            }
        }

        public static void Encode(Stream pngStream, Stream ptxStream, ILogger logger, PtxDdsPixelFormat format)
        {
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ImageCoder.PeekImageInfo(pngStream, out int width, out int height, out ImageFormat imageFormat);
            DdsEncodingFormat ddsFormat = format switch
            {
                PtxDdsPixelFormat.RGB_BC1_UByte => DdsEncodingFormat.RGB_BC1_UByte,
                PtxDdsPixelFormat.RGBA_BC1_UByte => DdsEncodingFormat.RGBA_BC1_UByte,
                PtxDdsPixelFormat.RGBA_BC2_UByte => DdsEncodingFormat.RGBA_BC2_UByte,
                PtxDdsPixelFormat.RGBA_BC3_UByte => DdsEncodingFormat.RGBA_BC3_UByte,
                PtxDdsPixelFormat.L8_UByte => DdsEncodingFormat.L8_UByte,
                PtxDdsPixelFormat.A8_UByte => DdsEncodingFormat.A8_UByte,
                PtxDdsPixelFormat.R8_G8_B8_A8_UByte => DdsEncodingFormat.R8_G8_B8_A8_UByte,
                PtxDdsPixelFormat.R8_G8_B8_X8_UByte => DdsEncodingFormat.R8_G8_B8_X8_UByte,
                PtxDdsPixelFormat.R8_G8_B8_UByte => DdsEncodingFormat.R8_G8_B8_UByte,
                PtxDdsPixelFormat.RGBA_ASTC_4x4_UByte => DdsEncodingFormat.RGBA_ASTC_4x4_UByte,
                PtxDdsPixelFormat.RGBA_ASTC_5x5_UByte => DdsEncodingFormat.RGBA_ASTC_5x5_UByte,
                PtxDdsPixelFormat.RGBA_ASTC_6x6_UByte => DdsEncodingFormat.RGBA_ASTC_6x6_UByte,
                PtxDdsPixelFormat.RGBA_ASTC_8x8_UByte => DdsEncodingFormat.RGBA_ASTC_8x8_UByte,
                _ => DdsEncodingFormat.RGBA_BC3_UByte
            };
            bool useDx10Header = ddsFormat == DdsEncodingFormat.RGBA_ASTC_4x4_UByte
                || ddsFormat == DdsEncodingFormat.RGBA_ASTC_5x5_UByte
                || ddsFormat == DdsEncodingFormat.RGBA_ASTC_6x6_UByte
                || ddsFormat == DdsEncodingFormat.RGBA_ASTC_8x8_UByte;
            using (NativeBitmap bitmap = new NativeBitmap(width, height))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                ImageCoder.DecodeImage(pngStream, refBitmap, imageFormat);
                DdsEncoder.EncodeDds(ptxStream, refBitmap, new DdsEncoderArgument { UseDX10Header = useDx10Header, Format = ddsFormat });
            }
        }

        public static PtxDdsPixelFormat Decode(string ptxPath, string pngPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            using (Stream ptxStream = fileSystem.OpenRead(ptxPath))
            {
                using (Stream pngStream = fileSystem.Create(pngPath))
                {
                    return Decode(ptxStream, pngStream, logger);
                }
            }
        }

        public static unsafe PtxDdsPixelFormat Decode(Stream ptxStream, Stream pngStream, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            long pos = ptxStream.Position;
            uint magic = ptxStream.ReadUInt32LE();
            if (magic != 0x20534444u)
            {
                logger.LogError($"Ptx magic mismatch: DDS  expected");
            }
            PtxDdsPixelFormat format = PtxDdsPixelFormat.None;
            DDS_HEADER header;
            ptxStream.Read(&header, (nuint)sizeof(DDS_HEADER));
            ptxStream.Seek(pos, SeekOrigin.Begin);
            if (header.dwSize == (uint)sizeof(DDS_HEADER))
            {
                if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_FOURCC) != 0u)
                {
                    if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DX10)
                    {
                        DDS_HEADER_DXT10 dx10Header;
                        long headerDx10Pos = pos + 4 + sizeof(DDS_HEADER);
                        long currentPos = ptxStream.Position;
                        ptxStream.Seek(headerDx10Pos, SeekOrigin.Begin);
                        ptxStream.Read(&dx10Header, (uint)sizeof(DDS_HEADER_DXT10));
                        ptxStream.Seek(currentPos, SeekOrigin.Begin);
                        if (dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_4X4_TYPELESS
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_4X4_UNORM
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_4X4_UNORM_SRGB)
                        {
                            format = PtxDdsPixelFormat.RGBA_ASTC_4x4_UByte;
                        }
                        else if (dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_5X5_TYPELESS
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_5X5_UNORM
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_5X5_UNORM_SRGB)
                        {
                            format = PtxDdsPixelFormat.RGBA_ASTC_5x5_UByte;
                        }
                        else if (dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_6X6_TYPELESS
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_6X6_UNORM
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_6X6_UNORM_SRGB)
                        {
                            format = PtxDdsPixelFormat.RGBA_ASTC_6x6_UByte;
                        }
                        else if (dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_8X8_TYPELESS
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_8X8_UNORM
                            || dx10Header.dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_ASTC_8X8_UNORM_SRGB)
                        {
                            format = PtxDdsPixelFormat.RGBA_ASTC_8x8_UByte;
                        }
                    }
                    else if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT1)
                    {
                        format = PtxDdsPixelFormat.RGBA_BC1_UByte;
                    }
                    else if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT2 || header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT3)
                    {
                        format = PtxDdsPixelFormat.RGBA_BC2_UByte;
                    }
                    else if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT4 || header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT5)
                    {
                        format = PtxDdsPixelFormat.RGBA_BC3_UByte;
                    }
                }
                else if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_RGB) != 0u)
                {
                    if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_ALPHAPIXELS) != 0u)
                    {
                        if (header.ddspf.dwRGBBitCount == 32u)
                        {
                            if (header.ddspf.dwRBitMask == 0xFFu
                                && header.ddspf.dwGBitMask == 0xFF00u
                                && header.ddspf.dwBBitMask == 0xFF0000u
                                && header.ddspf.dwABitMask == 0xFF000000u)
                            {
                                format = PtxDdsPixelFormat.R8_G8_B8_A8_UByte;
                            }
                            else if (header.ddspf.dwBBitMask == 0xFFu
                                    && header.ddspf.dwGBitMask == 0xFF00u
                                    && header.ddspf.dwRBitMask == 0xFF0000u
                                    && header.ddspf.dwABitMask == 0xFF000000u)
                            {
                                format = PtxDdsPixelFormat.B8_G8_R8_A8_UByte;
                            }
                        }
                    }
                    else
                    {
                        if (header.ddspf.dwRGBBitCount == 32u)
                        {
                            if (header.ddspf.dwRBitMask == 0xFFu
                                && header.ddspf.dwGBitMask == 0xFF00u
                                && header.ddspf.dwBBitMask == 0xFF0000u)
                            {
                                format = PtxDdsPixelFormat.R8_G8_B8_X8_UByte;
                            }
                            else if (header.ddspf.dwBBitMask == 0xFFu
                                && header.ddspf.dwGBitMask == 0xFF00u
                                && header.ddspf.dwRBitMask == 0xFF0000u)
                            {
                                format = PtxDdsPixelFormat.B8_G8_R8_X8_UByte;
                            }
                        }
                        else if (header.ddspf.dwRGBBitCount == 24u)
                        {
                            if (header.ddspf.dwRBitMask == 0xFFu
                                && header.ddspf.dwGBitMask == 0xFF00u
                                && header.ddspf.dwBBitMask == 0xFF0000u)
                            {
                                format = PtxDdsPixelFormat.R8_G8_B8_UByte;
                            }
                            else if (header.ddspf.dwRBitMask == 0xFF0000u
                                && header.ddspf.dwGBitMask == 0xFF00u
                                && header.ddspf.dwBBitMask == 0xFFu)
                            {
                                format = PtxDdsPixelFormat.B8_G8_R8_UByte;
                            }
                        }
                        else if (header.ddspf.dwRGBBitCount == 8u)
                        {
                            if (header.ddspf.dwRBitMask == 0xFFu)
                            {
                                format = PtxDdsPixelFormat.L8_UByte;
                            }
                        }
                    }
                }
                else if ((header.ddspf.dwFlags | DDS_PIXELFORMAT.DDPF_ALPHA) != 0u)
                {
                    if (header.ddspf.dwRGBBitCount == 8u)
                    {
                        if (header.ddspf.dwABitMask == 0xFFu)
                        {
                            format = PtxDdsPixelFormat.A8_UByte;
                        }
                    }
                }
            }
            using (NativeBitmap bitmap = new NativeBitmap((int)header.dwWidth, (int)header.dwHeight))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                DdsDecoder.DecodeDds(ptxStream, refBitmap);
                ImageCoder.EncodeImage(pngStream, refBitmap, ImageFormat.Png, null);
            }
            return format;
        }
    }
}
