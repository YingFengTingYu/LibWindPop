using LibWindPop.Images.PtxRsb;
using LibWindPop.Packs.Common;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using System;
using System.IO;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public sealed class EncodePtxFromPng : IContentPipeline
    {
        public void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            BuildInternal(unpackPath, fileSystem, logger);
        }

        public void OnEndBuild(string rsbPath, IFileSystem fileSystem, ILogger logger)
        {
            // Nothing to do
        }

        public void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            AddInternal(unpackPath, fileSystem, logger);
        }

        private static unsafe void BuildInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read rsb pack info...");

            RsbPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<RsbPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo == null || packInfo.Groups == null)
            {
                logger.LogError("Pack info is null");
            }
            else
            {
                if (packInfo.UseGroupFolder)
                {
                    paths = new RsbUnpackPathProvider(unpackPath, fileSystem, true);
                }
                IPtxRsbHandler ptxHandler = PtxRsbHandlerManager.GetHandlerFromId(packInfo.ImageHandler, logger);
                RsbPackGroupInfo?[] groups = packInfo.Groups;
                // Get max mem size
                const uint maxMemSize = 0x09000000u;
                // Alloc mem
                logger.Log($"Alloc memory buffer with size {maxMemSize}...");
                using (NativeMemoryOwner owner = new NativeMemoryOwner(maxMemSize))
                {
                    for (int i = groups.Length - 1; i >= 0; i--)
                    {
                        RsbPackGroupGPUFileInfo[]? images = groups[i]?.GPUFileList;
                        if (images != null)
                        {
                            string? groupId = groups[i]?.Id;
                            for (int j = images.Length - 1; j >= 0; j--)
                            {
                                RsbPackGroupGPUFileInfo image = images[j];
                                bool needToUpdate = true;
                                string nativePtxPath = image.InFileIndexDataMap ? paths.GetResourcePathByGroupIdAndPath(groupId, image.Path) : paths.GetUnusedResourcePathByGroupIdAndPath(groupId, image.Path);
                                string nativePngPath = fileSystem.ChangeExtension(nativePtxPath, ".png");
                                string nativeMetaPath = paths.AppendMetaExtension(nativePtxPath);
                                using (Stream pngStream = fileSystem.OpenRead(nativePngPath))
                                {
                                    ImageCoder.PeekImageInfo(pngStream, out int imageWidth, out int imageHeight, out ImageFormat imageFormat);
                                    if (fileSystem.FileExists(nativeMetaPath) && fileSystem.FileExists(nativePtxPath))
                                    {
                                        try
                                        {
                                            PtxRsbMetadata? meta = WindJsonSerializer.TryDeserializeFromFile<PtxRsbMetadata>(nativeMetaPath, fileSystem, new NullLogger(true));

                                            if (meta != null
                                                && meta.ImageHandler == packInfo.ImageHandler
                                                && (uint)imageWidth == image.Width
                                                && (uint)imageHeight == image.Height
                                                && meta.Width == image.Width
                                                && meta.Height == image.Height
                                                && meta.Pitch == image.Pitch
                                                && meta.Format == image.Format
                                                && ((!ptxHandler.UseExtend1AsAlphaSize) || meta.AlphaSize == image.Extend1)
                                                && meta.Hash == fileSystem.GetFileHash(nativePngPath)
                                                )
                                            {
                                                needToUpdate = false;
                                            }
                                        }
                                        catch
                                        {
                                            // Do not need to handle this exception
                                        }
                                    }
                                    if (needToUpdate)
                                    {
                                        uint bitmapSize = image.Width * image.Height * 4u;
                                        if (bitmapSize > owner.Size)
                                        {
                                            logger.LogWarning($"Memory overflow! Realloc memory with size {bitmapSize}...");
                                            owner.Realloc(bitmapSize);
                                        }
                                        void* bitmapPtr = owner.Pointer;
                                        RefBitmap refBitmap = new RefBitmap((int)image.Width, (int)image.Height, new Span<YFColor>(bitmapPtr, (int)(image.Width * image.Height)));
                                        ImageCoder.DecodeImage(pngStream, refBitmap, imageFormat);
                                        if (!ptxHandler.PeekEncodedPtxInfo(refBitmap, image.Format, out uint newWidth, out uint newHeight, out uint newPitch, out uint newAlphaSize))
                                        {
                                            logger.LogError($"PtxCoder.Encode can not encode this image with format {image.Format}: {nativePngPath}");
                                            continue;
                                        }
                                        void* ptxPtr = (void*)((nuint)owner.Pointer + bitmapSize);
                                        uint ptxSize = ptxHandler.GetPtxSize(newWidth, newHeight, newPitch, image.Format, newAlphaSize);
                                        if ((ptxSize + bitmapSize) > owner.Size)
                                        {
                                            logger.LogWarning($"Memory overflow! Realloc memory with size {ptxSize + bitmapSize}...");
                                            owner.Realloc(ptxSize + bitmapSize);
                                            ptxPtr = (void*)((nuint)owner.Pointer + bitmapSize);
                                            bitmapPtr = owner.Pointer;
                                            refBitmap = new RefBitmap((int)image.Width, (int)image.Height, new Span<YFColor>(bitmapPtr, (int)(image.Width * image.Height)));
                                        }
                                        logger.Log($"Encode ptx {nativePtxPath}...");
                                        new Span<byte>(ptxPtr, (int)ptxSize).Clear();
                                        ptxHandler.EncodePtx(refBitmap, new Span<byte>(ptxPtr, (int)ptxSize), newWidth, newHeight, newPitch, image.Format, newAlphaSize, logger);
                                        image.Width = newWidth;
                                        image.Height = newHeight;
                                        image.Pitch = newPitch;
                                        if (ptxHandler.UseExtend1AsAlphaSize)
                                        {
                                            image.Extend1 = newAlphaSize;
                                        }
                                        // Write ptx
                                        using (Stream ptxStream = fileSystem.Create(nativePtxPath))
                                        {
                                            ptxStream.Write(ptxPtr, ptxSize);
                                        }
                                        // Write meta
                                        PtxRsbMetadata meta = new PtxRsbMetadata
                                        {
                                            ImageHandler = packInfo.ImageHandler,
                                            Width = image.Width,
                                            Height = image.Height,
                                            Pitch = image.Pitch,
                                            Format = image.Format,
                                            AlphaSize = ptxHandler.UseExtend1AsAlphaSize ? image.Extend1 : 0u,
                                            Hash = fileSystem.GetFileHash(nativePngPath)
                                        };
                                        WindJsonSerializer.TrySerializeToFile(nativeMetaPath, meta, fileSystem, logger);
                                    }
                                }
                            }
                        }
                    }
                }
                WindJsonSerializer.TrySerializeToFile(paths.InfoPackInfoPath, packInfo, fileSystem, logger);
            }
        }

        private static unsafe void AddInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read rsb pack info...");

            RsbPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<RsbPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo == null || packInfo.Groups == null)
            {
                logger.LogError("Pack info is null");
            }
            else
            {
                if (packInfo.UseGroupFolder)
                {
                    paths = new RsbUnpackPathProvider(unpackPath, fileSystem, true);
                }
                IPtxRsbHandler ptxHandler = PtxRsbHandlerManager.GetHandlerFromId(packInfo.ImageHandler, logger);
                RsbPackGroupInfo?[] groups = packInfo.Groups;
                // Get max mem size
                uint maxMemSize = 0u;
                for (int i = groups.Length - 1; i >= 0; i--)
                {
                    RsbPackGroupGPUFileInfo[]? images = groups[i]?.GPUFileList;
                    if (images != null)
                    {
                        for (int j = images.Length - 1; j >= 0; j--)
                        {
                            RsbPackGroupGPUFileInfo image = images[j];
                            uint ptxSize = ptxHandler.GetPtxSize(image.Width, image.Height, image.Pitch, image.Format, image.Extend1);
                            uint bitmapSize = image.Width * image.Height * 4u;
                            maxMemSize = Math.Max(maxMemSize, ptxSize + bitmapSize);
                        }
                    }
                }
                // Alloc mem
                logger.Log($"Alloc memory buffer with size {maxMemSize}...");
                using (NativeMemoryOwner owner = new NativeMemoryOwner(maxMemSize))
                {
                    for (int i = groups.Length - 1; i >= 0; i--)
                    {
                        RsbPackGroupGPUFileInfo[]? images = groups[i]?.GPUFileList;
                        if (images != null)
                        {
                            string? groupId = groups[i]?.Id;
                            for (int j = images.Length - 1; j >= 0; j--)
                            {
                                RsbPackGroupGPUFileInfo image = images[j];
                                string nativePtxPath = image.InFileIndexDataMap ? paths.GetResourcePathByGroupIdAndPath(groupId, image.Path) : paths.GetUnusedResourcePathByGroupIdAndPath(groupId, image.Path);
                                string nativePngPath = fileSystem.ChangeExtension(nativePtxPath, ".png");
                                string nativeMetaPath = paths.AppendMetaExtension(nativePtxPath);
                                uint ptxSize = ptxHandler.GetPtxSize(image.Width, image.Height, image.Pitch, image.Format, image.Extend1);
                                uint bitmapSize = image.Width * image.Height * 4u;
                                void* ptxPtr = owner.Pointer;
                                void* bitmapPtr = (void*)((nuint)owner.Pointer + ptxSize);
                                logger.Log($"Decode ptx {nativePtxPath}...");
                                // Read ptx
                                using (Stream ptxStream = fileSystem.OpenRead(nativePtxPath))
                                {
                                    ptxStream.Read(ptxPtr, ptxSize);
                                }
                                RefBitmap refBitmap = new RefBitmap((int)image.Width, (int)image.Height, new Span<YFColor>(bitmapPtr, (int)(image.Width * image.Height)));
                                ptxHandler.DecodePtx(new ReadOnlySpan<byte>(ptxPtr, (int)ptxSize), refBitmap, image.Width, image.Height, image.Pitch, image.Format, image.Extend1, logger);
                                using (Stream pngStream = fileSystem.Create(nativePngPath))
                                {
                                    ImageCoder.EncodeImage(pngStream, refBitmap, ImageFormat.Png, null);
                                }
                                PtxRsbMetadata meta = new PtxRsbMetadata
                                {
                                    ImageHandler = packInfo.ImageHandler,
                                    Width = image.Width,
                                    Height = image.Height,
                                    Pitch = image.Pitch,
                                    Format = image.Format,
                                    AlphaSize = ptxHandler.UseExtend1AsAlphaSize ? image.Extend1 : 0u,
                                    Hash = fileSystem.GetFileHash(nativePngPath)
                                };
                                WindJsonSerializer.TrySerializeToFile(nativeMetaPath, meta, fileSystem, logger);
                            }
                        }
                    }
                }
            }
        }
    }
}
