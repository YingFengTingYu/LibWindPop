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
        public static void Encode(string pngPath, string ptxPath, IFileSystem fileSystem, ILogger logger, DdsEncodingFormat ddsFormat)
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

        public static void Encode(Stream pngStream, Stream ptxStream, ILogger logger, DdsEncodingFormat ddsFormat)
        {
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ImageCoder.PeekImageInfo(pngStream, out int width, out int height, out ImageFormat format);
            using (NativeBitmap bitmap = new NativeBitmap(width, height))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                ImageCoder.DecodeImage(ptxStream, refBitmap, format);
                DdsEncoder.EncodeDds(pngStream, refBitmap, new DdsEncoderArgument { UseDX10Header = false, Format = ddsFormat });
            }
        }

        public static void Decode(string ptxPath, string pngPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            using (Stream ptxStream = fileSystem.OpenRead(ptxPath))
            {
                using (Stream pngStream = fileSystem.Create(pngPath))
                {
                    Decode(ptxStream, pngStream, logger);
                }
            }
        }

        public static void Decode(Stream ptxStream, Stream pngStream, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(ptxStream, nameof(ptxStream));
            ArgumentNullException.ThrowIfNull(pngStream, nameof(pngStream));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            if (!DdsDecoder.IsDds(ptxStream))
            {
                logger.LogError($"Ptx magic mismatch: DDS  expected");
            }
            DdsDecoder.PeekDdsWidthHeight(ptxStream, out uint width, out uint height);
            using (NativeBitmap bitmap = new NativeBitmap((int)width, (int)height))
            {
                RefBitmap refBitmap = bitmap.AsRefBitmap();
                DdsDecoder.DecodeDds(ptxStream, refBitmap);
                ImageCoder.EncodeImage(pngStream, refBitmap, ImageFormat.Png, null);
            }
        }
    }
}
