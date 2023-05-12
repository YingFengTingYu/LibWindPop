using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibWindPop.Images.PtxRsb;
using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Packs.Rsb.RsbStructures;
using LibWindPop.PopCap.Packs.Rsb.Map;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibWindPop.Packs.Rsb
{
    public static class RsbUnpacker
    {
        private record struct CompiledMapPair(string Key, nuint Value);

        public static void Unpack(string rsbPath, string unpackPath, IFileSystem fileSystem, ILogger logger, string? ptxHandlerType, bool useGroupFolder, bool throwException)
        {
            ArgumentNullException.ThrowIfNull(rsbPath, nameof(rsbPath));
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get unpack info...", 0);

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, useGroupFolder);
            string? parentPath = fileSystem.GetParentPath(rsbPath); // for external rsg

            Encoding encoding = EncodingType.iso_8859_1.GetEncoding();

            // create content
            RsbPackInfo packInfo = new RsbPackInfo();

            logger.Log("Open rsb file...", 0);
            using (Stream rsbRawStream = fileSystem.OpenRead(rsbPath))
            {
                //logger.Log("Check whole zlib...", 0);
                //uint magic = rsbRawStream.ReadUInt32LE();
                //if (magic == 0xDEADFED4u)
                //{
                //    logger.Log("This rsb file uses whole zlib", 1);
                //    // read size
                //    uint size = rsbRawStream.ReadUInt32LE();
                //    // Create temp file
                //    logger.Log("Create temp file to uncompress...", 1);
                //    // uncompress
                //    using (ITempFile tempFile = fileSystem.CreateTempFile())
                //    {
                //        using (InflaterInputStream zlibStream = new InflaterInputStream(rsbRawStream))
                //        {
                //            zlibStream.IsStreamOwner = false;
                //            zlibStream.CopyTo(tempFile.Stream);
                //        }
                //        UnpackInternal(tempFile.Stream, paths, parentPath, ptxHandlerType, packInfo, fileSystem, logger, encoding, throwException);
                //        packInfo.UseWholeZLib = true;
                //    }
                //}
                //else
                //{
                //    logger.Log("This rsb file does not use whole zlib", 1);
                UnpackInternal(rsbRawStream, paths, parentPath, ptxHandlerType, packInfo, fileSystem, logger, encoding, throwException);
                //    packInfo.UseWholeZLib = false;
                //}
            }

            logger.Log("Save pack info...", 0);
            WindJsonSerializer.TrySerializeToFile(paths.InfoPackInfoPath, packInfo, 0u, fileSystem, logger, throwException);
        }

        private static unsafe void UnpackInternal(Stream rsbStream, RsbUnpackPathProvider paths, string? parentPath, string? ptxHandlerType, RsbPackInfo packInfo, IFileSystem fileSystem, ILogger logger, Encoding encoding, bool throwException)
        {
            logger.Log("Read rsb info...", 0);
            IPtxRsbHandler ptxHandler = PtxRsbHandlerManager.GetHandlerFromId(ptxHandlerType, logger, throwException);
            Span<byte> bufferSpan = stackalloc byte[16];
            //rsbStream.Seek(0, SeekOrigin.Begin);
            rsbStream.Read(bufferSpan);
            uint magic = BinaryPrimitives.ReadUInt32LittleEndian(bufferSpan);
            if (magic != 0x31627372u && magic != 0x72736231u)
            {
                if (magic == 0xDEADFED4u)
                {
                    logger.LogError($"Rsb magic mismatch: 1bsr(LE)/rsb1(BE) expected but value is 0x{magic:X8}. If this rsb is smf file, please use PopCapZlibCompressor.Uncompress to uncompress it.", 1, throwException);
                }
                else
                {
                    logger.LogError($"Rsb magic mismatch: 1bsr(LE)/rsb1(BE) expected but value is 0x{magic:X8}", 1, throwException);
                }
            }
            bool useBigEndian = magic != 0x72736231u;
            // Read header size
            uint headerSize = useBigEndian
                ? BinaryPrimitives.ReadUInt32BigEndian(bufferSpan[12..])
                : BinaryPrimitives.ReadUInt32LittleEndian(bufferSpan[12..]);
            logger.Log("Read rsb header...", 0);
            // read header
            using (NativeMemoryOwner rsbHeaderMemoryAllocator = new NativeMemoryOwner(headerSize))
            {
                nuint headerPtrNumber = (nuint)rsbHeaderMemoryAllocator.Pointer;
                rsbStream.Seek(0, SeekOrigin.Begin);
                rsbStream.Read(headerPtrNumber, headerSize);
                bool reverseEndian = (magic == 0x72736231u) ^ BitConverter.IsLittleEndian;
                RsbInfo* rsbInfo = (RsbInfo*)headerPtrNumber;
                if (reverseEndian)
                {
                    logger.Log("Reverse rsb header endianness...", 0);
                    RsbReverser.ReverseRsbHeader(rsbInfo, Endianness.NoneNative, Endianness.Native);
                }
                bool externalRsg = rsbStream.Length == rsbInfo->HeaderSize;
                packInfo.Version = rsbInfo->Version;
                packInfo.MinorVersion = rsbInfo->MinorVersion;
                packInfo.UseBigEndian = useBigEndian;
                packInfo.UseExternalRsg = externalRsg;
                packInfo.UseGlobalFileIndexMap = rsbInfo->GlobalFileIndexMapSize != 0xFFFFFFFFu;
                packInfo.UseManifest = rsbInfo->ManifestGroupInfoOffset != 0u;
                packInfo.UseGroupFolder = paths.UseGroupFolder;
                packInfo.TextureDescriptorPitch = rsbInfo->TextureDescriptorPitch;
                packInfo.ImageHandler = ptxHandlerType;
                if (rsbInfo->ManifestGroupInfoOffset != 0u)
                {
                    logger.Log("Unpack rsb manifest...", 0);
                    UnpackRsbManifest(rsbInfo->Version, headerPtrNumber + rsbInfo->ManifestGroupInfoOffset, headerPtrNumber + rsbInfo->ManifestResourceInfoOffset, headerPtrNumber + rsbInfo->ManifestStringPoolOffset, paths.InfoManifestPath, fileSystem, logger, encoding, throwException);
                }
                nuint currentPtrNumber;
                string[] groupIdList = new string[rsbInfo->GroupCount];
                string[] compositeIdList = new string[rsbInfo->CompositeCount];
                CompiledMap map = new CompiledMap();
                logger.Log("Read sub group map...", 0);
                map.Init(headerPtrNumber + rsbInfo->GroupIndexMapOffset, rsbInfo->GroupIndexMapSize);
                map.ForEach((string groupId, nuint groupInfoPtrNumber) =>
                {
                    uint index = *(uint*)groupInfoPtrNumber;
                    if (index < groupIdList.Length)
                    {
                        groupIdList[index] = groupId;
                    }
                    else
                    {
                        logger.LogWarning($"SubGroup index out of range: [0,{groupIdList.Length}) expected but value is {index}", 0);
                    }
                });
                logger.Log("Read composite group map...", 0);
                map.Init(headerPtrNumber + rsbInfo->CompositeIndexMapOffset, rsbInfo->CompositeIndexMapSize);
                map.ForEach((string compositeId, nuint compositeInfoPtrNumber) =>
                {
                    uint index = *(uint*)compositeInfoPtrNumber;
                    if (index < compositeIdList.Length)
                    {
                        compositeIdList[index] = compositeId;
                    }
                    else
                    {
                        logger.LogWarning($"CompositeGroup index out of range: [0,{compositeIdList.Length}) expected but value is {index}", 0);
                    }
                });

                logger.Log("Read res streams pool info...", 0);
                RsbPackPoolInfo[] pools = new RsbPackPoolInfo[rsbInfo->PoolCount];
                currentPtrNumber = headerPtrNumber + rsbInfo->PoolDescriptorOffset;
                for (int i = 0; i < pools.Length; i++)
                {
                    RsbPoolDescriptor* poolInfo = (RsbPoolDescriptor*)currentPtrNumber;
                    currentPtrNumber += rsbInfo->PoolDescriptorPitch;
                    RsbPackPoolInfo pool = new RsbPackPoolInfo();
                    pool.Index = (uint)i;
                    pool.Name = UnsafeStringHelper.GetUtf16String((nuint)poolInfo->Name, 0x80, encoding);
                    pool.BufferCount = poolInfo->BufferCount;
                    pools[i] = pool;
                }
                packInfo.Pools = pools;

                logger.Log("Read composite group info...", 0);
                RsbPackCompositeInfo[] composites = new RsbPackCompositeInfo[rsbInfo->CompositeCount];
                currentPtrNumber = headerPtrNumber + rsbInfo->CompositeDescriptorOffset;
                if (rsbInfo->Version < 3u)
                {
                    for (int i = 0; i < composites.Length; i++)
                    {
                        RsbCompositeDescriptorV1* compositeInfo = (RsbCompositeDescriptorV1*)currentPtrNumber;
                        currentPtrNumber += rsbInfo->CompositeDescriptorPitch;
                        RsbPackCompositeInfo composite = new RsbPackCompositeInfo();
                        composite.Id = compositeIdList[i];
                        composite.Name = UnsafeStringHelper.GetUtf16String((nuint)compositeInfo->Name, 0x80, encoding);
                        RsbPackSubGroupInfo[] compositeGroups = new RsbPackSubGroupInfo[compositeInfo->SubGroupCount];
                        RsbSubGroupV1* compositeGroupInfo = (RsbSubGroupV1*)((nuint)compositeInfo + 0x80);
                        for (int j = 0; j < compositeGroups.Length; j++)
                        {
                            RsbPackSubGroupInfo compositeGroup = new RsbPackSubGroupInfo();
                            if (compositeGroupInfo->GroupIndex >= groupIdList.Length)
                            {
                                logger.LogWarning($"Group index out of range: [0,{groupIdList.Length}) expected but value is {compositeGroupInfo->GroupIndex}", 0);
                                compositeGroup.Id = null;
                            }
                            else
                            {
                                compositeGroup.Id = groupIdList[compositeGroupInfo->GroupIndex];
                            }
                            compositeGroup.Res = compositeGroupInfo->Res;
                            compositeGroup.Loc = null;
                            compositeGroupInfo++;
                            compositeGroups[j] = compositeGroup;
                        }
                        composite.SubGroups = compositeGroups;
                        composites[i] = composite;
                    }
                }
                else
                {
                    for (int i = 0; i < composites.Length; i++)
                    {
                        RsbCompositeDescriptorV3V4* compositeInfo = (RsbCompositeDescriptorV3V4*)currentPtrNumber;
                        currentPtrNumber += rsbInfo->CompositeDescriptorPitch;
                        RsbPackCompositeInfo composite = new RsbPackCompositeInfo();
                        composite.Id = compositeIdList[i];
                        composite.Name = UnsafeStringHelper.GetUtf16String((nuint)compositeInfo->Name, 0x80, encoding);
                        RsbPackSubGroupInfo[] compositeGroups = new RsbPackSubGroupInfo[compositeInfo->SubGroupCount];
                        RsbSubGroupV3V4* compositeGroupInfo = (RsbSubGroupV3V4*)((nuint)compositeInfo + 0x80);
                        for (int j = 0; j < compositeGroups.Length; j++)
                        {
                            RsbPackSubGroupInfo compositeGroup = new RsbPackSubGroupInfo();
                            if (compositeGroupInfo->GroupIndex >= groupIdList.Length)
                            {
                                logger.LogWarning($"Group index out of range: [0,{groupIdList.Length}) expected but value is {compositeGroupInfo->GroupIndex}", 0);
                                compositeGroup.Id = null;
                            }
                            else
                            {
                                compositeGroup.Id = groupIdList[compositeGroupInfo->GroupIndex];
                            }
                            compositeGroup.Res = compositeGroupInfo->Res;
                            compositeGroup.Loc = UInt32StringConvertor.UInt32ToString(compositeGroupInfo->Loc);
                            compositeGroupInfo++;
                            compositeGroups[j] = compositeGroup;
                        }
                        composite.SubGroups = compositeGroups;
                        composites[i] = composite;
                    }
                }
                packInfo.Composites = composites;

                // peek rsg size
                logger.Log("Peek rsg max header size...", 0);
                uint maxRsgHeaderSize = 0u;
                uint maxRsgImageCount = 0u;
                uint[] rsgSizeList = new uint[groupIdList.Length];
                (uint index, uint offset)[] rsgSortList = new (uint index, uint offset)[groupIdList.Length];
                currentPtrNumber = headerPtrNumber + rsbInfo->GroupDescriptorOffset;
                for (int i = 0; i < rsgSortList.Length; i++)
                {
                    RsbGroupDescriptor* groupInfo = (RsbGroupDescriptor*)currentPtrNumber;
                    currentPtrNumber += rsbInfo->GroupDescriptorPitch;
                    rsgSortList[i] = ((uint)i, groupInfo->RsgOffset);
                    maxRsgHeaderSize = Math.Max(maxRsgHeaderSize, groupInfo->RsgHeaderSize);
                    maxRsgImageCount = Math.Max(
                        maxRsgImageCount,
                        rsbInfo->Version <= 1
                            ? ((RsbPoolDescriptor*)(headerPtrNumber + rsbInfo->PoolDescriptorOffset + rsbInfo->PoolDescriptorPitch * groupInfo->PoolIndex))->TextureCount
                            : groupInfo->TextureCount
                        );
                }
                Array.Sort(rsgSortList, ((uint index, uint offset) a1, (uint index, uint offset) a2) =>
                {
                    return a1.offset > a2.offset ? 1 : (a1.offset == a2.offset ? 0 : -1);
                });
                for (int i = 0; i < rsgSortList.Length; i++)
                {
                    uint index = rsgSortList[i].index;
                    uint begin = rsgSortList[i].offset;
                    uint end = (i == (rsgSortList.Length - 1)) ? (uint)rsbStream.Length : rsgSortList[i + 1].offset;
                    rsgSizeList[i] = end - begin;
                }
                RsbPackGroupInfo[] groups = new RsbPackGroupInfo[rsbInfo->GroupCount];
                currentPtrNumber = headerPtrNumber + rsbInfo->GroupDescriptorOffset;
                using (NativeMemoryOwner rsgHeaderMemoryAllocator = new NativeMemoryOwner(maxRsgHeaderSize))
                {
                    nuint rsgHeaderPtrNumber = (nuint)rsgHeaderMemoryAllocator.Pointer;
                    RsgInfo* rsgInfo = (RsgInfo*)rsgHeaderPtrNumber;
                    List<CompiledMapPair> residentFileList = new List<CompiledMapPair>();
                    List<CompiledMapPair> imageList = new List<CompiledMapPair>();
                    using ITempFile decompressedTempFile = fileSystem.CreateTempFile();
                    Stream decompressedStream = decompressedTempFile.Stream;
                    uint* imageSizeList = stackalloc uint[(int)maxRsgImageCount];
                    uint* imageOffsetList = stackalloc uint[(int)maxRsgImageCount];
                    logger.Log("Unpack rsg and resource...", 0);
                    for (int i = 0; i < groups.Length; i++)
                    {
                        logger.Log($"Unpack group {groupIdList[i]}...", 1);
                        logger.Log($"Read group info...", 2);
                        RsbGroupDescriptor* groupInfo = (RsbGroupDescriptor*)currentPtrNumber;
                        currentPtrNumber += rsbInfo->GroupDescriptorPitch;
                        RsbPackGroupInfo group = new()
                        {
                            Id = groupIdList[i],
                            Name = UnsafeStringHelper.GetUtf16String((nuint)groupInfo->Name, 0x80, encoding),
                            PoolIndex = groupInfo->PoolIndex,
                            CompressionFlags = groupInfo->RsgCompressionFlags
                        };
                        RsgMetadata rsgPackInfo = new()
                        {
                            Version = rsbInfo->Version,
                            UseBigEndian = useBigEndian,
                            CompressionFlags = groupInfo->RsgCompressionFlags,
                            ImageHandler = ptxHandlerType,
                            HeaderSize = groupInfo->RsgHeaderSize,
                            ResidentDataOffset = groupInfo->RsgResidentDataOffset,
                            ResidentDataCompressedSize = groupInfo->RsgResidentDataCompressedSize,
                            ResidentDataUncompressedSize = groupInfo->RsgResidentDataUncompressedSize,
                            ResidentFileList = null,
                            GPUDataOffset = groupInfo->RsgGPUDataOffset,
                            GPUDataCompressedSize = groupInfo->RsgGPUDataCompressedSize,
                            GPUDataUncompressedSize = groupInfo->RsgGPUDataUncompressedSize,
                            GPUFileList = null,
                        };
                        // compute image len
                        uint imageSize = 0u;
                        uint imageCount, imageOffset;
                        if (rsbInfo->Version <= 1)
                        {
                            RsbPoolDescriptor* poolInfo = (RsbPoolDescriptor*)(headerPtrNumber + rsbInfo->PoolDescriptorOffset + rsbInfo->PoolDescriptorPitch * groupInfo->PoolIndex);
                            imageCount = poolInfo->TextureCount;
                            imageOffset = poolInfo->TextureDescriptorStartIndex;
                        }
                        else
                        {
                            imageCount = groupInfo->TextureCount;
                            imageOffset = groupInfo->TextureDescriptorStartIndex;
                        }
                        for (uint j = 0u; j < imageCount; j++)
                        {
                            RsbTextureDescriptor* imagePtr = (RsbTextureDescriptor*)(headerPtrNumber + rsbInfo->TextureDescriptorOffset + rsbInfo->TextureDescriptorPitch * (imageOffset + j));
                            imageOffsetList[j] = j == 0u ? 0u : Align(imageOffsetList[j - 1] + imageSizeList[j - 1]);
                            imageSizeList[j] = ptxHandler.GetPtxSize(imagePtr->Width, imagePtr->Height, imagePtr->Pitch, imagePtr->Format, rsbInfo->TextureDescriptorPitch >= 0x14u ? imagePtr->Extend0x10 : 0u);
                        }
                        if (imageCount > 0u)
                        {
                            imageSize = Align(imageOffsetList[imageCount - 1] + imageSizeList[imageCount - 1]);
                        }
                        // extract rsg
                        logger.Log($"Extract rsg...", 2);
                        string rsgPath = paths.GetRsgPathByGroupId(groupIdList[i]);
                        string rsgMetaPath = paths.AppendMetaExtension(rsgPath);
                        try
                        {
                            string? externPath = null;
                            if (externalRsg)
                            {
                                // find from file system
                                string? externRsgPath = fileSystem.Combine(parentPath, group.Name + ".rsg");
                                string? externRsgSmfPath = fileSystem.Combine(parentPath, group.Name + ".rsg.smf");
                                if (externRsgPath != null && fileSystem.FileExists(externRsgPath))
                                {
                                    externPath = externRsgPath;
                                }
                                else if (externRsgSmfPath != null && fileSystem.FileExists(externRsgSmfPath))
                                {
                                    externPath = externRsgSmfPath;
                                }
                                else
                                {
                                    logger.LogWarning($"Can not find rsg {group.Id}", 3);
                                    continue;
                                }
                            }
                            using (Stream rsgStream = fileSystem.Create(rsgPath))
                            {
                                if (externalRsg)
                                {
                                    using (Stream externRsgStream = fileSystem.OpenRead(externPath!))
                                    {
                                        externRsgStream.CopyTo(rsgStream);
                                    }
                                }
                                else
                                {
                                    rsbStream.Seek(groupInfo->RsgOffset, SeekOrigin.Begin);
                                    rsbStream.CopyLengthTo(rsgStream, rsgSizeList[i]);
                                }
                                logger.Log($"Read rsg header...", 2);
                                rsgStream.Seek(0, SeekOrigin.Begin);
                                rsgStream.Read(rsgInfo, groupInfo->RsgHeaderSize);
                                if (reverseEndian)
                                {
                                    logger.Log("Reverse rsg header endianness...", 2);
                                    RsbReverser.ReverseRsgHeader(rsgInfo, Endianness.NoneNative, Endianness.Native);
                                }
                                logger.Log($"Read resource map...", 2);
                                map.Init(rsgHeaderPtrNumber + rsgInfo->FileIndexDataMapOffset, rsgInfo->FileIndexDataMapSize);
                                residentFileList.Clear();
                                imageList.Clear();
                                uint residentFileSize = 0u;
                                map.ForEach((string resourceName, nuint resourceInfo) =>
                                {
                                    uint type = *(uint*)resourceInfo;
                                    if (type == 0u)
                                    {
                                        RsgResidentFileExtraData* extra = (RsgResidentFileExtraData*)resourceInfo;
                                        residentFileSize = Math.Max(residentFileSize, extra->Offset + extra->Size);
                                        residentFileList.Add(new CompiledMapPair(resourceName, resourceInfo));
                                    }
                                    else if (type == 1u)
                                    {
                                        RsgImageExtraData* extra = (RsgImageExtraData*)resourceInfo;
                                        imageList.Add(new CompiledMapPair(resourceName, resourceInfo));
                                    }
                                    else
                                    {
                                        logger.LogError($"Unknow resource type {type} at {resourceName}", 3, throwException);
                                    }
                                });
                                if (residentFileList.Count > 0)
                                {
                                    logger.Log($"Extract resident files...", 2);
                                    residentFileList.Sort((CompiledMapPair p1, CompiledMapPair p2) =>
                                    {
                                        uint p1Offset = ((RsgResidentFileExtraData*)p1.Value)->Offset;
                                        uint p2Offset = ((RsgResidentFileExtraData*)p2.Value)->Offset;
                                        return p1Offset > p2Offset ? 1 : (p1Offset == p2Offset ? 0 : -1);
                                    });
                                    RsbPackGroupResidentFileInfo[] residentFiles = new RsbPackGroupResidentFileInfo[residentFileList.Count];
                                    RsgMetadataResidentFileInfo[] rsgResidentFiles = new RsgMetadataResidentFileInfo[residentFileList.Count];
                                    Stream readStream;
                                    uint readOffset;
                                    if ((groupInfo->RsgCompressionFlags & 0b10) != 0)
                                    {
                                        rsgStream.Seek(groupInfo->RsgResidentDataOffset, SeekOrigin.Begin);
                                        decompressedStream.Seek(0, SeekOrigin.Begin);
                                        using (InflaterInputStream zlibStream = new InflaterInputStream(rsgStream, new Inflater()))
                                        {
                                            zlibStream.IsStreamOwner = false;
                                            zlibStream.CopyLengthTo(decompressedStream, residentFileSize);
                                        }
                                        readStream = decompressedStream;
                                        readOffset = 0u;
                                    }
                                    else
                                    {
                                        readStream = rsgStream;
                                        readOffset = groupInfo->RsgResidentDataOffset;
                                    }
                                    for (int j = 0; j < residentFileList.Count; j++)
                                    {
                                        logger.Log($"Extract resident file {residentFileList[j].Key}...", 3);
                                        try
                                        {
                                            RsgResidentFileExtraData* extra = (RsgResidentFileExtraData*)residentFileList[j].Value;
                                            string path = paths.GetResourcePathByGroupIdAndPath(groupIdList[i], residentFileList[j].Key);
                                            using (Stream resourceStream = fileSystem.Create(path))
                                            {
                                                readStream.Seek(readOffset + extra->Offset, SeekOrigin.Begin);
                                                readStream.CopyLengthTo(resourceStream, extra->Size);
                                            }
                                            residentFiles[j] = new RsbPackGroupResidentFileInfo
                                            {
                                                Path = residentFileList[j].Key,
                                            };
                                            rsgResidentFiles[j] = new RsgMetadataResidentFileInfo
                                            {
                                                Path = residentFileList[j].Key,
                                                ModifyTimeUtc = fileSystem.GetModifyTimeUtc(path),
                                            };
                                        }
                                        catch (Exception ex) when (ex is not LoggerException)
                                        {
                                            logger.LogException(ex, 4, throwException);
                                        }
                                    }
                                    group.ResidentFileList = residentFiles;
                                    rsgPackInfo.ResidentFileList = rsgResidentFiles;
                                }
                                if (imageCount > 0u)
                                {
                                    logger.Log($"Extract images...", 2);
                                    RsbPackGroupGPUFileInfo[] images = new RsbPackGroupGPUFileInfo[imageCount];
                                    RsgMetadataGPUFileInfo[] rsgImages = new RsgMetadataGPUFileInfo[imageCount];
                                    Stream readStream;
                                    uint readOffset;
                                    if ((groupInfo->RsgCompressionFlags & 0b1) != 0)
                                    {
                                        rsgStream.Seek(groupInfo->RsgGPUDataOffset, SeekOrigin.Begin);
                                        decompressedStream.Seek(0, SeekOrigin.Begin);
                                        using (InflaterInputStream zlibStream = new InflaterInputStream(rsgStream, new Inflater()))
                                        {
                                            zlibStream.IsStreamOwner = false;
                                            zlibStream.CopyLengthTo(decompressedStream, imageSize);
                                        }
                                        readStream = decompressedStream;
                                        readOffset = 0u;
                                    }
                                    else
                                    {
                                        readStream = rsgStream;
                                        readOffset = groupInfo->RsgGPUDataOffset;
                                    }
                                    nuint imgInfoBaseOffset = headerPtrNumber + rsbInfo->TextureDescriptorOffset + rsbInfo->TextureDescriptorPitch * imageOffset;
                                    for (uint j = 0; j < imageCount; j++)
                                    {
                                        RsbTextureDescriptor* imgInfo = (RsbTextureDescriptor*)imgInfoBaseOffset;
                                        imgInfoBaseOffset += rsbInfo->TextureDescriptorPitch;
                                        try
                                        {
                                            CompiledMapPair? pair = null;
                                            for (int k = 0; k < imageList.Count; k++)
                                            {
                                                if (((RsgImageExtraData*)imageList[k].Value)->Index == j)
                                                {
                                                    pair = imageList[k];
                                                    break;
                                                }
                                            }
                                            string recordPath, path;
                                            if (pair.HasValue)
                                            {
                                                recordPath = pair.Value.Key;
                                                path = paths.GetResourcePathByGroupIdAndPath(groupIdList[i], recordPath);
                                            }
                                            else
                                            {
                                                path = paths.GetUnusedResourcePathByGroupIdAndIndex(groupIdList[i], j, out recordPath);
                                            }
                                            logger.Log($"Extract image {recordPath}...", 3);
                                            using (Stream resourceStream = fileSystem.Create(path))
                                            {
                                                readStream.Seek(readOffset + imageOffsetList[j], SeekOrigin.Begin);
                                                readStream.CopyLengthTo(resourceStream, imageSizeList[j]);
                                            }
                                            RsbPackGroupGPUFileInfo rsbGroupImageInfo = new RsbPackGroupGPUFileInfo
                                            {
                                                Path = recordPath,
                                                InFileIndexDataMap = pair != null,
                                                Width = imgInfo->Width,
                                                Height = imgInfo->Height,
                                                Pitch = imgInfo->Pitch,
                                                Format = imgInfo->Format,
                                                Extend1 = rsbInfo->TextureDescriptorPitch >= 0x14u ? imgInfo->Extend0x10 : 0u,
                                                Extend2 = rsbInfo->TextureDescriptorPitch >= 0x18u ? imgInfo->Extend0x14 : 0u
                                            };
                                            images[j] = rsbGroupImageInfo;
                                            rsgImages[j] = new RsgMetadataGPUFileInfo
                                            {
                                                Path = recordPath,
                                                InResMap = pair != null,
                                                Width = imgInfo->Width,
                                                Height = imgInfo->Height,
                                                ModifyTimeUtc = fileSystem.GetModifyTimeUtc(path),
                                            };
                                        }
                                        catch (Exception ex) when (ex is not LoggerException)
                                        {
                                            logger.LogException(ex, 3, throwException);
                                        }
                                    }
                                    group.GPUFileList = images;
                                    rsgPackInfo.GPUFileList = rsgImages;
                                }
                            }
                            WindJsonSerializer.TrySerializeToFile(rsgMetaPath, rsgPackInfo, 0u, fileSystem, logger, throwException);
                        }
                        catch (Exception ex) when (ex is not LoggerException)
                        {
                            logger.LogException(ex, 3, throwException);
                            continue;
                        }
                        groups[i] = group;
                    }
                    packInfo.Groups = groups;
                }
            }
            // Add Content Pipeline
            logger.Log("Create content pipeline...", 0);
            try
            {
                if (File.Exists(paths.InfoContentPipelinePath))
                {
                    File.Delete(paths.InfoContentPipelinePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex, 0, throwException);
            }
            RsbContentPipelineManager.AddContentPipeline(paths.UnpackPath, nameof(UpdateRsgCache), fileSystem, logger, throwException);
        }

        private static unsafe void UnpackRsbManifest<TFileSystem, TLogger>(uint version, nuint groupPtrNumber, nuint resPtrNumber, nuint poolPtrNumber, string outPath, TFileSystem fileSystem, TLogger logger, Encoding encoding, bool throwException)
            where TFileSystem : IFileSystem
            where TLogger : ILogger
        {
            RsbManifestInfo manifestInfo = new RsbManifestInfo();
            // peek composite number
            uint compositeCount = 0, resCount;
            nuint currentGroupPtrNumber = groupPtrNumber;
            ManifestCompositeGroupInfo* compositePtr;
            ManifestSubGroupInfoForRsbV3V4* groupPtrV3V4;
            ManifestSubGroupInfoForRsbV1* groupPtrV1;
            ManifestResourceHeader* resHeaderPtr;
            ManifestResourceImageProperty* imgPropPtr;
            ManifestResourceUniversalProperty* uniPropPtr;
            while (currentGroupPtrNumber < resPtrNumber)
            {
                compositePtr = (ManifestCompositeGroupInfo*)currentGroupPtrNumber;
                currentGroupPtrNumber += (uint)sizeof(ManifestCompositeGroupInfo);
                for (uint i = 0; i < compositePtr->SubGroupCount; i++)
                {
                    if (version < 3u)
                    {
                        groupPtrV1 = (ManifestSubGroupInfoForRsbV1*)currentGroupPtrNumber;
                        currentGroupPtrNumber += compositePtr->SubGroupInfoEachSize;
                        currentGroupPtrNumber += 4 * groupPtrV1->ResourceCount;
                    }
                    else
                    {
                        groupPtrV3V4 = (ManifestSubGroupInfoForRsbV3V4*)currentGroupPtrNumber;
                        currentGroupPtrNumber += compositePtr->SubGroupInfoEachSize;
                        currentGroupPtrNumber += 4 * groupPtrV3V4->ResourceCount;
                    }
                }
                compositeCount++;
            }
            RsbManifestCompositeInfo[] composites = new RsbManifestCompositeInfo[compositeCount];
            manifestInfo.CompositeGroups = composites;
            currentGroupPtrNumber = groupPtrNumber;
            for (uint i = 0; i < compositeCount; i++)
            {
                RsbManifestCompositeInfo composite = new RsbManifestCompositeInfo();
                compositePtr = (ManifestCompositeGroupInfo*)currentGroupPtrNumber;
                currentGroupPtrNumber += (uint)sizeof(ManifestCompositeGroupInfo);
                composite.Id = UnsafeStringHelper.GetUtf16String(poolPtrNumber + compositePtr->IdOffset, encoding);
                RsbManifestGroupInfo[] groups = new RsbManifestGroupInfo[compositePtr->SubGroupCount];
                composite.SubGroups = groups;
                for (uint j = 0; j < compositePtr->SubGroupCount; j++)
                {
                    RsbManifestGroupInfo group = new RsbManifestGroupInfo();
                    if (version < 3u)
                    {
                        groupPtrV1 = (ManifestSubGroupInfoForRsbV1*)currentGroupPtrNumber;
                        currentGroupPtrNumber += compositePtr->SubGroupInfoEachSize;
                        group.Id = UnsafeStringHelper.GetUtf16String(poolPtrNumber + groupPtrV1->IdOffset, encoding);
                        group.Res = groupPtrV1->Res;
                        group.Loc = null;
                        resCount = groupPtrV1->ResourceCount;
                    }
                    else
                    {
                        groupPtrV3V4 = (ManifestSubGroupInfoForRsbV3V4*)currentGroupPtrNumber;
                        currentGroupPtrNumber += compositePtr->SubGroupInfoEachSize;
                        group.Id = UnsafeStringHelper.GetUtf16String(poolPtrNumber + groupPtrV3V4->IdOffset, encoding);
                        group.Res = groupPtrV3V4->Res;
                        group.Loc = UInt32StringConvertor.UInt32ToString(groupPtrV3V4->Loc);
                        resCount = groupPtrV3V4->ResourceCount;
                    }
                    RsbManifestResourceInfo[] resources = new RsbManifestResourceInfo[resCount];
                    group.Resources = resources;
                    for (uint k = 0; k < resCount; k++)
                    {
                        RsbManifestResourceInfo resource = new RsbManifestResourceInfo();
                        resHeaderPtr = (ManifestResourceHeader*)(resPtrNumber + *(uint*)currentGroupPtrNumber);
                        currentGroupPtrNumber += 4;
                        resource.Type = resHeaderPtr->Type;
                        resource.Id = UnsafeStringHelper.GetUtf16String(poolPtrNumber + resHeaderPtr->IdOffset, encoding);
                        resource.Path = UnsafeStringHelper.GetUtf16String(poolPtrNumber + resHeaderPtr->PathOffset, encoding);
                        if (resHeaderPtr->ImagePropertyOffset == 0u)
                        {
                            resource.ImageProperties = null;
                        }
                        else
                        {
                            RsbManifestImageProperty image = new RsbManifestImageProperty();
                            imgPropPtr = (ManifestResourceImageProperty*)(resPtrNumber + resHeaderPtr->ImagePropertyOffset);
                            image.Atlas = imgPropPtr->Atlas;
                            image.AtlasFlags = imgPropPtr->AtlasFlags;
                            image.OffsetX = imgPropPtr->OffsetX;
                            image.OffsetY = imgPropPtr->OffsetY;
                            image.AtlasX = imgPropPtr->AtlasX;
                            image.AtlasY = imgPropPtr->AtlasY;
                            image.AtlasWidth = imgPropPtr->AtlasWidth;
                            image.AtlasHeight = imgPropPtr->AtlasHeight;
                            image.Rows = imgPropPtr->Rows;
                            image.Cols = imgPropPtr->Cols;
                            image.Parent = UnsafeStringHelper.GetUtf16String(poolPtrNumber + imgPropPtr->ParentOffset, encoding);
                            resource.ImageProperties = image;
                        }
                        if (resHeaderPtr->UniversalPropertyCount == 0u)
                        {
                            resource.UniversalProperties = null;
                        }
                        else
                        {
                            Dictionary<string, string> universal = new Dictionary<string, string>();
                            uniPropPtr = (ManifestResourceUniversalProperty*)(resPtrNumber + resHeaderPtr->UniversalPropertyOffset);
                            for (uint l = 0; l < resHeaderPtr->UniversalPropertyCount; l++)
                            {
                                universal.Add(UnsafeStringHelper.GetUtf16String(poolPtrNumber + uniPropPtr->KeyOffset, encoding), UnsafeStringHelper.GetUtf16String(poolPtrNumber + uniPropPtr->ValueOffset, encoding));
                                uniPropPtr++;
                            }
                            resource.UniversalProperties = universal;
                        }
                        resources[k] = resource;
                    }
                    groups[j] = group;
                }
                composites[i] = composite;
            }
            WindJsonSerializer.TrySerializeToFile(outPath, manifestInfo, 0u, fileSystem, logger, throwException);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Align(uint value)
        {
            if ((value & 0xFFFu) != 0u)
            {
                value |= 0xFFFu;
                value++;
            }
            return value;
        }
    }
}
