using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Graphics.FormatProvider.Dds;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Images.PtxPS3
{
    public static class PtxPS3Coder
    {
        public static void Encode(string pngPath, string ptxPath, IFileSystem fileSystem, ILogger logger, PtxPS3PixelFormat ddsFormat)
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

        public static void Encode(Stream pngStream, Stream ptxStream, ILogger logger, PtxPS3PixelFormat format)
        {
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ImageCoder.PeekImageInfo(pngStream, out int width, out int height, out ImageFormat imageFormat);
            DdsEncodingFormat ddsFormat = format switch
            {
                PtxPS3PixelFormat.RGBA_BC1_UByte => DdsEncodingFormat.RGBA_BC1_UByte,
                PtxPS3PixelFormat.RGBA_BC2_UByte => DdsEncodingFormat.RGBA_BC2_UByte,
                PtxPS3PixelFormat.RGBA_BC3_UByte => DdsEncodingFormat.RGBA_BC3_UByte,
                PtxPS3PixelFormat.L8_UByte => DdsEncodingFormat.L8_UByte,
                PtxPS3PixelFormat.R8_G8_B8_A8_UByte => DdsEncodingFormat.R8_G8_B8_A8_UByte,
                PtxPS3PixelFormat.R8_G8_B8_X8_UByte => DdsEncodingFormat.R8_G8_B8_X8_UByte,
                PtxPS3PixelFormat.R8_G8_B8_UByte => DdsEncodingFormat.R8_G8_B8_UByte,
                _ => DdsEncodingFormat.RGBA_BC3_UByte
            };
            using (NativeBitmap bitmap = new NativeBitmap(width, height))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                ImageCoder.DecodeImage(pngStream, refBitmap, imageFormat);
                DdsEncoder.EncodeDds(ptxStream, refBitmap, new DdsEncoderArgument { UseDX10Header = false, Format = ddsFormat });
            }
        }

        public static PtxPS3PixelFormat Decode(string ptxPath, string pngPath, IFileSystem fileSystem, ILogger logger)
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

        public static unsafe PtxPS3PixelFormat Decode(Stream ptxStream, Stream pngStream, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            if (!DdsDecoder.IsDds(ptxStream))
            {
                logger.LogError($"Ptx magic mismatch: DDS  expected");
            }
            PtxPS3PixelFormat format = PtxPS3PixelFormat.None;
            long pos = ptxStream.Position;
            DDS_HEADER header;
            ptxStream.Seek(pos + 4, SeekOrigin.Begin);
            ptxStream.Read(&header, (nuint)sizeof(DDS_HEADER));
            ptxStream.Seek(pos, SeekOrigin.Begin);
            if (header.dwSize == (uint)sizeof(DDS_HEADER))
            {
                if ((header.ddspf.dwFlags & DDS_PIXELFORMAT.DDPF_FOURCC) != 0u)
                {
                    if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT1)
                    {
                        format = PtxPS3PixelFormat.RGBA_BC1_UByte;
                    }
                    if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT2)
                    {
                        format = PtxPS3PixelFormat.RGBA_BC2_UByte;
                    }
                    if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT3)
                    {
                        format = PtxPS3PixelFormat.RGBA_BC2_UByte;
                    }
                    if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT4)
                    {
                        format = PtxPS3PixelFormat.RGBA_BC3_UByte;
                    }
                    if (header.ddspf.dwFourCC == DDS_PIXELFORMAT.DXT5)
                    {
                        format = PtxPS3PixelFormat.RGBA_BC3_UByte;
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
                                format = PtxPS3PixelFormat.R8_G8_B8_A8_UByte;
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
                                format = PtxPS3PixelFormat.R8_G8_B8_X8_UByte;
                            }
                        }
                        else if (header.ddspf.dwRGBBitCount == 24u)
                        {
                            if (header.ddspf.dwRBitMask == 0xFFu
                                && header.ddspf.dwGBitMask == 0xFF00u
                                && header.ddspf.dwBBitMask == 0xFF0000u)
                            {
                                format = PtxPS3PixelFormat.R8_G8_B8_UByte;
                            }
                        }
                        else if (header.ddspf.dwRGBBitCount == 8u)
                        {
                            if (header.ddspf.dwRBitMask == 0xFFu)
                            {
                                format = PtxPS3PixelFormat.L8_UByte;
                            }
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
