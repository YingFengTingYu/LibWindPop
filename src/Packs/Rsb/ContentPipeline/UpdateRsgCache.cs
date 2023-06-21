using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Images.PtxRsb;
using LibWindPop.Packs.Common;
using LibWindPop.Packs.Rsb.RsbStructures;
using LibWindPop.PopCap.Packs.Rsb.Map;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using System;
using System.IO;
using System.Text;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public sealed class UpdateRsgCache : IContentPipeline
    {
        public void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            BuildInternal(unpackPath, fileSystem, logger, 6);
        }

        public void OnEndBuild(string rsbPath, IFileSystem fileSystem, ILogger logger)
        {
            // Nothing to do
        }

        public void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            // Nothing to do
        }

        private static unsafe void BuildInternal(string unpackPath, IFileSystem fileSystem, ILogger logger, int compressionLevel)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            Encoding encoding = EncodingType.iso_8859_1.GetEncoding();

            logger.Log("Read rsb pack info...");

            RsbPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<RsbPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo == null)
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
                RsbPackGroupInfo?[]? groupInfoList = packInfo.Groups;
                if (groupInfoList == null)
                {
                    logger.LogError("Group array is null");
                }
                else
                {
                    // Alloc Stream
                    using ITempFile residentFileTempFile = fileSystem.CreateTempFile();
                    using ITempFile imageTempFile = fileSystem.CreateTempFile();
                    Stream residentFileStream = residentFileTempFile.Stream;
                    Stream imageStream = imageTempFile.Stream;
                    for (int i = 0; i < groupInfoList.Length; i++)
                    {
                        RsbPackGroupInfo? groupInfo = groupInfoList[i];
                        if (groupInfo == null)
                        {
                            logger.LogWarning($"Group {i} is null");
                        }
                        else
                        {
                            bool needToUpdate = true;
                            string rsgPath = paths.GetRsgPathByGroupId(groupInfo.Id);
                            string rsgMetaPath = paths.AppendMetaExtension(rsgPath);
                            if (fileSystem.FileExists(rsgMetaPath) && fileSystem.FileExists(rsgPath))
                            {
                                try
                                {
                                    RsgMetadata? rsgPackInfo = WindJsonSerializer.TryDeserializeFromFile<RsgMetadata>(rsgMetaPath, fileSystem, new NullLogger(true));
                                    if (rsgPackInfo != null
                                        && rsgPackInfo.MajorVersion == packInfo.MajorVersion
                                        && rsgPackInfo.MinorVersion == packInfo.MinorVersion
                                        && rsgPackInfo.UseBigEndian == packInfo.UseBigEndian
                                        && rsgPackInfo.ImageHandler == packInfo.ImageHandler
                                        && (rsgPackInfo.CompressionFlags & 0b11) == (groupInfo.CompressionFlags & 0b11)
                                        && CompareResidentFileList(groupInfo.ResidentFileList, rsgPackInfo.ResidentFileList)
                                        && CompareImageList(groupInfo.GPUFileList, rsgPackInfo.GPUFileList)
                                        && !CheckResidentFileModify(rsgPackInfo.ResidentFileList, paths, groupInfo.Id, fileSystem)
                                        && !CheckImageModify(rsgPackInfo.GPUFileList, paths, groupInfo.Id, fileSystem))
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
                                logger.Log($"Update group {groupInfo.Id}...");
                                // AllocMem
                                uint inMemSize = 0u;
                                uint poolOffset = 0u;
                                uint pairCount = 0u;
                                if (groupInfo.ResidentFileList != null)
                                {
                                    for (int j = groupInfo.ResidentFileList.Length - 1; j >= 0; j--)
                                    {
                                        string? path = groupInfo.ResidentFileList[j]?.Path;
                                        if (path != null)
                                        {
                                            inMemSize += CompiledMapEncodePair.PeekSize(path, (uint)sizeof(RsgResidentFileExtraData)) + (uint)sizeof(CompiledMapEncodePair);
                                            poolOffset += (uint)sizeof(CompiledMapEncodePair);
                                            pairCount++;
                                        }
                                    }
                                }
                                if (groupInfo.GPUFileList != null)
                                {
                                    for (int j = groupInfo.GPUFileList.Length - 1; j >= 0; j--)
                                    {
                                        if (groupInfo.GPUFileList[j] == null || !groupInfo.GPUFileList[j].InFileIndexDataMap)
                                        {
                                            continue;
                                        }
                                        string? path = groupInfo.GPUFileList[j].Path;
                                        if (path != null)
                                        {
                                            inMemSize += CompiledMapEncodePair.PeekSize(path, (uint)sizeof(ResStreamFileGPULocationInfo)) + (uint)sizeof(CompiledMapEncodePair);
                                            poolOffset += (uint)sizeof(CompiledMapEncodePair);
                                            pairCount++;
                                        }
                                    }
                                }
                                using (NativeMemoryOwner rawDataAllocator = new NativeMemoryOwner(inMemSize))
                                {
                                    rawDataAllocator.Clear();
                                    nuint pairPtrNumber = (nuint)rawDataAllocator.Pointer;
                                    nuint poolPtrNumber = pairPtrNumber + poolOffset;
                                    CompiledMapEncodePair* currentPairPtr = (CompiledMapEncodePair*)pairPtrNumber;
                                    nuint currentPoolPtr = poolPtrNumber;
                                    if (groupInfo.ResidentFileList != null)
                                    {
                                        for (int j = 0; j < groupInfo.ResidentFileList.Length; j++)
                                        {
                                            string? key = groupInfo.ResidentFileList[j]?.Path;
                                            if (key != null)
                                            {
                                                nuint stringPtrNumber = currentPoolPtr;
                                                currentPairPtr->KeyOffset = (uint)(currentPoolPtr - poolPtrNumber);
                                                currentPairPtr->KeySize = (uint)key.Length + 1u;
                                                encoding.GetBytes(key, new Span<byte>((void*)currentPoolPtr, key.Length));
                                                currentPoolPtr += (uint)key.Length;
                                                *(byte*)currentPoolPtr = 0x0;
                                                currentPoolPtr++;
                                                currentPairPtr->ValueOffset = (uint)(currentPoolPtr - poolPtrNumber);
                                                currentPairPtr->ValueSize = (uint)sizeof(RsgResidentFileExtraData);
                                                RsgResidentFileExtraData* extra = (RsgResidentFileExtraData*)currentPoolPtr;
                                                extra->Type = 0u;
                                                extra->Offset = 0u;
                                                extra->Size = 0u;
                                                currentPoolPtr += (uint)sizeof(RsgResidentFileExtraData);
                                                currentPairPtr++;
                                                UnsafeStringHelper.StringToUpper(stringPtrNumber);
                                            }
                                        }
                                    }
                                    if (groupInfo.GPUFileList != null)
                                    {
                                        for (int j = 0; j < groupInfo.GPUFileList.Length; j++)
                                        {
                                            if (groupInfo.GPUFileList[j] == null || !groupInfo.GPUFileList[j].InFileIndexDataMap)
                                            {
                                                continue;
                                            }
                                            string? key = groupInfo.GPUFileList[j].Path;
                                            if (key != null)
                                            {
                                                nuint stringPtrNumber = currentPoolPtr;
                                                currentPairPtr->KeyOffset = (uint)(currentPoolPtr - poolPtrNumber);
                                                currentPairPtr->KeySize = (uint)key.Length + 1u;
                                                encoding.GetBytes(key, new Span<byte>((void*)currentPoolPtr, key.Length));
                                                currentPoolPtr += (uint)key.Length;
                                                *(byte*)currentPoolPtr = 0x0;
                                                currentPoolPtr++;
                                                currentPairPtr->ValueOffset = (uint)(currentPoolPtr - poolPtrNumber);
                                                currentPairPtr->ValueSize = (uint)sizeof(ResStreamFileGPULocationInfo);
                                                ResStreamFileGPULocationInfo* extra = (ResStreamFileGPULocationInfo*)currentPoolPtr;
                                                extra->Type = 1u;
                                                extra->Offset = 0u;
                                                extra->Size = 0u;
                                                extra->TextureId = (uint)j;
                                                extra->Width = groupInfo.GPUFileList[j].Width;
                                                extra->Height = groupInfo.GPUFileList[j].Height;
                                                currentPoolPtr += (uint)sizeof(ResStreamFileGPULocationInfo);
                                                currentPairPtr++;
                                                UnsafeStringHelper.StringToUpper(stringPtrNumber);
                                            }
                                        }
                                    }
                                    currentPairPtr = (CompiledMapEncodePair*)pairPtrNumber;
                                    CompiledMapEncoder.Sort(currentPairPtr, pairCount, poolPtrNumber);
                                    CompiledMapEncoder.ComputeRepeatLength(currentPairPtr, pairCount, poolPtrNumber);
                                    uint mapSize = CompiledMapEncoder.PeekSize(currentPairPtr, pairCount);
                                    uint rsgHeaderSize = Align((uint)sizeof(ResStreamGroupHeader) + mapSize);
                                    using (NativeMemoryOwner rsgHeaderMemoryAllocator = new NativeMemoryOwner(rsgHeaderSize))
                                    {
                                        rsgHeaderMemoryAllocator.Fill(0);
                                        nuint rsgHeaderPtrNumber = (nuint)rsgHeaderMemoryAllocator.Pointer;
                                        ResStreamGroupHeader* rsgInfo = (ResStreamGroupHeader*)rsgHeaderPtrNumber;
                                        rsgInfo->Magic = 1920165744u; // pgsr
                                        rsgInfo->MajorVersion = packInfo.MajorVersion;
                                        rsgInfo->MinorVersion = packInfo.MinorVersion;
                                        rsgInfo->CompressionFlags = groupInfo.CompressionFlags;
                                        rsgInfo->HeaderSize = rsgHeaderSize;
                                        rsgInfo->FileIndexDataMapSize = mapSize;
                                        rsgInfo->FileIndexDataMapOffset = 0x5C;
                                        RsgMetadata rsgPackInfo = new()
                                        {
                                            UseBigEndian = packInfo.UseBigEndian,
                                            MajorVersion = packInfo.MajorVersion,
                                            MinorVersion = packInfo.MinorVersion,
                                            ImageHandler = packInfo.ImageHandler,
                                            CompressionFlags = groupInfo.CompressionFlags,
                                            HeaderSize = rsgHeaderSize,
                                            ResidentDataOffset = 0u,
                                            ResidentDataCompressedSize = 0u,
                                            ResidentDataUncompressedSize = 0u,
                                            GPUDataOffset = 0u,
                                            GPUDataCompressedSize = 0u,
                                            GPUDataUncompressedSize = 0u,
                                            ResidentFileList = null,
                                            GPUFileList = null,
                                        };
                                        // CompiledMap
                                        CompiledMapEncoder.Encode(currentPairPtr, pairCount, poolPtrNumber, rsgHeaderPtrNumber + rsgInfo->FileIndexDataMapOffset);
                                        CompiledMap map = new CompiledMap();
                                        map.Init(rsgHeaderPtrNumber + rsgInfo->FileIndexDataMapOffset, rsgInfo->FileIndexDataMapSize);
                                        using (Stream rsgStream = fileSystem.Create(rsgPath))
                                        {
                                            // Header length
                                            rsgStream.SetLength(rsgPackInfo.HeaderSize);
                                            rsgStream.Seek(rsgPackInfo.HeaderSize, SeekOrigin.Begin);
                                            // Resident file
                                            if (groupInfo.ResidentFileList == null)
                                            {
                                                if ((rsgPackInfo.CompressionFlags & 0b10) == 0)
                                                {
                                                    // No Zlib
                                                    rsgPackInfo.ResidentDataOffset = (uint)rsgStream.Position;
                                                    rsgPackInfo.ResidentDataCompressedSize = 0u;
                                                    rsgPackInfo.ResidentDataUncompressedSize = 0u;
                                                }
                                                else
                                                {
                                                    // Zlib
                                                    rsgPackInfo.ResidentDataOffset = (uint)rsgStream.Position;
                                                    rsgPackInfo.ResidentDataUncompressedSize = 0u;
                                                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(rsgStream, new Deflater(compressionLevel)))
                                                    {
                                                        zlibStream.IsStreamOwner = false;
                                                    }
                                                    long alignOffset = Align(rsgStream.Length);
                                                    rsgStream.SetLength(alignOffset);
                                                    rsgStream.Seek(alignOffset, SeekOrigin.Begin);
                                                    rsgPackInfo.ResidentDataCompressedSize = (uint)(rsgStream.Position - rsgPackInfo.ResidentDataOffset);
                                                }
                                            }
                                            else
                                            {
                                                rsgPackInfo.ResidentFileList = new RsgMetadataResidentFileInfo[groupInfo.ResidentFileList.Length];
                                                residentFileStream.SetLength(0);
                                                residentFileStream.Seek(0, SeekOrigin.Begin);
                                                for (uint j = 0u; j < groupInfo.ResidentFileList.Length; j++)
                                                {
                                                    string? path = groupInfo.ResidentFileList[j].Path;
                                                    if (path != null)
                                                    {
                                                        string resPath = paths.GetResourcePathByGroupIdAndPath(groupInfo.Id, path);
                                                        using (Stream resStream = fileSystem.OpenRead(resPath))
                                                        {
                                                            RsgResidentFileExtraData* extra = (RsgResidentFileExtraData*)map.Find(path);
                                                            extra->Offset = (uint)residentFileStream.Position;
                                                            extra->Size = (uint)resStream.Length;
                                                            rsgPackInfo.ResidentFileList[j] = new RsgMetadataResidentFileInfo
                                                            {
                                                                Path = path,
                                                                ModifyTimeUtc = fileSystem.GetModifyTimeUtc(resPath),
                                                            };
                                                            resStream.CopyTo(residentFileStream);
                                                            long alignOffset = Align(residentFileStream.Length);
                                                            residentFileStream.SetLength(alignOffset);
                                                            residentFileStream.Seek(alignOffset, SeekOrigin.Begin);
                                                        }
                                                    }
                                                }
                                                residentFileStream.Seek(0x0, SeekOrigin.Begin);
                                                if ((rsgPackInfo.CompressionFlags & 0b10) == 0)
                                                {
                                                    // No Zlib
                                                    rsgPackInfo.ResidentDataOffset = (uint)rsgStream.Position;
                                                    rsgPackInfo.ResidentDataUncompressedSize = (uint)residentFileStream.Length;
                                                    residentFileStream.CopyTo(rsgStream);
                                                    long alignOffset = Align(rsgStream.Length);
                                                    rsgStream.SetLength(alignOffset);
                                                    rsgStream.Seek(alignOffset, SeekOrigin.Begin);
                                                    rsgPackInfo.ResidentDataCompressedSize = (uint)(rsgStream.Position - rsgPackInfo.ResidentDataOffset);
                                                }
                                                else
                                                {
                                                    // Zlib
                                                    rsgPackInfo.ResidentDataOffset = (uint)rsgStream.Position;
                                                    rsgPackInfo.ResidentDataUncompressedSize = (uint)residentFileStream.Length;
                                                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(rsgStream, new Deflater(compressionLevel)))
                                                    {
                                                        zlibStream.IsStreamOwner = false;
                                                        residentFileStream.CopyTo(zlibStream);
                                                    }
                                                    long alignOffset = Align(rsgStream.Length);
                                                    rsgStream.SetLength(alignOffset);
                                                    rsgStream.Seek(alignOffset, SeekOrigin.Begin);
                                                    rsgPackInfo.ResidentDataCompressedSize = (uint)(rsgStream.Position - rsgPackInfo.ResidentDataOffset);
                                                }
                                            }
                                            // Image
                                            if (groupInfo.GPUFileList == null)
                                            {
                                                rsgPackInfo.GPUDataOffset = (uint)rsgStream.Position;
                                                rsgPackInfo.GPUDataCompressedSize = 0u;
                                                rsgPackInfo.GPUDataUncompressedSize = 0u;
                                            }
                                            else
                                            {
                                                rsgPackInfo.GPUFileList = new RsgMetadataGPUFileInfo[groupInfo.GPUFileList.Length];
                                                imageStream.SetLength(0);
                                                imageStream.Seek(0, SeekOrigin.Begin);
                                                for (uint j = 0u; j < groupInfo.GPUFileList.Length; j++)
                                                {
                                                    RsbPackGroupGPUFileInfo rsbGroupImageInfo = groupInfo.GPUFileList[j];
                                                    string? path = rsbGroupImageInfo.Path;
                                                    if (path != null)
                                                    {
                                                        string resPath = rsbGroupImageInfo.InFileIndexDataMap ? paths.GetResourcePathByGroupIdAndPath(groupInfo.Id, path) : paths.GetUnusedResourcePathByGroupIdAndPath(groupInfo.Id, path);
                                                        uint imgSize = ptxHandler.GetPtxSize(rsbGroupImageInfo.Width, rsbGroupImageInfo.Height, rsbGroupImageInfo.Pitch, rsbGroupImageInfo.Format, rsbGroupImageInfo.Extend1);
                                                        using (Stream resStream = fileSystem.OpenRead(resPath))
                                                        {
                                                            ResStreamFileGPULocationInfo* extra = (ResStreamFileGPULocationInfo*)map.Find(path);
                                                            if (extra != null)
                                                            {
                                                                extra->Offset = (uint)imageStream.Position;
                                                                extra->Size = imgSize;
                                                            }
                                                            rsgPackInfo.GPUFileList[j] = new RsgMetadataGPUFileInfo
                                                            {
                                                                Path = path,
                                                                InResMap = groupInfo.GPUFileList[j].InFileIndexDataMap,
                                                                ModifyTimeUtc = fileSystem.GetModifyTimeUtc(resPath),
                                                                Width = groupInfo.GPUFileList[j].Width,
                                                                Height = groupInfo.GPUFileList[j].Height,
                                                            };
                                                            if (imgSize <= resStream.Length)
                                                            {
                                                                resStream.CopyLengthTo(imageStream, imgSize);
                                                            }
                                                            else
                                                            {
                                                                resStream.CopyTo(imageStream);
                                                                uint delta = (uint)(imgSize - resStream.Length);
                                                                long offset = imageStream.Length + delta;
                                                                imageStream.SetLength(offset);
                                                                imageStream.Seek(offset, SeekOrigin.Begin);
                                                            }
                                                            long alignOffset = Align(imageStream.Length);
                                                            imageStream.SetLength(alignOffset);
                                                            imageStream.Seek(alignOffset, SeekOrigin.Begin);
                                                        }
                                                    }
                                                }
                                                imageStream.Seek(0x0, SeekOrigin.Begin);
                                                if ((rsgPackInfo.CompressionFlags & 0b1) == 0)
                                                {
                                                    // No Zlib
                                                    rsgPackInfo.GPUDataOffset = (uint)rsgStream.Position;
                                                    rsgPackInfo.GPUDataUncompressedSize = (uint)imageStream.Length;
                                                    imageStream.CopyTo(rsgStream);
                                                    long alignOffset = Align(rsgStream.Length);
                                                    rsgStream.SetLength(alignOffset);
                                                    rsgStream.Seek(alignOffset, SeekOrigin.Begin);
                                                    rsgPackInfo.GPUDataCompressedSize = (uint)(rsgStream.Position - rsgPackInfo.GPUDataOffset);
                                                }
                                                else
                                                {
                                                    // Zlib
                                                    rsgPackInfo.GPUDataOffset = (uint)rsgStream.Position;
                                                    rsgPackInfo.GPUDataUncompressedSize = (uint)imageStream.Length;
                                                    using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(rsgStream, new Deflater(compressionLevel)))
                                                    {
                                                        zlibStream.IsStreamOwner = false;
                                                        imageStream.CopyTo(zlibStream);
                                                    }
                                                    long alignOffset = Align(rsgStream.Length);
                                                    rsgStream.SetLength(alignOffset);
                                                    rsgStream.Seek(alignOffset, SeekOrigin.Begin);
                                                    rsgPackInfo.GPUDataCompressedSize = (uint)(rsgStream.Position - rsgPackInfo.GPUDataOffset);
                                                }
                                            }
                                            rsgInfo->ResidentDataOffset = rsgPackInfo.ResidentDataOffset;
                                            rsgInfo->ResidentDataCompressedSize = rsgPackInfo.ResidentDataCompressedSize;
                                            rsgInfo->ResidentDataUncompressedSize = rsgPackInfo.ResidentDataUncompressedSize;
                                            rsgInfo->GPUDataOffset = rsgPackInfo.GPUDataOffset;
                                            rsgInfo->GPUDataCompressedSize = rsgPackInfo.GPUDataCompressedSize;
                                            rsgInfo->GPUDataUncompressedSize = rsgPackInfo.GPUDataUncompressedSize;
                                            // write header with right endianness
                                            rsgStream.Seek(0, SeekOrigin.Begin);
                                            RsbReverser.ReverseRsgHeader(rsgInfo, Endianness.Native, rsgPackInfo.UseBigEndian ? Endianness.Big : Endianness.Little);
                                            rsgStream.Write(rsgInfo, rsgPackInfo.HeaderSize);
                                        }
                                        WindJsonSerializer.TrySerializeToFile(rsgMetaPath, rsgPackInfo, fileSystem, logger);
                                    }
                                }
                            }
                            //else
                            //{
                            //    logger.Log($"Group {groupInfo.Id} does not need to update", 0);
                            //}
                        }
                    }
                }
            }
        }

        private static uint Align(uint value)
        {
            if ((value & 0xFFF) != 0)
            {
                value |= 0xFFF;
                value++;
            }
            return value;
        }

        private static long Align(long value)
        {
            if ((value & 0xFFF) != 0)
            {
                value |= 0xFFF;
                value++;
            }
            return value;
        }

        private static bool CheckImageModify(RsgMetadataGPUFileInfo[]? rsgImageList, RsbUnpackPathProvider paths, string? groupId, IFileSystem fileSystem)
        {
            if (rsgImageList == null)
            {
                return false;
            }
            for (int i = 0; i < rsgImageList.Length; i++)
            {
                if (rsgImageList[i] != null
                    && rsgImageList[i].Path != null
                    && (!rsgImageList[i].InResMap && rsgImageList[i].ModifyTimeUtc != fileSystem.GetModifyTimeUtc(paths.GetUnusedResourcePathByGroupIdAndPath(groupId, rsgImageList[i].Path)) || rsgImageList[i].InResMap && rsgImageList[i].ModifyTimeUtc != fileSystem.GetModifyTimeUtc(paths.GetResourcePathByGroupIdAndPath(groupId, rsgImageList[i].Path))))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckResidentFileModify(RsgMetadataResidentFileInfo[]? rsgResidentFileList, RsbUnpackPathProvider paths, string? groupId, IFileSystem fileSystem)
        {
            if (rsgResidentFileList == null)
            {
                return false;
            }
            for (int i = 0; i < rsgResidentFileList.Length; i++)
            {
                if (rsgResidentFileList[i] != null && rsgResidentFileList[i].Path != null && rsgResidentFileList[i].ModifyTimeUtc != fileSystem.GetModifyTimeUtc(paths.GetResourcePathByGroupIdAndPath(groupId, rsgResidentFileList[i].Path)))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CompareResidentFileList(RsbPackGroupResidentFileInfo[]? rsbResidentFileList, RsgMetadataResidentFileInfo[]? rsgResidentFileList)
        {
            if (rsbResidentFileList == null && rsgResidentFileList == null)
            {
                return true;
            }
            if (rsbResidentFileList != null && rsgResidentFileList != null && rsbResidentFileList.Length == rsgResidentFileList.Length)
            {
                for (int i = 0; i < rsbResidentFileList.Length; i++)
                {
                    if (rsbResidentFileList[i].Path == null)
                    {
                        if (rsgResidentFileList[i]?.Path != null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (rsgResidentFileList[i] == null
                            || rsbResidentFileList[i].Path != rsgResidentFileList[i].Path)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private static bool CompareImageList(RsbPackGroupGPUFileInfo[]? rsbImageList, RsgMetadataGPUFileInfo[]? rsgImageList)
        {
            if (rsbImageList == null && rsgImageList == null)
            {
                return true;
            }
            if (rsbImageList != null && rsgImageList != null && rsbImageList.Length == rsgImageList.Length)
            {
                for (int i = 0; i < rsbImageList.Length; i++)
                {
                    if (rsbImageList[i].Path == null)
                    {
                        if (rsgImageList[i]?.Path != null)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (rsgImageList[i] == null
                            || rsbImageList[i].Path != rsgImageList[i].Path
                            || rsbImageList[i].InFileIndexDataMap != rsgImageList[i].InResMap
                            || rsbImageList[i].Width != rsgImageList[i].Width
                            || rsbImageList[i].Height != rsgImageList[i].Height)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
