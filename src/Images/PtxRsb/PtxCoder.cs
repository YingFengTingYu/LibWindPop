using LibWindPop.Utils;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Images.PtxRsb
{
    public static class PtxCoder
    {
        public static void Decode(string ptxPath, string pngPath, IFileSystem fileSystem, ILogger logger, uint width, uint height, uint pitch, uint format, uint alphaSize, string? ptxHandlerType)
        {
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            IPtxRsbHandler handler = PtxRsbHandlerManager.GetHandlerFromId(ptxHandlerType, logger, false);
            uint dataSize = handler.GetPtxSize(width, height, pitch, format, alphaSize);
            logger.Log($"PtxCoder.Decode load data from ptx...", 0);
            using (NativeMemoryOwner owner = new NativeMemoryOwner(dataSize))
            {
                Span<byte> ptxData = owner.AsSpan();
                using (Stream ptxStream = fileSystem.OpenRead(ptxPath))
                {
                    ptxStream.Read(ptxData);
                }
                using (IDisposableBitmap bitmap = new NativeBitmap((int)width, (int)height))
                {
                    RefBitmap refBitmap = bitmap.AsRefBitmap();
                    logger.Log($"PtxCoder.Decode decode ptx with width = {width}, height = {height}, pitch = {pitch}, format = {format}, chineseAlphaSize = {alphaSize}...", 0);
                    handler.DecodePtx(ptxData, refBitmap, width, height, pitch, format, alphaSize, logger);
                    logger.Log($"PtxCoder.Decode save data as png...", 0);
                    using (Stream pngStream = fileSystem.Create(pngPath))
                    {
                        ImageCoder.EncodeImage(pngStream, refBitmap, ImageFormat.Png);
                    }
                }
            }
        }

        public static void Encode(string pngPath, string ptxPath, IFileSystem fileSystem, ILogger logger, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize, string? ptxHandlerType)
        {
            ArgumentNullException.ThrowIfNull(pngPath, nameof(pngPath));
            ArgumentNullException.ThrowIfNull(ptxPath, nameof(ptxPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            IPtxRsbHandler handler = PtxRsbHandlerManager.GetHandlerFromId(ptxHandlerType, logger, false);
            logger.Log($"PtxCoder.Encode load data from png...", 0);
            using (Stream pngStream = fileSystem.OpenRead(pngPath))
            {
                ImageCoder.PeekImageInfo(pngStream, out int imgWidth, out int imgHeight, out ImageFormat imgFormat);
                using (IDisposableBitmap bitmap = new NativeBitmap(imgWidth, imgHeight))
                {
                    RefBitmap refBitmap = bitmap.AsRefBitmap();
                    ImageCoder.DecodeImage(pngStream, refBitmap, imgFormat);
                    if (!handler.PeekEncodedPtxInfo(refBitmap, format, out width, out height, out pitch, out alphaSize))
                    {
                        logger.LogError($"PtxCoder.Encode can not encode this image with format {format}", 0, true);
                    }
                    uint dataSize = handler.GetPtxSize(width, height, pitch, format, alphaSize);
                    using (NativeMemoryOwner owner = new NativeMemoryOwner(dataSize))
                    {
                        owner.Fill(0);
                        Span<byte> ptxData = owner.AsSpan();
                        logger.Log($"PtxCoder.Encode encode ptx with width = {width}, height = {height}, pitch = {pitch}, format = {format}, chineseAlphaSize = {alphaSize}...", 0);
                        handler.EncodePtx(refBitmap, ptxData, width, height, pitch, format, alphaSize, logger);
                        logger.Log($"PtxCoder.Encode save data as ptx...", 0);
                        using (Stream ptxStream = fileSystem.Create(ptxPath))
                        {
                            ptxStream.Write(ptxData);
                        }
                    }
                }
            }
        }
    }
}
