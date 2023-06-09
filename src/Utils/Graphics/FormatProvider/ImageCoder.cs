﻿using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider.Dds;
using LibWindPop.Utils.Graphics.FormatProvider.Png;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider
{
    public static class ImageCoder
    {
        public static void PeekImageInfo(Stream stream, out int width, out int height, out ImageFormat format)
        {
            format = ImageFormat.None;
            width = 0;
            height = 0;
            if (PngDecoder.IsPng(stream))
            {
                if (PngDecoder.PeekPngWidthHeight(stream, out uint w, out uint h))
                {
                    format = ImageFormat.Png;
                    width = (int)w;
                    height = (int)h;
                    return;
                }
            }
            if (DdsDecoder.IsDds(stream))
            {
                if (DdsDecoder.PeekDdsWidthHeight(stream, out uint w, out uint h))
                {
                    format = ImageFormat.Dds;
                    width = (int)w;
                    height = (int)h;
                    return;
                }
            }
        }

        public static bool DecodeImage(Stream stream, RefBitmap bitmap, ImageFormat format)
        {
            if (format == ImageFormat.Png)
            {
                PngDecoder.DecodePng(stream, bitmap);
                return true;
            }
            if (format == ImageFormat.Dds)
            {
                DdsDecoder.DecodeDds(stream, bitmap);
                return true;
            }
            return false;
        }

        public static bool EncodeImage(Stream stream, RefBitmap bitmap, ImageFormat format, IImageEncoderArgument? args)
        {
            if (format == ImageFormat.Png)
            {
                PngEncoder.EncodePng(stream, bitmap, args);
                return true;
            }
            if (format == ImageFormat.Dds)
            {
                DdsEncoder.EncodeDds(stream, bitmap, args);
                return true;
            }
            return false;
        }
    }
}
