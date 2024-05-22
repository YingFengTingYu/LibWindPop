using LibWindPop.Images.PtxRsb;
using LibWindPop.Packs.Common;
using LibWindPop.Utils;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using System;
using System.IO;
using LibWindPop.Utils.Extension;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public sealed class SquarePVRTCImages : IContentPipeline
    {
        public void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            
        }

        public void OnEndBuild(string packPath, IFileSystem fileSystem, ILogger logger)
        {
            
        }

        public void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            BuildInternal(unpackPath, fileSystem, logger);
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
                                if (image.Format != 30 && image.Format != 31 && image.Format != 148)
                                {
                                    continue;
                                }
                                string nativePtxPath = image.InFileIndexDataMap ? paths.GetResourcePathByGroupIdAndPath(groupId, image.Path) : paths.GetUnusedResourcePathByGroupIdAndPath(groupId, image.Path);
                                string nativePngPath = fileSystem.ChangeExtension(nativePtxPath, ".png");
                                using (Stream pngStream = fileSystem.OpenReadWrite(nativePngPath))
                                {
                                    ImageCoder.PeekImageInfo(pngStream, out int imageWidth, out int imageHeight, out ImageFormat imageFormat);
                                    if (imageWidth != imageHeight || !BitHelper.IsPowerOfTwo(imageWidth))
                                    {
                                        image.Width = (uint)imageWidth;
                                        image.Height = (uint)imageHeight;
                                        int newSize = BitHelper.GetClosestPowerOfTwoAbove(Math.Max(imageWidth, imageHeight));
                                        uint rawBitmapSize = (uint)imageWidth * (uint)imageHeight * 4u;
                                        if (rawBitmapSize > owner.Size)
                                        {
                                            logger.LogWarning($"Memory overflow! Realloc memory with size {rawBitmapSize}...");
                                            owner.Realloc(rawBitmapSize);
                                        }
                                        void* rawBitmapPtr = owner.Pointer;
                                        RefBitmap rawRefBitmap = new RefBitmap(imageWidth, imageHeight, new Span<YFColor>(rawBitmapPtr, imageWidth * imageHeight));
                                        ImageCoder.DecodeImage(pngStream, rawRefBitmap, imageFormat);
                                        void* newBitmapPtr = (void*)((nuint)owner.Pointer + rawBitmapSize);
                                        uint newBitmapSize = (uint)newSize * (uint)newSize * 4u;
                                        if ((newBitmapSize + rawBitmapSize) > owner.Size)
                                        {
                                            logger.LogWarning($"Memory overflow! Realloc memory with size {newBitmapSize + rawBitmapSize}...");
                                            owner.Realloc(newBitmapSize + rawBitmapSize);
                                            newBitmapPtr = (void*)((nuint)owner.Pointer + rawBitmapSize);
                                            rawBitmapPtr = owner.Pointer;
                                            rawRefBitmap = new RefBitmap(imageWidth, imageHeight, new Span<YFColor>(rawBitmapPtr, imageWidth * imageHeight));
                                        }
                                        logger.Log($"Square png {nativePngPath}...");
                                        new Span<byte>(newBitmapPtr, (int)newBitmapSize).Clear();
                                        RefBitmap newRefBitmap = new RefBitmap(newSize, newSize, new Span<YFColor>(rawBitmapPtr, newSize * newSize));
                                        for (int y = 0; y < imageHeight; y++)
                                        {
                                            rawRefBitmap[y].CopyTo(newRefBitmap[y]);
                                        }
                                        pngStream.SetLength(0);
                                        pngStream.Seek(0, SeekOrigin.Begin);
                                        ImageCoder.EncodeImage(pngStream, newRefBitmap, ImageFormat.Png, null);
                                    }
                                }
                            }
                        }
                    }
                }
                WindJsonSerializer.TrySerializeToFile(paths.InfoPackInfoPath, packInfo, fileSystem, logger);
            }
        }
    }
}
