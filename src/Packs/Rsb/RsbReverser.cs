using LibWindPop.Packs.Rsb.RsbStructures;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Packs.Rsb
{
    /// <summary>
    /// 用于交换Rsb端序的类
    /// </summary>
    public static unsafe class RsbReverser
    {
        private static Endianness GetOutEndianness(Endianness in_endian, Endianness out_endian)
        {
            return out_endian.IsNoneEndian() ? (in_endian.IsLittleEndian() ? Endianness.Big : Endianness.Little) : out_endian;
        }

        public static Endianness CheckRsbEndian(uint rsb_magic, Endianness in_endian)
        {
            if (!in_endian.IsNoneEndian())
            {
                return in_endian;
            }
            if (BitConverter.IsLittleEndian)
            {
                return rsb_magic == 1920164401u ? Endianness.Little : Endianness.Big;
            }
            return rsb_magic == 828535666u ? Endianness.Little : Endianness.Big;
        }

        public static Endianness CheckRsgEndian(uint rsg_magic, Endianness in_endian)
        {
            if (!in_endian.IsNoneEndian())
            {
                return in_endian;
            }
            if (BitConverter.IsLittleEndian)
            {
                return rsg_magic == 1920165744u ? Endianness.Little : Endianness.Big;
            }
            return rsg_magic == 1885827954u ? Endianness.Little : Endianness.Big;
        }

        /// <summary>
        /// 交换Rsb头部端序
        /// </summary>
        /// <param name="rsb_header_pointer">rsb头部指针</param>
        /// <param name="in_endian">输入端序，若不为Little/Big/Native/NoneNative则通过文件幻数自行判断，函数返回时为Little/Big</param>
        /// <param name="out_endian">输出端序，若不为Little/Big/Native/NoneNative则与输入端序相反，函数返回时为Little/Big</param>
        public static void ReverseRsbHeader(void* rsb_header_pointer, Endianness in_endian, Endianness out_endian)
        {
            ArgumentNullException.ThrowIfNull(rsb_header_pointer, nameof(rsb_header_pointer));
            RsbInfo* rsb_info = (RsbInfo*)rsb_header_pointer;
            in_endian = CheckRsbEndian(rsb_info->Magic, in_endian);
            out_endian = GetOutEndianness(in_endian, out_endian);
            if (!in_endian.IsEqual(out_endian))
            {
                if (out_endian.IsNativeEndian())
                {
                    ReverseRsbHeaderEndianAsNativeEndian(rsb_header_pointer);
                }
                else
                {
                    ReverseRsbHeaderEndianAsUnusedEndian(rsb_header_pointer);
                }
            }
        }

        /// <summary>
        /// 交换Rsg头部端序
        /// </summary>
        /// <param name="rsg_header_pointer">rsg头部指针</param>
        /// <param name="in_endian">输入端序，若不为Little/Big/Native/NoneNative则通过文件幻数自行判断，函数返回时为Little/Big</param>
        /// <param name="out_endian">输出端序，若不为Little/Big/Native/NoneNative则与输入端序相反，函数返回时为Little/Big</param>
        public static void ReverseRsgHeader(void* rsg_header_pointer, Endianness in_endian, Endianness out_endian)
        {
            ArgumentNullException.ThrowIfNull(rsg_header_pointer, nameof(rsg_header_pointer));
            RsgInfo* rsg_info = (RsgInfo*)rsg_header_pointer;
            in_endian = CheckRsgEndian(rsg_info->Magic, in_endian);
            out_endian = GetOutEndianness(in_endian, out_endian);
            if (!in_endian.IsEqual(out_endian))
            {
                if (out_endian.IsNativeEndian())
                {
                    ReverseRsgpHeaderEndianAsNativeEndian(rsg_header_pointer);
                }
                else
                {
                    ReverseRsgpHeaderEndianAsUnusedEndian(rsg_header_pointer);
                }
            }
        }

        private static void ReverseRsgpHeaderEndianAsUnusedEndian(void* header_pointer)
        {
            ArgumentNullException.ThrowIfNull(header_pointer, nameof(header_pointer));
            RsgInfo* rsgp_info = (RsgInfo*)header_pointer;
            ReverseInfoEndian((nuint)header_pointer + rsgp_info->FileIndexDataMapOffset, 1u, rsgp_info->FileIndexDataMapSize, 0u);
            ReverseInfoEndian((nuint)header_pointer, 1u, 0x5Cu, 0u);
        }

        private static void ReverseRsgpHeaderEndianAsNativeEndian(void* header_pointer)
        {
            ArgumentNullException.ThrowIfNull(header_pointer, nameof(header_pointer));
            RsgInfo* rsgp_info = (RsgInfo*)header_pointer;
            ReverseInfoEndian((nuint)header_pointer, 1u, 0x5Cu, 0u);
            ReverseInfoEndian((nuint)header_pointer + rsgp_info->FileIndexDataMapOffset, 1u, rsgp_info->FileIndexDataMapSize, 0u);
        }

        private static void ReverseRsbHeaderEndianAsUnusedEndian(void* header_pointer)
        {
            ArgumentNullException.ThrowIfNull(header_pointer, nameof(header_pointer));
            RsbInfo* rsb_info = (RsbInfo*)header_pointer;
            nuint header_pointer_number = (nuint)header_pointer;
            // file_compiled_map
            ReverseInfoEndian(header_pointer_number + rsb_info->GlobalFileIndexMapOffset, 1u, rsb_info->GlobalFileIndexMapSize, 0u);
            // sub_group_compiled_map
            ReverseInfoEndian(header_pointer_number + rsb_info->GroupIndexMapOffset, 1u, rsb_info->GroupIndexMapSize, 0u);
            // composite_res_group_info
            if (rsb_info->CompositeDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->CompositeDescriptorOffset, rsb_info->CompositeCount, rsb_info->CompositeDescriptorPitch, 0x80u);
            }
            // composite_res_group_compiled_map
            ReverseInfoEndian(header_pointer_number + rsb_info->CompositeIndexMapOffset, 1u, rsb_info->CompositeIndexMapSize, 0u);
            // sub_group_info
            if (rsb_info->GroupDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->GroupDescriptorOffset, rsb_info->GroupCount, rsb_info->GroupDescriptorPitch, 0x80u);
            }
            // res_streams_pool_info
            if (rsb_info->PoolDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->PoolDescriptorOffset, rsb_info->PoolCount, rsb_info->PoolDescriptorPitch, 0x80u);
            }
            // image_info
            if (rsb_info->TextureDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->TextureDescriptorOffset, rsb_info->TextureCount, rsb_info->TextureDescriptorPitch, 0u);
            }
            // manifast
            if (rsb_info->ManifestGroupInfoOffset != 0)
            {
                ReverseManifestEndianAsUnusedEndian(rsb_info->Version, header_pointer_number, header_pointer_number + rsb_info->ManifestGroupInfoOffset, header_pointer_number + rsb_info->ManifestResourceInfoOffset, header_pointer_number + rsb_info->ManifestStringPoolOffset);
            }
            // rsb_info
            ReverseInfoEndian(header_pointer_number, 1u, rsb_info->Version > 3 ? 0x70u : 0x6Cu, 0u);
        }

        private static void ReverseRsbHeaderEndianAsNativeEndian(void* header_pointer)
        {
            RsbInfo* rsb_info = (RsbInfo*)header_pointer;
            nuint header_pointer_number = (nuint)header_pointer;
            uint version = BinaryPrimitives.ReverseEndianness(rsb_info->Version);
            // rsb_info
            ReverseInfoEndian(header_pointer_number, 1u, version > 3u ? 0x70u : 0x6Cu, 0u);
            // file_compiled_map
            ReverseInfoEndian(header_pointer_number + rsb_info->GlobalFileIndexMapOffset, 1u, rsb_info->GlobalFileIndexMapSize, 0u);
            // sub_group_compiled_map
            ReverseInfoEndian(header_pointer_number + rsb_info->GroupIndexMapOffset, 1u, rsb_info->GroupIndexMapSize, 0u);
            // composite_res_group_info
            if (rsb_info->CompositeDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->CompositeDescriptorOffset, rsb_info->CompositeCount, rsb_info->CompositeDescriptorPitch, 0x80u);
            }
            // composite_res_group_compiled_map
            ReverseInfoEndian(header_pointer_number + rsb_info->CompositeIndexMapOffset, 1u, rsb_info->CompositeIndexMapSize, 0u);
            // sub_group_info
            if (rsb_info->GroupDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->GroupDescriptorOffset, rsb_info->GroupCount, rsb_info->GroupDescriptorPitch, 0x80u);
            }
            // res_streams_pool_info
            if (rsb_info->PoolDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->PoolDescriptorOffset, rsb_info->PoolCount, rsb_info->PoolDescriptorPitch, 0x80u);
            }
            // texture_info
            if (rsb_info->TextureDescriptorOffset != 0)
            {
                ReverseInfoEndian(header_pointer_number + rsb_info->TextureDescriptorOffset, rsb_info->TextureCount, rsb_info->TextureDescriptorPitch, 0u);
            }
            // manifast
            if (rsb_info->ManifestGroupInfoOffset != 0)
            {
                ReverseManifestEndianAsNativeEndian(version, header_pointer_number, header_pointer_number + rsb_info->ManifestGroupInfoOffset, header_pointer_number + rsb_info->ManifestResourceInfoOffset, header_pointer_number + rsb_info->ManifestStringPoolOffset);
            }
        }

        private static void ReverseManifestStructEndian(ManifestResourceImageProperty* ptr)
        {
            ptr->Atlas = BinaryPrimitives.ReverseEndianness(ptr->Atlas);
            ptr->AtlasFlags = BinaryPrimitives.ReverseEndianness(ptr->AtlasFlags);
            ptr->OffsetX = BinaryPrimitives.ReverseEndianness(ptr->OffsetX);
            ptr->OffsetY = BinaryPrimitives.ReverseEndianness(ptr->OffsetY);
            ptr->AtlasX = BinaryPrimitives.ReverseEndianness(ptr->AtlasX);
            ptr->AtlasY = BinaryPrimitives.ReverseEndianness(ptr->AtlasY);
            ptr->AtlasWidth = BinaryPrimitives.ReverseEndianness(ptr->AtlasWidth);
            ptr->AtlasHeight = BinaryPrimitives.ReverseEndianness(ptr->AtlasHeight);
            ptr->Rows = BinaryPrimitives.ReverseEndianness(ptr->Rows);
            ptr->Cols = BinaryPrimitives.ReverseEndianness(ptr->Cols);
            ptr->ParentOffset = BinaryPrimitives.ReverseEndianness(ptr->ParentOffset);
        }

        private static void ReverseManifestStructEndian(ManifestResourceUniversalProperty* ptr)
        {
            ptr->KeyOffset = BinaryPrimitives.ReverseEndianness(ptr->KeyOffset);
            ptr->Unknow0x4 = BinaryPrimitives.ReverseEndianness(ptr->Unknow0x4);
            ptr->ValueOffset = BinaryPrimitives.ReverseEndianness(ptr->ValueOffset);
        }

        private static void ReverseManifestStructEndian(ManifestResourceHeader* ptr)
        {
            ptr->Unknow0x0 = BinaryPrimitives.ReverseEndianness(ptr->Unknow0x0);
            ptr->Type = BinaryPrimitives.ReverseEndianness(ptr->Type);
            ptr->HeaderSize = BinaryPrimitives.ReverseEndianness(ptr->HeaderSize);
            ptr->UniversalPropertyOffset = BinaryPrimitives.ReverseEndianness(ptr->UniversalPropertyOffset);
            ptr->ImagePropertyOffset = BinaryPrimitives.ReverseEndianness(ptr->ImagePropertyOffset);
            ptr->IdOffset = BinaryPrimitives.ReverseEndianness(ptr->IdOffset);
            ptr->PathOffset = BinaryPrimitives.ReverseEndianness(ptr->PathOffset);
            ptr->UniversalPropertyCount = BinaryPrimitives.ReverseEndianness(ptr->UniversalPropertyCount);
        }

        private static void ReverseManifestStructEndian(ManifestSubGroupInfoForRsbV1* ptr)
        {
            ptr->Res = BinaryPrimitives.ReverseEndianness(ptr->Res);
            ptr->IdOffset = BinaryPrimitives.ReverseEndianness(ptr->IdOffset);
            ptr->ResourceCount = BinaryPrimitives.ReverseEndianness(ptr->ResourceCount);
        }

        private static void ReverseManifestStructEndian(ManifestSubGroupInfoForRsbV3V4* ptr)
        {
            ptr->Res = BinaryPrimitives.ReverseEndianness(ptr->Res);
            ptr->Loc = BinaryPrimitives.ReverseEndianness(ptr->Loc);
            ptr->IdOffset = BinaryPrimitives.ReverseEndianness(ptr->IdOffset);
            ptr->ResourceCount = BinaryPrimitives.ReverseEndianness(ptr->ResourceCount);
        }

        private static void ReverseManifestStructEndian(ManifestCompositeGroupInfo* ptr)
        {
            ptr->IdOffset = BinaryPrimitives.ReverseEndianness(ptr->IdOffset);
            ptr->SubGroupCount = BinaryPrimitives.ReverseEndianness(ptr->SubGroupCount);
            ptr->SubGroupInfoEachSize = BinaryPrimitives.ReverseEndianness(ptr->SubGroupInfoEachSize);
        }

        private static void ReverseManifestEndianAsUnusedEndian(uint version, nuint header_pointer_number, nuint group_pointer_number, nuint file_pointer_number, nuint pool_pointer_number)
        {
            nuint current_group_pointer_number = group_pointer_number;
            ManifestCompositeGroupInfo* composite_pointer;
            ManifestSubGroupInfoForRsbV1* group_pointer_v1 = null;
            ManifestSubGroupInfoForRsbV3V4* group_pointer_v3v4 = null;
            uint* file_pointer;
            ManifestResourceHeader* res_header_pointer;
            ManifestResourceImageProperty* image_prop_pointer;
            ManifestResourceUniversalProperty* universal_prop_pointer;
            while (current_group_pointer_number < file_pointer_number)
            {
                composite_pointer = (ManifestCompositeGroupInfo*)current_group_pointer_number;
                current_group_pointer_number += (uint)sizeof(ManifestCompositeGroupInfo);
                for (uint i = 0u; i < composite_pointer->SubGroupCount; i++)
                {
                    uint resCount;
                    if (version >= 3u)
                    {
                        group_pointer_v3v4 = (ManifestSubGroupInfoForRsbV3V4*)current_group_pointer_number;
                        resCount = group_pointer_v3v4->ResourceCount;
                    }
                    else
                    {
                        group_pointer_v1 = (ManifestSubGroupInfoForRsbV1*)current_group_pointer_number;
                        resCount = group_pointer_v1->ResourceCount;
                    }
                    current_group_pointer_number += composite_pointer->SubGroupInfoEachSize;
                    for (uint j = 0u; j < resCount; j++)
                    {
                        file_pointer = (uint*)current_group_pointer_number;
                        current_group_pointer_number += 4;
                        res_header_pointer = (ManifestResourceHeader*)(file_pointer_number + *file_pointer);
                        *file_pointer = BinaryPrimitives.ReverseEndianness(*file_pointer);
                        if (res_header_pointer->ImagePropertyOffset != 0u)
                        {
                            image_prop_pointer = (ManifestResourceImageProperty*)(file_pointer_number + res_header_pointer->ImagePropertyOffset);
                            ReverseManifestStructEndian(image_prop_pointer);
                        }
                        if (res_header_pointer->UniversalPropertyCount != 0u)
                        {
                            universal_prop_pointer = (ManifestResourceUniversalProperty*)(file_pointer_number + res_header_pointer->UniversalPropertyOffset);
                            for (uint k = 0u; k < res_header_pointer->UniversalPropertyCount; k++)
                            {
                                ReverseManifestStructEndian(universal_prop_pointer);
                                universal_prop_pointer++;
                            }
                        }
                        ReverseManifestStructEndian(res_header_pointer);
                    }
                    if (version >= 3u)
                    {
                        ReverseManifestStructEndian(group_pointer_v3v4);
                    }
                    else
                    {
                        ReverseManifestStructEndian(group_pointer_v1);
                    }
                }
                ReverseManifestStructEndian(composite_pointer);
            }
        }

        private static void ReverseManifestEndianAsNativeEndian(uint version, nuint header_pointer_number, nuint group_pointer_number, nuint file_pointer_number, nuint pool_pointer_number)
        {
            nuint current_group_pointer_number = group_pointer_number;
            ManifestCompositeGroupInfo* composite_pointer;
            ManifestSubGroupInfoForRsbV1* group_pointer_v1;
            ManifestSubGroupInfoForRsbV3V4* group_pointer_v3v4;
            uint* file_pointer;
            ManifestResourceHeader* res_header_pointer;
            ManifestResourceImageProperty* image_prop_pointer;
            ManifestResourceUniversalProperty* universal_prop_pointer;
            while (current_group_pointer_number < file_pointer_number)
            {
                composite_pointer = (ManifestCompositeGroupInfo*)current_group_pointer_number;
                current_group_pointer_number += (uint)sizeof(ManifestCompositeGroupInfo);
                ReverseManifestStructEndian(composite_pointer);
                for (uint i = 0u; i < composite_pointer->SubGroupCount; i++)
                {
                    uint resCount;
                    if (version >= 3u)
                    {
                        group_pointer_v3v4 = (ManifestSubGroupInfoForRsbV3V4*)current_group_pointer_number;
                        ReverseManifestStructEndian(group_pointer_v3v4);
                        resCount = group_pointer_v3v4->ResourceCount;
                    }
                    else
                    {
                        group_pointer_v1 = (ManifestSubGroupInfoForRsbV1*)current_group_pointer_number;
                        ReverseManifestStructEndian(group_pointer_v1);
                        resCount = group_pointer_v1->ResourceCount;
                    }
                    current_group_pointer_number += composite_pointer->SubGroupInfoEachSize;
                    for (uint j = 0u; j < resCount; j++)
                    {
                        file_pointer = (uint*)current_group_pointer_number;
                        current_group_pointer_number += 4;
                        *file_pointer = BinaryPrimitives.ReverseEndianness(*file_pointer);
                        res_header_pointer = (ManifestResourceHeader*)(file_pointer_number + *file_pointer);
                        ReverseManifestStructEndian(res_header_pointer);
                        if (res_header_pointer->ImagePropertyOffset != 0u)
                        {
                            image_prop_pointer = (ManifestResourceImageProperty*)(file_pointer_number + res_header_pointer->ImagePropertyOffset);
                            ReverseManifestStructEndian(image_prop_pointer);
                        }
                        if (res_header_pointer->UniversalPropertyCount != 0u)
                        {
                            universal_prop_pointer = (ManifestResourceUniversalProperty*)(file_pointer_number + res_header_pointer->UniversalPropertyOffset);
                            for (uint k = 0u; k < res_header_pointer->UniversalPropertyCount; k++)
                            {
                                ReverseManifestStructEndian(universal_prop_pointer);
                                universal_prop_pointer++;
                            }
                        }
                    }
                }
            }
        }

        private static void ReverseInfoEndian(nuint info_pointer, uint info_count, uint info_each_length, uint info_string_length)
        {
            nuint begin_pointer = info_pointer + info_string_length;
            for (uint i = 0u; i < info_count; i++)
            {
                uint* reserve_pointer = (uint*)begin_pointer;
                for (uint j = info_string_length; j < info_each_length; j += 4)
                {
                    *reserve_pointer = BinaryPrimitives.ReverseEndianness(*reserve_pointer);
                    reserve_pointer++;
                }
                begin_pointer += info_each_length;
            }
        }
    }
}
