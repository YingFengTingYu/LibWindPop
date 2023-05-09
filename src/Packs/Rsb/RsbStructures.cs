using System.Runtime.InteropServices;

namespace LibWindPop.Packs.Rsb.RsbStructures
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x484)]
    public unsafe struct RsbCompositeDescriptorV3V4
    {
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        [FieldOffset(0x480)]
        public uint SubGroupCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
    public unsafe struct RsbSubGroupV3V4
    {
        [FieldOffset(0x0)]
        public uint GroupIndex;

        [FieldOffset(0x4)]
        public uint Res;

        [FieldOffset(0x8)]
        public uint Loc;

        [FieldOffset(0xC)]
        public uint Unknow0xC;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x284)]
    public unsafe struct RsbCompositeDescriptorV1
    {
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        [FieldOffset(0x280)]
        public uint SubGroupCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]
    public unsafe struct RsbSubGroupV1
    {
        [FieldOffset(0x0)]
        public uint GroupIndex;

        [FieldOffset(0x4)]
        public uint Res;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x18)]
    public struct RsbTextureDescriptor
    {
        [FieldOffset(0x0)]
        public uint Width;

        [FieldOffset(0x4)]
        public uint Height;

        [FieldOffset(0x8)]
        public uint Pitch;

        [FieldOffset(0xC)]
        public uint Format;

        [FieldOffset(0x10)]
        public uint Extend0x10;

        [FieldOffset(0x14)]
        public uint Extend0x14;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x98)]
    public unsafe struct RsbPoolDescriptor
    {
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        [FieldOffset(0x80)]
        public uint ResidentDataMemorySize;

        [FieldOffset(0x84)]
        public uint GPUDataMemorySize;

        [FieldOffset(0x88)]
        public uint BufferCount;

        [FieldOffset(0x8C)]
        public uint Unknow0x8C;

        [FieldOffset(0x90)]
        public uint TextureCount; // Only v1

        [FieldOffset(0x94)]
        public uint TextureDescriptorStartIndex; // Only v1
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xCC)]
    public unsafe struct RsbGroupDescriptor
    {
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        [FieldOffset(0x80)]
        public uint RsgOffset;

        [FieldOffset(0x84)]
        public uint RsgSize;

        [FieldOffset(0x88)]
        public uint PoolIndex;

        [FieldOffset(0x8C)]
        public uint RsgCompressionFlags;

        [FieldOffset(0x90)]
        public uint RsgHeaderSize;

        [FieldOffset(0x94)]
        public uint RsgResidentDataOffset;

        [FieldOffset(0x98)]
        public uint RsgResidentDataCompressedSize;

        [FieldOffset(0x9C)]
        public uint RsgResidentDataUncompressedSize;

        [FieldOffset(0xA0)]
        public uint RsgResidentDataPoolSize;

        [FieldOffset(0xA4)]
        public uint RsgGPUDataOffset;

        [FieldOffset(0xA8)]
        public uint RsgGPUDataCompressedSize;

        [FieldOffset(0xAC)]
        public uint RsgGPUDataUncompressedSize;

        [FieldOffset(0xB0)]
        public uint Unknow0xB0;

        [FieldOffset(0xB4)]
        public uint Unknow0xB4;

        [FieldOffset(0xB8)]
        public uint Unknow0xB8;

        [FieldOffset(0xBC)]
        public uint Unknow0xBC;

        [FieldOffset(0xC0)]
        public uint Unknow0xC0;

        [FieldOffset(0xC4)]
        public uint TextureCount; // Only v3/v4

        [FieldOffset(0xC8)]
        public uint TextureDescriptorStartIndex; // Only v3/v4
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x70)]
    public struct RsbInfo
    {
        [FieldOffset(0x0)]
        public uint Magic;

        [FieldOffset(0x4)]
        public uint Version;

        [FieldOffset(0x8)]
        public uint MinorVersion;

        [FieldOffset(0xC)]
        public uint HeaderSize;

        [FieldOffset(0x10)]
        public uint GlobalFileIndexMapSize;

        [FieldOffset(0x14)]
        public uint GlobalFileIndexMapOffset;

        [FieldOffset(0x18)]
        public uint Unknow0x18;

        [FieldOffset(0x1C)]
        public uint Unknow0x1C;

        [FieldOffset(0x20)]
        public uint GroupIndexMapSize;

        [FieldOffset(0x24)]
        public uint GroupIndexMapOffset;

        [FieldOffset(0x28)]
        public uint GroupCount;

        [FieldOffset(0x2C)]
        public uint GroupDescriptorOffset;

        [FieldOffset(0x30)]
        public uint GroupDescriptorPitch;

        [FieldOffset(0x34)]
        public uint CompositeCount;

        [FieldOffset(0x38)]
        public uint CompositeDescriptorOffset;

        [FieldOffset(0x3C)]
        public uint CompositeDescriptorPitch;

        [FieldOffset(0x40)]
        public uint CompositeIndexMapSize;

        [FieldOffset(0x44)]
        public uint CompositeIndexMapOffset;

        [FieldOffset(0x48)]
        public uint PoolCount;

        [FieldOffset(0x4C)]
        public uint PoolDescriptorOffset;

        [FieldOffset(0x50)]
        public uint PoolDescriptorPitch;

        [FieldOffset(0x54)]
        public uint TextureCount;

        [FieldOffset(0x58)]
        public uint TextureDescriptorOffset;

        [FieldOffset(0x5C)]
        public uint TextureDescriptorPitch;

        [FieldOffset(0x60)]
        public uint ManifestGroupInfoOffset;

        [FieldOffset(0x64)]
        public uint ManifestResourceInfoOffset;

        [FieldOffset(0x68)]
        public uint ManifestStringPoolOffset;

        [FieldOffset(0x6C)]
        public uint HeaderSizeNoManifest;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x5C)]
    public unsafe struct RsgInfo
    {
        [FieldOffset(0x0)]
        public uint Magic;

        [FieldOffset(0x4)]
        public uint Version;

        [FieldOffset(0x8)]
        public uint MinorVersion;

        [FieldOffset(0xC)]
        public uint Unknow0xC;

        [FieldOffset(0x10)]
        public uint CompressionFlags;

        [FieldOffset(0x14)]
        public uint HeaderSize;

        [FieldOffset(0x18)]
        public uint ResidentDataOffset;

        [FieldOffset(0x1C)]
        public uint ResidentDataCompressedSize;

        [FieldOffset(0x20)]
        public uint ResidentDataUncompressedSize;

        [FieldOffset(0x24)]
        public uint Unknow0x24;

        [FieldOffset(0x28)]
        public uint GPUDataOffset;

        [FieldOffset(0x2C)]
        public uint GPUDataCompressedSize;

        [FieldOffset(0x30)]
        public uint GPUDataUncompressedSize;

        [FieldOffset(0x34)]
        public uint Unknow0x34;

        [FieldOffset(0x38)]
        public uint Unknow0x38;

        [FieldOffset(0x3C)]
        public uint Unknow0x3C;

        [FieldOffset(0x40)]
        public uint Unknow0x40;

        [FieldOffset(0x44)]
        public uint Unknow0x44;

        [FieldOffset(0x48)]
        public uint FileIndexDataMapSize;

        [FieldOffset(0x4C)]
        public uint FileIndexDataMapOffset;

        [FieldOffset(0x50)]
        public uint Unknow0x50;

        [FieldOffset(0x54)]
        public uint Unknow0x54;

        [FieldOffset(0x58)]
        public uint Unknow0x58;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
    public unsafe struct RsgResidentFileExtraData
    {
        [FieldOffset(0x0)]
        public uint Type;

        [FieldOffset(0x4)]
        public uint Offset;

        [FieldOffset(0x8)]
        public uint Size;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]
    public unsafe struct RsgImageExtraData
    {
        [FieldOffset(0x0)]
        public uint Type;

        [FieldOffset(0x4)]
        public uint Offset;

        [FieldOffset(0x8)]
        public uint Size;

        [FieldOffset(0xC)]
        public uint Index;

        [FieldOffset(0x10)]
        public uint Unknow0x10;

        [FieldOffset(0x14)]
        public uint Unknow0x14;

        [FieldOffset(0x18)]
        public uint Width;

        [FieldOffset(0x1C)]
        public uint Height;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
    public struct ManifestCompositeGroupInfo
    {
        [FieldOffset(0x0)]
        public uint IdOffset;

        [FieldOffset(0x4)]
        public uint SubGroupCount;

        [FieldOffset(0x8)]
        public uint SubGroupInfoEachSize;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
    public struct ManifestSubGroupInfoForRsbV1
    {
        [FieldOffset(0x0)]
        public uint Res;

        [FieldOffset(0x4)]
        public uint IdOffset;

        [FieldOffset(0x8)]
        public uint ResourceCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
    public struct ManifestSubGroupInfoForRsbV3V4
    {
        [FieldOffset(0x0)]
        public uint Res;

        [FieldOffset(0x4)]
        public uint Loc;

        [FieldOffset(0x8)]
        public uint IdOffset;

        [FieldOffset(0xC)]
        public uint ResourceCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x4)]
    public struct ManifestResourceInfo
    {
        [FieldOffset(0x0)]
        public uint Offset;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x1C)]
    public struct ManifestResourceHeader
    {
        [FieldOffset(0x0)]
        public uint Unknow0x0;

        [FieldOffset(0x4)]
        public ushort Type;

        [FieldOffset(0x6)]
        public ushort HeaderSize;

        [FieldOffset(0x8)]
        public uint UniversalPropertyOffset;

        [FieldOffset(0xC)]
        public uint ImagePropertyOffset;

        [FieldOffset(0x10)]
        public uint IdOffset;

        [FieldOffset(0x14)]
        public uint PathOffset;

        [FieldOffset(0x18)]
        public uint UniversalPropertyCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x18)]
    public struct ManifestResourceImageProperty
    {
        [FieldOffset(0x0)]
        public ushort Atlas;

        [FieldOffset(0x2)]
        public ushort AtlasFlags;

        [FieldOffset(0x4)]
        public ushort OffsetX;

        [FieldOffset(0x6)]
        public ushort OffsetY;

        [FieldOffset(0x8)]
        public ushort AtlasX;

        [FieldOffset(0xA)]
        public ushort AtlasY;

        [FieldOffset(0xC)]
        public ushort AtlasWidth;

        [FieldOffset(0xE)]
        public ushort AtlasHeight;

        [FieldOffset(0x10)]
        public ushort Rows;

        [FieldOffset(0x12)]
        public ushort Cols;

        [FieldOffset(0x14)]
        public uint ParentOffset;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
    public struct ManifestResourceUniversalProperty
    {
        [FieldOffset(0x0)]
        public uint KeyOffset;

        [FieldOffset(0x4)]
        public uint Unknow0x4;

        [FieldOffset(0x8)]
        public uint ValueOffset;
    }
}
