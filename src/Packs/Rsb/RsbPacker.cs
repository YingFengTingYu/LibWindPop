using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Packs.Rsb.RsbStructures;
using LibWindPop.PopCap.Packs.Rsb.Map;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibWindPop.Packs.Rsb
{
    public static class RsbPacker
    {
        public static void Pack(string unpackPath, string rsbPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(rsbPath, nameof(rsbPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            // Update cache
            logger.Log("Building content pipeline...");
            RsbContentPipelineManager.StartBuildContentPipeline(unpackPath, fileSystem, logger);

            logger.Log("Get pack info...");

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);
            string? parentPath = fileSystem.GetParentPath(rsbPath); // for external rsg

            Encoding encoding = EncodingType.iso_8859_1.GetEncoding();

            RsbPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<RsbPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);
            if (packInfo == null)
            {
                logger.LogError("Pack info is null");
            }
            else if (packInfo.Composites == null)
            {
                logger.LogError("Composite group info is null");
            }
            else if (packInfo.Groups == null)
            {
                logger.LogError("Sub group info is null");
            }
            else
            {
                if (packInfo.UseGroupFolder)
                {
                    paths = new RsbUnpackPathProvider(unpackPath, fileSystem, true);
                }
                //if (packInfo.UseWholeZLib)
                //{
                //    logger.Log("Create temp file for compressing...", 1);
                //    using (ITempFile tempFile = fileSystem.CreateTempFile())
                //    {
                //        PackInternal(tempFile.Stream, paths, parentPath, packInfo, fileSystem, logger, encoding, throwException);
                //        using (Stream rsbStream = fileSystem.Create(rsbPath))
                //        {
                //            rsbStream.WriteUInt32LE(0xDEADFED4u);
                //            rsbStream.WriteUInt32LE((uint)tempFile.Stream.Length);
                //            using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(rsbStream, new Deflater(8)))
                //            {
                //                zlibStream.IsStreamOwner = false;
                //                tempFile.Stream.Seek(0, SeekOrigin.Begin);
                //                tempFile.Stream.CopyTo(zlibStream);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                using (Stream rsbStream = fileSystem.Create(rsbPath))
                {
                    PackInternal(rsbStream, paths, parentPath, packInfo, fileSystem, logger, encoding);
                }
                //}
            }

            RsbContentPipelineManager.EndBuildContentPipeline(rsbPath, unpackPath, fileSystem, logger);
        }

        private static unsafe void PackInternal(Stream rsbStream, RsbUnpackPathProvider paths, string? parentPath, RsbPackInfo packInfo, IFileSystem fileSystem, ILogger logger, Encoding encoding)
        {
            if (packInfo.Composites != null && packInfo.Groups != null)
            {
                List<RsbPackPoolInfo> poolList = new List<RsbPackPoolInfo>(512);
                List<RsbPackGroupInfo> groupList = new List<RsbPackGroupInfo>(512);
                if (packInfo.Pools != null)
                {
                    for (int i = 0; i < packInfo.Pools.Length; i++)
                    {
                        poolList.Add(packInfo.Pools[i]);
                    }
                }
                int startPoolCount = poolList.Count;
                for (int i = 0; i < packInfo.Groups.Length; i++)
                {
                    RsbPackGroupInfo? groupInfo = packInfo.Groups[i];
                    if (groupInfo != null)
                    {
                        groupList.Add(groupInfo);
                        bool match = false;
                        for (int j = 0; j < startPoolCount; j++)
                        {
                            if (poolList[j].Id == groupInfo.PoolId)
                            {
                                match = true;
                                groupInfo.PoolId = (uint)j;
                                break;
                            }
                        }
                        if (!match)
                        {
                            groupInfo.PoolId = (uint)poolList.Count;
                            poolList.Add(new RsbPackPoolInfo { Name = $"{groupInfo.Name}_AutoPool", Id = uint.MaxValue, NumInstances = 1, PoolFlags = 0 });
                        }
                    }
                }
                // Get raw data
                uint in_mem_len = 0u;
                uint pool_offset = 0u;
                uint resource_pair_count = 0u;
                uint group_pair_count = 0u;
                uint composite_pair_count = 0u;
                uint image_count = 0u;
                for (int i = packInfo.Composites.Length - 1; i >= 0; i--)
                {
                    string? id = packInfo.Composites[i].Id;
                    if (id != null)
                    {
                        in_mem_len += CompiledMapEncodePair.PeekSize(id, sizeof(uint)) + (uint)sizeof(CompiledMapEncodePair);
                        pool_offset += (uint)sizeof(CompiledMapEncodePair);
                        composite_pair_count++;
                    }
                }
                for (int i = groupList.Count - 1; i >= 0; i--)
                {
                    string? id = groupList[i].Id;
                    if (id != null)
                    {
                        in_mem_len += CompiledMapEncodePair.PeekSize(id, sizeof(uint)) + (uint)sizeof(CompiledMapEncodePair);
                        pool_offset += (uint)sizeof(CompiledMapEncodePair);
                        group_pair_count++;
                    }
                    if (packInfo.UseGlobalFileIndexMap)
                    {
                        RsbPackGroupResidentFileInfo[]? resident_file_list = groupList[i].ResidentFileList;
                        RsbPackGroupGPUFileInfo[]? image_list = groupList[i].GPUFileList;
                        if (resident_file_list != null)
                        {
                            for (int j = resident_file_list.Length - 1; j >= 0; j--)
                            {
                                string? path = resident_file_list[j].Path;
                                if (path != null)
                                {
                                    in_mem_len += CompiledMapEncodePair.PeekSize(path, sizeof(uint)) + (uint)sizeof(CompiledMapEncodePair);
                                    pool_offset += (uint)sizeof(CompiledMapEncodePair);
                                    resource_pair_count++;
                                }
                            }
                        }
                        if (image_list != null)
                        {
                            for (int j = image_list.Length - 1; j >= 0; j--)
                            {
                                image_count++;
                                if (image_list[j] == null || !image_list[j].InFileIndexDataMap)
                                {
                                    continue;
                                }
                                string? path = image_list[j].Path;
                                if (path != null)
                                {
                                    in_mem_len += CompiledMapEncodePair.PeekSize(path, sizeof(uint)) + (uint)sizeof(CompiledMapEncodePair);
                                    pool_offset += (uint)sizeof(CompiledMapEncodePair);
                                    resource_pair_count++;
                                }
                            }
                        }
                    }
                }
                if (in_mem_len > 0u)
                {
                    using (NativeMemoryOwner raw_data_allocator = new NativeMemoryOwner(in_mem_len))
                    {
                        nuint composite_pair_pointer_number = (nuint)raw_data_allocator.Pointer;
                        nuint group_pair_pointer_number = composite_pair_pointer_number + composite_pair_count * (uint)sizeof(CompiledMapEncodePair);
                        nuint resource_pair_pointer_number = group_pair_pointer_number + group_pair_count * (uint)sizeof(CompiledMapEncodePair);
                        nuint pool_pointer_number = composite_pair_pointer_number + pool_offset;
                        CompiledMapEncodePair* current_composite_pair_pointer = (CompiledMapEncodePair*)composite_pair_pointer_number;
                        CompiledMapEncodePair* current_group_pair_pointer = (CompiledMapEncodePair*)group_pair_pointer_number;
                        CompiledMapEncodePair* current_resource_pair_pointer = (CompiledMapEncodePair*)resource_pair_pointer_number;
                        nuint current_pool_pointer = pool_pointer_number;
                        for (int i = 0; i < packInfo.Composites.Length; i++)
                        {
                            string? key = packInfo.Composites[i].Id;
                            if (key != null)
                            {
                                nuint string_pointer_number = current_pool_pointer;
                                current_composite_pair_pointer->KeyOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                current_composite_pair_pointer->KeySize = (uint)key.Length + 1u;
                                encoding.GetBytes(key, new Span<byte>((void*)current_pool_pointer, key.Length));
                                current_pool_pointer += (uint)key.Length;
                                *(byte*)current_pool_pointer = 0x0;
                                current_pool_pointer++;
                                current_composite_pair_pointer->ValueOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                current_composite_pair_pointer->ValueSize = sizeof(uint);
                                *(uint*)current_pool_pointer = (uint)i;
                                current_pool_pointer += sizeof(uint);
                                current_composite_pair_pointer++;
                                UnsafeStringHelper.StringToUpper(string_pointer_number);
                            }
                        }
                        for (int i = 0; i < groupList.Count; i++)
                        {
                            string? key = groupList[i].Id;
                            if (key != null)
                            {
                                nuint string_pointer_number = current_pool_pointer;
                                current_group_pair_pointer->KeyOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                current_group_pair_pointer->KeySize = (uint)key.Length + 1u;
                                encoding.GetBytes(key, new Span<byte>((void*)current_pool_pointer, key.Length));
                                current_pool_pointer += (uint)key.Length;
                                *(byte*)current_pool_pointer = 0x0;
                                current_pool_pointer++;
                                current_group_pair_pointer->ValueOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                current_group_pair_pointer->ValueSize = sizeof(uint);
                                *(uint*)current_pool_pointer = (uint)i;
                                current_pool_pointer += sizeof(uint);
                                current_group_pair_pointer++;
                                UnsafeStringHelper.StringToUpper(string_pointer_number);
                            }
                            if (packInfo.UseGlobalFileIndexMap)
                            {
                                RsbPackGroupResidentFileInfo[]? resident_file_list = groupList[i].ResidentFileList;
                                RsbPackGroupGPUFileInfo[]? image_list = groupList[i].GPUFileList;
                                if (resident_file_list != null)
                                {
                                    for (int j = 0; j < resident_file_list.Length; j++)
                                    {
                                        string? path = resident_file_list[j].Path;
                                        if (path != null)
                                        {
                                            nuint string_pointer_number = current_pool_pointer;
                                            current_resource_pair_pointer->KeyOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                            current_resource_pair_pointer->KeySize = (uint)path.Length + 1u;
                                            encoding.GetBytes(path, new Span<byte>((void*)current_pool_pointer, path.Length));
                                            current_pool_pointer += (uint)path.Length;
                                            *(byte*)current_pool_pointer = 0x0;
                                            current_pool_pointer++;
                                            current_resource_pair_pointer->ValueOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                            current_resource_pair_pointer->ValueSize = sizeof(uint);
                                            *(uint*)current_pool_pointer = (uint)i; // not j
                                            current_pool_pointer += sizeof(uint);
                                            current_resource_pair_pointer++;
                                            UnsafeStringHelper.StringToUpper(string_pointer_number);
                                        }
                                    }
                                }
                                if (image_list != null)
                                {
                                    for (int j = 0; j < image_list.Length; j++)
                                    {
                                        if (image_list[j] == null || !image_list[j].InFileIndexDataMap)
                                        {
                                            continue;
                                        }
                                        string? path = image_list[j].Path;
                                        if (path != null)
                                        {
                                            nuint string_pointer_number = current_pool_pointer;
                                            current_resource_pair_pointer->KeyOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                            current_resource_pair_pointer->KeySize = (uint)path.Length + 1u;
                                            encoding.GetBytes(path, new Span<byte>((void*)current_pool_pointer, path.Length));
                                            current_pool_pointer += (uint)path.Length;
                                            *(byte*)current_pool_pointer = 0x0;
                                            current_pool_pointer++;
                                            current_resource_pair_pointer->ValueOffset = (uint)(current_pool_pointer - pool_pointer_number);
                                            current_resource_pair_pointer->ValueSize = sizeof(uint);
                                            *(uint*)current_pool_pointer = (uint)i; // not j
                                            current_pool_pointer += sizeof(uint);
                                            current_resource_pair_pointer++;
                                            UnsafeStringHelper.StringToUpper(string_pointer_number);
                                        }
                                    }
                                }
                            }
                        }
                        // sort
                        CompiledMapEncoder.Sort((CompiledMapEncodePair*)composite_pair_pointer_number, composite_pair_count, pool_pointer_number);
                        if (packInfo.UseGlobalFileIndexMap)
                        {
                            CompiledMapEncoder.Sort((CompiledMapEncodePair*)resource_pair_pointer_number, resource_pair_count, pool_pointer_number);
                        }
                        CompiledMapEncoder.Sort((CompiledMapEncodePair*)group_pair_pointer_number, group_pair_count, pool_pointer_number);
                        // compute repeat len
                        CompiledMapEncoder.ComputeRepeatLength((CompiledMapEncodePair*)composite_pair_pointer_number, composite_pair_count, pool_pointer_number);
                        if (packInfo.UseGlobalFileIndexMap)
                        {
                            CompiledMapEncoder.ComputeRepeatLength((CompiledMapEncodePair*)resource_pair_pointer_number, resource_pair_count, pool_pointer_number);
                        }
                        CompiledMapEncoder.ComputeRepeatLength((CompiledMapEncodePair*)group_pair_pointer_number, group_pair_count, pool_pointer_number);
                        // Peek header len
                        uint info_len = packInfo.MajorVersion >= 4 ? 0x70u : 0x6Cu;
                        uint resource_map_offset = info_len;
                        uint resource_map_len = packInfo.UseGlobalFileIndexMap ? CompiledMapEncoder.PeekSize((CompiledMapEncodePair*)resource_pair_pointer_number, resource_pair_count) : 0u;
                        uint group_map_offset = resource_map_offset + resource_map_len;
                        uint group_map_len = CompiledMapEncoder.PeekSize((CompiledMapEncodePair*)group_pair_pointer_number, group_pair_count);
                        uint composite_info_offset = group_map_offset + group_map_len;
                        uint composite_info_count = (uint)packInfo.Composites.Length;
                        uint composite_info_each_len = packInfo.MajorVersion >= 3 ? (uint)sizeof(ResStreamCompositeDescriptorV3V4) : (uint)sizeof(ResStreamCompositeDescriptorV1);
                        uint composite_map_offset = composite_info_offset + composite_info_count * composite_info_each_len;
                        uint composite_map_len = CompiledMapEncoder.PeekSize((CompiledMapEncodePair*)composite_pair_pointer_number, composite_pair_count);
                        uint group_info_offset = composite_map_offset + composite_map_len;
                        uint group_info_count = (uint)groupList.Count;
                        uint group_info_each_len = (uint)sizeof(ResStreamGroupDescriptor);
                        if (packInfo.MajorVersion <= 1)
                        {
                            group_info_each_len -= 8;
                        }
                        uint pool_info_offset = group_info_offset + group_info_count * group_info_each_len;
                        uint pool_info_count = (uint)poolList.Count;
                        uint pool_info_each_len = (uint)sizeof(ResStreamPoolDescriptor);
                        uint image_info_offset = pool_info_offset + pool_info_count * pool_info_each_len;
                        uint image_info_count = image_count;
                        uint image_info_each_len = packInfo.TextureDescriptorPitch;
                        if (!packInfo.UseGlobalFileIndexMap)
                        {
                            resource_map_offset = 0u;
                            resource_map_len = 0xFFFFFFFFu;
                        }
                        uint manifest_group_info_len;
                        uint manifest_resource_info_len;
                        uint manifest_pool_len;
                        uint manifest_group_info_offset;
                        uint manifest_resource_info_offset;
                        uint manifest_pool_offset;
                        uint header_len_no_manifest;
                        uint header_len;
                        RsbManifestInfo? manifest_info = null;
                        if (packInfo.UseManifest)
                        {
                            manifest_info = WindJsonSerializer.TryDeserializeFromFile<RsbManifestInfo>(paths.InfoManifestPath, fileSystem, new NullLogger(false));
                        }
                        Dictionary<string, uint>? manifest_pool_dic = PeekRsbManifestInfo(packInfo.MajorVersion, manifest_info, encoding, out manifest_group_info_len, out manifest_resource_info_len, out manifest_pool_len);
                        if (manifest_info != null && manifest_pool_dic != null)
                        {
                            header_len_no_manifest = image_info_offset + image_info_count * image_info_each_len;
                            if (packInfo.MajorVersion >= 4)
                            {
                                header_len_no_manifest = Align0x1000(header_len_no_manifest);
                            }
                            manifest_group_info_offset = header_len_no_manifest;
                            manifest_resource_info_offset = manifest_group_info_offset + manifest_group_info_len;
                            manifest_pool_offset = manifest_resource_info_offset + manifest_resource_info_len;
                            header_len = Align0x1000(manifest_pool_offset + manifest_pool_len);
                        }
                        else
                        {
                            header_len_no_manifest = Align0x1000(image_info_offset + image_info_count * image_info_each_len);
                            manifest_group_info_offset = 0u;
                            manifest_resource_info_offset = 0u;
                            manifest_pool_offset = 0u;
                            header_len = header_len_no_manifest;
                        }
                        using (IMemoryOwner<byte> rsb_header_allocator = MemoryPool<byte>.Shared.Rent((int)header_len))
                        {
                            Span<byte> rsb_header_allocator_span = rsb_header_allocator.Memory.Span[..(int)header_len];
                            rsb_header_allocator_span.Fill(0);
                            fixed (byte* rsb_header_allocator_pointer = rsb_header_allocator_span)
                            {
                                nuint rsb_header_pointer_number = (nuint)rsb_header_allocator_pointer;
                                // Encode info
                                ResStreamsHeader* rsb_info = (ResStreamsHeader*)rsb_header_pointer_number;
                                rsb_info->Magic = 1920164401u;
                                rsb_info->MajorVersion = packInfo.MajorVersion;
                                rsb_info->MinorVersion = packInfo.MinorVersion;
                                rsb_info->HeaderSize = header_len;
                                rsb_info->GlobalFileIndexMapSize = resource_map_len;
                                rsb_info->GlobalFileIndexMapOffset = resource_map_offset;
                                rsb_info->GroupIndexMapSize = group_map_len;
                                rsb_info->GroupIndexMapOffset = group_map_offset;
                                rsb_info->GroupCount = group_info_count;
                                rsb_info->GroupDescriptorOffset = group_info_offset;
                                rsb_info->GroupDescriptorSize = group_info_each_len;
                                rsb_info->CompositeCount = composite_info_count;
                                rsb_info->CompositeDescriptorOffset = composite_info_offset;
                                rsb_info->CompositeDescriptorSize = composite_info_each_len;
                                rsb_info->CompositeIndexMapSize = composite_map_len;
                                rsb_info->CompositeIndexMapOffset = composite_map_offset;
                                rsb_info->PoolCount = pool_info_count;
                                rsb_info->PoolDescriptorOffset = pool_info_offset;
                                rsb_info->PoolDescriptorSize = pool_info_each_len;
                                rsb_info->TextureCount = image_info_count;
                                rsb_info->TextureDescriptorOffset = image_info_offset;
                                rsb_info->TextureDescriptorSize = image_info_each_len;
                                rsb_info->ManifestGroupInfoOffset = manifest_group_info_offset;
                                rsb_info->ManifestResourceInfoOffset = manifest_resource_info_offset;
                                rsb_info->ManifestStringPoolOffset = manifest_pool_offset;
                                if (packInfo.MajorVersion >= 4)
                                {
                                    rsb_info->HeaderSizeNoManifest = header_len_no_manifest;
                                }
                                // Encode manifest
                                if (manifest_info != null && manifest_pool_dic != null)
                                {
                                    PackRsbManifest(packInfo.MajorVersion, manifest_info, manifest_pool_dic, encoding, rsb_header_pointer_number + manifest_group_info_offset, rsb_header_pointer_number + manifest_resource_info_offset, rsb_header_pointer_number + manifest_pool_offset);
                                }
                                // Encode cmap
                                if (packInfo.UseGlobalFileIndexMap)
                                {
                                    CompiledMapEncoder.Encode((CompiledMapEncodePair*)resource_pair_pointer_number, resource_pair_count, pool_pointer_number, rsb_header_pointer_number + resource_map_offset);
                                }
                                CompiledMapEncoder.Encode((CompiledMapEncodePair*)group_pair_pointer_number, group_pair_count, pool_pointer_number, rsb_header_pointer_number + group_map_offset);
                                CompiledMapEncoder.Encode((CompiledMapEncodePair*)composite_pair_pointer_number, composite_pair_count, pool_pointer_number, rsb_header_pointer_number + composite_map_offset);
                                CompiledMap group_map = new CompiledMap();
                                group_map.Init(rsb_header_pointer_number + group_map_offset, group_map_len);
                                // Encode composite info
                                nuint current_pointer_number = rsb_header_pointer_number + composite_info_offset;
                                if (packInfo.MajorVersion >= 3)
                                {
                                    for (uint i = 0u; i < composite_info_count; i++)
                                    {
                                        RsbPackCompositeInfo pack_composite_info = packInfo.Composites[i];
                                        ResStreamCompositeDescriptorV3V4* composite_info = (ResStreamCompositeDescriptorV3V4*)current_pointer_number;
                                        ResStreamCompositeDescriptorChildV3V4* composite_sub_info = (ResStreamCompositeDescriptorChildV3V4*)(current_pointer_number + 0x80u);
                                        UnsafeStringHelper.SetUtf16String(pack_composite_info.Name, composite_info->Name, 0x80, encoding);
                                        int subGroupCount = pack_composite_info.SubGroups?.Length ?? 0;
                                        composite_info->ChildCount = 0u;
                                        for (uint j = 0u; j < subGroupCount; j++)
                                        {
                                            RsbPackSubGroupInfo pack_composite_group_info = pack_composite_info.SubGroups![j];
                                            void* ptr = group_map.Find(pack_composite_group_info.Id);
                                            if (ptr == null)
                                            {
                                                logger.LogWarning($"Can not find group {pack_composite_group_info.Id}");
                                            }
                                            else
                                            {
                                                composite_info->ChildCount++;
                                                composite_sub_info->GroupIndex = *(uint*)ptr;
                                                composite_sub_info->ArtResolution = pack_composite_group_info.Res;
                                                composite_sub_info->Localization = UInt32StringConvertor.StringToUInt32(pack_composite_group_info.Loc);
                                                composite_sub_info++;
                                            }
                                        }
                                        current_pointer_number += composite_info_each_len;
                                    }
                                }
                                else
                                {
                                    for (uint i = 0u; i < composite_info_count; i++)
                                    {
                                        RsbPackCompositeInfo pack_composite_info = packInfo.Composites[i];
                                        ResStreamCompositeDescriptorV1* composite_info = (ResStreamCompositeDescriptorV1*)current_pointer_number;
                                        ResStreamCompositeDescriptorChildV1* composite_sub_info = (ResStreamCompositeDescriptorChildV1*)(current_pointer_number + 0x80u);
                                        UnsafeStringHelper.SetUtf16String(pack_composite_info.Name, composite_info->Name, 0x80, encoding);
                                        int subGroupCount = pack_composite_info.SubGroups?.Length ?? 0;
                                        composite_info->SubGroupCount = 0u;
                                        for (uint j = 0u; j < subGroupCount; j++)
                                        {
                                            RsbPackSubGroupInfo pack_composite_group_info = pack_composite_info.SubGroups![j];
                                            void* ptr = group_map.Find(pack_composite_group_info.Id);
                                            if (ptr == null)
                                            {
                                                logger.LogWarning($"Can not find group {pack_composite_group_info.Id}");
                                            }
                                            else
                                            {
                                                composite_info->SubGroupCount++;
                                                composite_sub_info->GroupIndex = *(uint*)ptr;
                                                composite_sub_info->ArtResolution = pack_composite_group_info.Res;
                                                composite_sub_info++;
                                            }
                                        }
                                        current_pointer_number += composite_info_each_len;
                                    }
                                }
                                // Encode pool info
                                current_pointer_number = rsb_header_pointer_number + pool_info_offset;
                                for (uint i = 0u; i < pool_info_count; i++)
                                {
                                    RsbPackPoolInfo pack_pool_info = poolList[(int)i];
                                    ResStreamPoolDescriptor* pool_info = (ResStreamPoolDescriptor*)current_pointer_number;
                                    UnsafeStringHelper.SetUtf16String(pack_pool_info.Name, pool_info->Name, 0x80, encoding);
                                    pool_info->NumInstances = pack_pool_info.NumInstances;
                                    pool_info->PoolFlags = pack_pool_info.PoolFlags;
                                    current_pointer_number += pool_info_each_len;
                                }
                                rsbStream.SetLength(header_len);
                                rsbStream.Seek(header_len, SeekOrigin.Begin);
                                // Encode group and image info
                                current_pointer_number = rsb_header_pointer_number + group_info_offset;
                                nuint current_image_info_pointer_number = rsb_header_pointer_number + image_info_offset;
                                uint image_current_offset = 0u;
                                for (uint i = 0u; i < group_info_count; i++)
                                {
                                    RsbPackGroupInfo pack_group_info = groupList[(int)i];
                                    ResStreamGroupDescriptor* group_info = (ResStreamGroupDescriptor*)current_pointer_number;
                                    UnsafeStringHelper.SetUtf16String(pack_group_info.Name, group_info->Name, 0x80, encoding);
                                    string rsg_path = paths.GetRsgPathByGroupId(pack_group_info.Id);
                                    string rsg_info_path = paths.AppendMetaExtension(rsg_path);
                                    RsgMetadata? rsg_pack_info = WindJsonSerializer.TryDeserializeFromFile<RsgMetadata>(rsg_info_path, fileSystem, new NullLogger(false));
                                    if (rsg_pack_info == null)
                                    {
                                        logger.LogError($"Can not read rsg info at {rsg_info_path}");
                                    }
                                    else
                                    {
                                        using (Stream rsg_stream = fileSystem.OpenRead(rsg_path))
                                        {
                                            uint group_image_count = (uint)(pack_group_info.GPUFileList?.Length ?? 0);
                                            group_info->RsgOffset = (uint)rsbStream.Position;
                                            group_info->RsgSize = (uint)rsg_stream.Length;
                                            group_info->PoolIndex = pack_group_info.PoolId;
                                            group_info->RsgCompressionFlags = rsg_pack_info.CompressionFlags;
                                            group_info->RsgHeaderSize = rsg_pack_info.HeaderSize;
                                            group_info->RsgResidentDataOffset = rsg_pack_info.ResidentDataOffset;
                                            group_info->RsgResidentDataCompressedSize = rsg_pack_info.ResidentDataCompressedSize;
                                            group_info->RsgResidentDataUncompressedSize = rsg_pack_info.ResidentDataUncompressedSize;
                                            group_info->RsgResidentDataPoolSize = rsg_pack_info.ResidentDataUncompressedSize;
                                            group_info->RsgGPUDataOffset = rsg_pack_info.GPUDataOffset;
                                            group_info->RsgGPUDataCompressedSize = rsg_pack_info.GPUDataCompressedSize;
                                            group_info->RsgGPUDataUncompressedSize = rsg_pack_info.GPUDataUncompressedSize;
                                            rsg_stream.CopyTo(rsbStream);
                                            ResStreamPoolDescriptor* pool_info = (ResStreamPoolDescriptor*)(rsb_header_pointer_number + pool_info_offset + pool_info_each_len * group_info->PoolIndex);
                                            pool_info->ResidentDataMemorySize = Math.Max(pool_info->ResidentDataMemorySize, rsg_pack_info.HeaderSize + rsg_pack_info.ResidentDataUncompressedSize);
                                            pool_info->GPUDataMemorySize = Math.Max(pool_info->GPUDataMemorySize, rsg_pack_info.GPUDataUncompressedSize);
                                            if (packInfo.MajorVersion <= 1)
                                            {
                                                pool_info->TextureCount = group_image_count;
                                                pool_info->TextureDescriptorStartIndex = image_current_offset;
                                            }
                                            else
                                            {
                                                group_info->TextureCount = group_image_count;
                                                group_info->TextureDescriptorStartIndex = image_current_offset;
                                            }
                                            if (pack_group_info.GPUFileList != null)
                                            {
                                                for (int j = 0; j < pack_group_info.GPUFileList.Length; j++)
                                                {
                                                    RsbPackGroupGPUFileInfo pack_image_info = pack_group_info.GPUFileList[j];
                                                    ResStreamTextureDescriptor* image_info = (ResStreamTextureDescriptor*)current_image_info_pointer_number;
                                                    image_info->Width = pack_image_info.Width;
                                                    image_info->Height = pack_image_info.Height;
                                                    image_info->Pitch = pack_image_info.Pitch;
                                                    image_info->Format = pack_image_info.Format;
                                                    if (image_info_each_len >= 0x14u)
                                                    {
                                                        image_info->Extend1 = pack_image_info.Extend1;
                                                    }
                                                    if (image_info_each_len >= 0x18u)
                                                    {
                                                        image_info->Extend2 = pack_image_info.Extend2;
                                                    }
                                                    current_image_info_pointer_number += image_info_each_len;
                                                }
                                            }
                                            image_current_offset += group_image_count;
                                        }
                                    }
                                    current_pointer_number += group_info_each_len;
                                }
                                rsbStream.Seek(0, SeekOrigin.Begin);
                                RsbReverser.ReverseRsbHeader(rsb_info, Endianness.Native, packInfo.UseBigEndian ? Endianness.Big : Endianness.Little);
                                rsbStream.Write((void*)rsb_header_pointer_number, header_len);
                            }
                        }
                    }
                }
            }
        }

        private static unsafe void PackRsbManifest(uint rsbVersion, RsbManifestInfo manifest, Dictionary<string, uint> pool_dic, Encoding encoding, nuint manifest_group_info_pointer_number, nuint manifest_resource_info_pointer_number, nuint manifest_pool_pointer_number)
        {
            // group and resource
            uint current_resource_offset = 0u;
            if (manifest.CompositeGroups != null)
            {
                uint manifestSubGroupInfoSize = rsbVersion >= 3u ? (uint)sizeof(ManifestGroupInfoV3V4) : (uint)sizeof(ManifestGroupInfoV1);
                for (int i = 0; i < manifest.CompositeGroups.Length; i++)
                {
                    RsbManifestCompositeInfo composite_info = manifest.CompositeGroups[i];
                    ManifestCompositeInfo* composite = (ManifestCompositeInfo*)manifest_group_info_pointer_number;
                    manifest_group_info_pointer_number += (uint)sizeof(ManifestCompositeInfo);
                    composite->IdOffset = pool_dic[composite_info.Id ?? string.Empty];
                    composite->ChildCount = (uint)(composite_info.SubGroups?.Length ?? 0);
                    composite->GroupInfoSize = manifestSubGroupInfoSize;
                    if (composite_info.SubGroups != null)
                    {
                        for (int j = 0; j < composite_info.SubGroups.Length; j++)
                        {
                            RsbManifestGroupInfo group_info = composite_info.SubGroups[j];
                            if (rsbVersion >= 3u)
                            {
                                ManifestGroupInfoV3V4* group = (ManifestGroupInfoV3V4*)manifest_group_info_pointer_number;
                                group->ArtResolution = group_info.Res;
                                group->Localization = UInt32StringConvertor.StringToUInt32(group_info.Loc);
                                group->IdOffset = pool_dic[group_info.Id ?? string.Empty];
                                group->ResourceCount = (uint)(group_info.Resources?.Length ?? 0);
                            }
                            else
                            {
                                ManifestGroupInfoV1* group = (ManifestGroupInfoV1*)manifest_group_info_pointer_number;
                                group->ArtResolution = group_info.Res;
                                group->IdOffset = pool_dic[group_info.Id ?? string.Empty];
                                group->ResourceCount = (uint)(group_info.Resources?.Length ?? 0);
                            }
                            manifest_group_info_pointer_number += manifestSubGroupInfoSize;
                            if (group_info.Resources != null)
                            {
                                for (int k = 0; k < group_info.Resources.Length; k++)
                                {
                                    RsbManifestResourceInfo resource_info = group_info.Resources[k];
                                    *(uint*)manifest_group_info_pointer_number = current_resource_offset;
                                    manifest_group_info_pointer_number += 4;
                                    ManifestResourceHeader* res_header = (ManifestResourceHeader*)(manifest_resource_info_pointer_number + current_resource_offset);
                                    current_resource_offset += (uint)sizeof(ManifestResourceHeader);
                                    res_header->Type = (ushort)resource_info.Type;
                                    res_header->HeaderSize = (ushort)sizeof(ManifestResourceHeader);
                                    res_header->IdOffset = pool_dic[resource_info.Id ?? string.Empty];
                                    res_header->PathOffset = pool_dic[resource_info.Path ?? string.Empty];
                                    res_header->UniversalPropertyCount = (uint)(resource_info.UniversalProperties?.Count ?? 0);
                                    if (resource_info.ImageProperties == null)
                                    {
                                        res_header->ImagePropertyOffset = 0u;
                                    }
                                    else
                                    {
                                        res_header->ImagePropertyOffset = current_resource_offset;
                                        ManifestResourceImageProperty* res_img_prop = (ManifestResourceImageProperty*)(manifest_resource_info_pointer_number + current_resource_offset);
                                        current_resource_offset += (uint)sizeof(ManifestResourceImageProperty);
                                        res_img_prop->Atlas = (ushort)resource_info.ImageProperties.Atlas;
                                        res_img_prop->AtlasFlags = (ushort)resource_info.ImageProperties.AFlags;
                                        res_img_prop->OffsetX = (ushort)resource_info.ImageProperties.X;
                                        res_img_prop->OffsetY = (ushort)resource_info.ImageProperties.Y;
                                        res_img_prop->AtlasX = (ushort)resource_info.ImageProperties.AX;
                                        res_img_prop->AtlasY = (ushort)resource_info.ImageProperties.AY;
                                        res_img_prop->AtlasWidth = (ushort)resource_info.ImageProperties.AW;
                                        res_img_prop->AtlasHeight = (ushort)resource_info.ImageProperties.AH;
                                        res_img_prop->Rows = (ushort)resource_info.ImageProperties.Rows;
                                        res_img_prop->Cols = (ushort)resource_info.ImageProperties.Cols;
                                        res_img_prop->ParentOffset = pool_dic[resource_info.ImageProperties.Parent ?? string.Empty];
                                    }
                                    res_header->UniversalPropertyOffset = current_resource_offset;
                                    if (resource_info.UniversalProperties != null)
                                    {
                                        foreach (KeyValuePair<string, string> property in resource_info.UniversalProperties)
                                        {
                                            ManifestResourceUniversalProperty* res_prop = (ManifestResourceUniversalProperty*)(manifest_resource_info_pointer_number + current_resource_offset);
                                            current_resource_offset += (uint)sizeof(ManifestResourceUniversalProperty);
                                            res_prop->KeyOffset = pool_dic[property.Key ?? string.Empty];
                                            res_prop->ValueOffset = pool_dic[property.Value ?? string.Empty];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // pool
            foreach (KeyValuePair<string, uint> pool_pair in pool_dic)
            {
                uint count = (uint)UnsafeStringHelper.SetUtf16String(pool_pair.Key, manifest_pool_pointer_number + pool_pair.Value, encoding);
                *(byte*)(manifest_pool_pointer_number + pool_pair.Value + count) = 0;
            }
        }

        private static unsafe Dictionary<string, uint>? PeekRsbManifestInfo(uint rsbVersion, RsbManifestInfo? manifest, Encoding encoding, out uint manifest_group_info_len, out uint manifest_resource_info_len, out uint manifest_pool_len)
        {
            manifest_group_info_len = 0u;
            manifest_resource_info_len = 0u;
            manifest_pool_len = 0u;
            if (manifest == null || manifest.CompositeGroups == null)
            {
                return null;
            }
            Dictionary<string, uint> string_pool = new Dictionary<string, uint>();
            uint manifest_pool_len_native = 0u;
            void ThrowInPool(string? str)
            {
                str ??= string.Empty;
                if (!string_pool.ContainsKey(str))
                {
                    string_pool.Add(str, manifest_pool_len_native);
                    manifest_pool_len_native += (uint)encoding.GetByteCount(str) + 1u;
                }
            }
            ThrowInPool(string.Empty);
            uint manifestSubGroupInfoSize = rsbVersion >= 3u ? (uint)sizeof(ManifestGroupInfoV3V4) : (uint)sizeof(ManifestGroupInfoV1);
            for (int i = 0; i < manifest.CompositeGroups.Length; i++)
            {
                RsbManifestCompositeInfo composite_info = manifest.CompositeGroups[i];
                manifest_group_info_len += (uint)sizeof(ManifestCompositeInfo);
                ThrowInPool(composite_info.Id);
                if (composite_info.SubGroups != null)
                {
                    for (int j = 0; j < composite_info.SubGroups.Length; j++)
                    {
                        RsbManifestGroupInfo group_info = composite_info.SubGroups[j];
                        manifest_group_info_len += manifestSubGroupInfoSize;
                        ThrowInPool(group_info.Id);
                        if (group_info.Resources != null)
                        {
                            manifest_group_info_len += 4 * (uint)group_info.Resources.Length;
                            for (int k = 0; k < group_info.Resources.Length; k++)
                            {
                                RsbManifestResourceInfo resource_info = group_info.Resources[k];
                                manifest_resource_info_len += (uint)sizeof(ManifestResourceHeader);
                                ThrowInPool(resource_info.Id);
                                ThrowInPool(resource_info.Path);
                                if (resource_info.ImageProperties != null)
                                {
                                    manifest_resource_info_len += (uint)sizeof(ManifestResourceImageProperty);
                                    ThrowInPool(resource_info.ImageProperties.Parent);
                                }
                                if (resource_info.UniversalProperties != null)
                                {
                                    manifest_resource_info_len += (uint)sizeof(ManifestResourceUniversalProperty) * (uint)resource_info.UniversalProperties.Count;
                                    foreach (KeyValuePair<string, string> property in resource_info.UniversalProperties)
                                    {
                                        ThrowInPool(property.Key);
                                        ThrowInPool(property.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            manifest_pool_len = manifest_pool_len_native;
            return string_pool;
        }

        private static uint Align0x1000(uint value)
        {
            if ((value & 0xFFF) != 0)
            {
                value |= 0xFFF;
                value++;
            }
            return value;
        }
    }
}
