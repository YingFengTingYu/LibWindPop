using System.Runtime.InteropServices;

namespace LibWindPop.Packs.Rsb.RsbStructures
{
    // from Bejeweled Classic iOS version

    /// <summary>
    /// Sexy::ResStreamCompositeDescriptor
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x484)]
    public unsafe struct ResStreamCompositeDescriptorV3V4
    {
        /// <summary>
        /// name of composite
        /// </summary>
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        //[FieldOffset(0x80)]
        //public fixed ResStreamCompositeDescriptorChildV3V4 Childs[64];

        /// <summary>
        /// count of child
        /// </summary>
        [FieldOffset(0x480)]
        public uint ChildCount;
    }

    /// <summary>
    /// Sexy::ResStreamCompositeDescriptor::Child
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
    public unsafe struct ResStreamCompositeDescriptorChildV3V4
    {
        /// <summary>
        /// index of ResStreamsGroup
        /// </summary>
        [FieldOffset(0x0)]
        public uint GroupIndex;

        /// <summary>
        /// art resolution
        /// </summary>
        [FieldOffset(0x4)]
        public uint ArtResolution;

        /// <summary>
        /// localization
        /// </summary>
        [FieldOffset(0x8)]
        public uint Localization;
    }

    /// <summary>
    /// Sexy::ResStreamCompositeDescriptor
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x284)]
    public unsafe struct ResStreamCompositeDescriptorV1
    {
        /// <summary>
        /// name of composite
        /// </summary>
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        //[FieldOffset(0x80)]
        //public fixed ResStreamCompositeDescriptorChildV1 Childs[64];

        /// <summary>
        /// count of child
        /// </summary>
        [FieldOffset(0x280)]
        public uint SubGroupCount;
    }

    /// <summary>
    /// Sexy::ResStreamCompositeDescriptor::Child
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]
    public unsafe struct ResStreamCompositeDescriptorChildV1
    {
        /// <summary>
        /// index of ResStreamsGroup
        /// </summary>
        [FieldOffset(0x0)]
        public uint GroupIndex;

        /// <summary>
        /// art resolution
        /// </summary>
        [FieldOffset(0x4)]
        public uint ArtResolution;
    }

    /// <summary>
    /// Sexy::ResStreamTextureDescriptor
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x18)]
    public struct ResStreamTextureDescriptor
    {
        /// <summary>
        /// ptx width
        /// </summary>
        [FieldOffset(0x0)]
        public uint Width;

        /// <summary>
        /// ptx height
        /// </summary>
        [FieldOffset(0x4)]
        public uint Height;

        /// <summary>
        /// ptx pitch
        /// </summary>
        [FieldOffset(0x8)]
        public uint Pitch;

        /// <summary>
        /// ptx format
        /// </summary>
        [FieldOffset(0xC)]
        public uint Format;

        /// <summary>
        /// ptx pvz2cn alpha size or scale or null
        /// </summary>
        [FieldOffset(0x10)]
        public uint Extend1; // PVZ2CN

        /// <summary>
        /// ptx pvz2cn scale or null
        /// </summary>
        [FieldOffset(0x14)]
        public uint Extend2; // PVZ2CN
    }

    /// <summary>
    /// Sexy::ResStreamPoolDescriptor
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x98)]
    public unsafe struct ResStreamPoolDescriptor
    {
        /// <summary>
        /// name of pool
        /// </summary>
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        /// <summary>
        /// allocated size of each instance of pool for resident data for rsg info and uncompressed resident data
        /// </summary>
        [FieldOffset(0x80)]
        public uint ResidentDataMemorySize;

        /// <summary>
        /// allocated size for rsg uncompressed texture data
        /// </summary>
        [FieldOffset(0x84)]
        public uint GPUDataMemorySize;

        /// <summary>
        /// instance count
        /// </summary>
        [FieldOffset(0x88)]
        public uint NumInstances;

        /// <summary>
        /// Unknow boolean (I have never seen this value was used)
        /// </summary>
        [FieldOffset(0x8C)]
        public uint UnknowBoolean;

        /// <summary>
        /// count of texture for rsb v1
        /// </summary>
        [FieldOffset(0x90)]
        public uint TextureCount; // Only v1

        /// <summary>
        /// offset of texture descriptor for rsb v1
        /// </summary>
        [FieldOffset(0x94)]
        public uint TextureDescriptorStartIndex; // Only v1
    }

    /// <summary>
    /// Sexy::ResStreamGroupDescriptor
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xCC)]
    public unsafe struct ResStreamGroupDescriptor
    {
        /// <summary>
        /// name of group
        /// </summary>
        [FieldOffset(0x0)]
        public fixed byte Name[0x80];

        /// <summary>
        /// offset of rsg in rsb
        /// </summary>
        [FieldOffset(0x80)]
        public uint RsgOffset;

        /// <summary>
        /// size of rsg
        /// </summary>
        [FieldOffset(0x84)]
        public uint RsgSize;

        /// <summary>
        /// index of pool
        /// </summary>
        [FieldOffset(0x88)]
        public uint PoolIndex;

        /// <summary>
        /// compression flags
        /// </summary>
        [FieldOffset(0x8C)]
        public uint RsgCompressionFlags;

        /// <summary>
        /// size of res stream header
        /// </summary>
        [FieldOffset(0x90)]
        public uint RsgHeaderSize;

        /// <summary>
        /// resident data offset of rsg
        /// </summary>
        [FieldOffset(0x94)]
        public uint RsgResidentDataOffset;

        /// <summary>
        /// compressed resident data size
        /// </summary>
        [FieldOffset(0x98)]
        public uint RsgResidentDataCompressedSize;

        /// <summary>
        /// uncompressed resident data size
        /// </summary>
        [FieldOffset(0x9C)]
        public uint RsgResidentDataUncompressedSize;

        /// <summary>
        /// used pool resident data size
        /// </summary>
        [FieldOffset(0xA0)]
        public uint RsgResidentDataPoolSize;

        /// <summary>
        /// GPU data offset of rsg
        /// </summary>
        [FieldOffset(0xA4)]
        public uint RsgGPUDataOffset;

        /// <summary>
        /// compressed GPU data size
        /// </summary>
        [FieldOffset(0xA8)]
        public uint RsgGPUDataCompressedSize;

        /// <summary>
        /// uncompressed GPU data size
        /// </summary>
        [FieldOffset(0xAC)]
        public uint RsgGPUDataUncompressedSize;

        /// <summary>
        /// used pool GPU data size (but GPU data never hold memory in pool)
        /// </summary>
        [FieldOffset(0xB0)]
        public uint RsgGPUDataPoolSize;

        /// <summary>
        /// count of texture for rsb v3/v4 
        /// </summary>
        [FieldOffset(0xC4)]
        public uint TextureCount;

        /// <summary>
        /// offset of texture descriptor for rsb v3/v4
        /// </summary>
        [FieldOffset(0xC8)]
        public uint TextureDescriptorStartIndex;
    }

    /// <summary>
    /// no offical name from PopCap Games
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x70)]
    public struct ResStreamsHeader
    {
        /// <summary>
        /// magic number 1bsr(LE)/rsb1(BE)
        /// </summary>
        [FieldOffset(0x0)]
        public uint Magic;

        /// <summary>
        /// major version, 1/3/4
        /// </summary>
        [FieldOffset(0x4)]
        public uint MajorVersion;

        /// <summary>
        /// minor version, 0/1
        /// </summary>
        [FieldOffset(0x8)]
        public uint MinorVersion;

        /// <summary>
        /// size of header
        /// </summary>
        [FieldOffset(0xC)]
        public uint HeaderSize;

        /// <summary>
        /// size of global file index data compiled map, or -1 for not exist
        /// </summary>
        [FieldOffset(0x10)]
        public uint GlobalFileIndexMapSize;

        /// <summary>
        /// offset of global file index data compiled map
        /// </summary>
        [FieldOffset(0x14)]
        public uint GlobalFileIndexMapOffset;

        /// <summary>
        /// size of group index data compiled map
        /// </summary>
        [FieldOffset(0x20)]
        public uint GroupIndexMapSize;

        /// <summary>
        /// offset of group index data compiled map
        /// </summary>
        [FieldOffset(0x24)]
        public uint GroupIndexMapOffset;

        /// <summary>
        /// count of group
        /// </summary>
        [FieldOffset(0x28)]
        public uint GroupCount;

        /// <summary>
        /// offset of group descriptor
        /// </summary>
        [FieldOffset(0x2C)]
        public uint GroupDescriptorOffset;

        /// <summary>
        /// size of group descriptor
        /// </summary>
        [FieldOffset(0x30)]
        public uint GroupDescriptorSize;

        /// <summary>
        /// count of composite
        /// </summary>
        [FieldOffset(0x34)]
        public uint CompositeCount;

        /// <summary>
        /// offset of composite descriptor
        /// </summary>
        [FieldOffset(0x38)]
        public uint CompositeDescriptorOffset;

        /// <summary>
        /// size of composite descriptor
        /// </summary>
        [FieldOffset(0x3C)]
        public uint CompositeDescriptorSize;

        /// <summary>
        /// size of composite index data compiled map
        /// </summary>
        [FieldOffset(0x40)]
        public uint CompositeIndexMapSize;

        /// <summary>
        /// offset of composite index data compiled map
        /// </summary>
        [FieldOffset(0x44)]
        public uint CompositeIndexMapOffset;

        /// <summary>
        /// count of pool
        /// </summary>
        [FieldOffset(0x48)]
        public uint PoolCount;

        /// <summary>
        /// offset of pool descriptor
        /// </summary>
        [FieldOffset(0x4C)]
        public uint PoolDescriptorOffset;

        /// <summary>
        /// size of pool descriptor
        /// </summary>
        [FieldOffset(0x50)]
        public uint PoolDescriptorSize;

        /// <summary>
        /// count of texture
        /// </summary>
        [FieldOffset(0x54)]
        public uint TextureCount;

        /// <summary>
        /// offset of texture descriptor
        /// </summary>
        [FieldOffset(0x58)]
        public uint TextureDescriptorOffset;

        /// <summary>
        /// size of texture descriptor
        /// </summary>
        [FieldOffset(0x5C)]
        public uint TextureDescriptorSize;

        /// <summary>
        /// manifest bin group offset
        /// </summary>
        [FieldOffset(0x60)]
        public uint ManifestGroupInfoOffset;

        /// <summary>
        /// manifest bin resource offset
        /// </summary>
        [FieldOffset(0x64)]
        public uint ManifestResourceInfoOffset;

        /// <summary>
        /// manifest bin string pool offset
        /// </summary>
        [FieldOffset(0x68)]
        public uint ManifestStringPoolOffset;

        /// <summary>
        /// header size without manifest for rsb v4
        /// </summary>
        [FieldOffset(0x6C)]
        public uint HeaderSizeNoManifest;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x5C)]
    public unsafe struct ResStreamGroupHeader
    {
        /// <summary>
        /// magic number 1bsr(LE)/rsb1(BE)
        /// </summary>
        [FieldOffset(0x0)]
        public uint Magic;

        /// <summary>
        /// major version, 1/3/4
        /// </summary>
        [FieldOffset(0x4)]
        public uint MajorVersion;

        /// <summary>
        /// minor version, 0/1
        /// </summary>
        [FieldOffset(0x8)]
        public uint MinorVersion;

        /// <summary>
        /// compression flags
        /// </summary>
        [FieldOffset(0x10)]
        public uint CompressionFlags;

        /// <summary>
        /// size of header
        /// </summary>
        [FieldOffset(0x14)]
        public uint HeaderSize;

        /// <summary>
        /// offset of resident data
        /// </summary>
        [FieldOffset(0x18)]
        public uint ResidentDataOffset;

        /// <summary>
        /// compressed size of resident data
        /// </summary>
        [FieldOffset(0x1C)]
        public uint ResidentDataCompressedSize;

        /// <summary>
        /// uncompressed size of resident data
        /// </summary>
        [FieldOffset(0x20)]
        public uint ResidentDataUncompressedSize;

        /// <summary>
        /// offset of GPU data
        /// </summary>
        [FieldOffset(0x28)]
        public uint GPUDataOffset;

        /// <summary>
        /// compressed size of GPU data
        /// </summary>
        [FieldOffset(0x2C)]
        public uint GPUDataCompressedSize;

        /// <summary>
        /// uncompressed size of GPU data
        /// </summary>
        [FieldOffset(0x30)]
        public uint GPUDataUncompressedSize;

        /// <summary>
        /// map size
        /// </summary>
        [FieldOffset(0x48)]
        public uint FileIndexDataMapSize;

        /// <summary>
        /// map offset
        /// </summary>
        [FieldOffset(0x4C)]
        public uint FileIndexDataMapOffset;
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

        [FieldOffset(0x18)]
        public uint Width;

        [FieldOffset(0x1C)]
        public uint Height;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
    public struct ManifestCompositeInfo
    {
        [FieldOffset(0x0)]
        public uint IdOffset;

        [FieldOffset(0x4)]
        public uint ChildCount;

        [FieldOffset(0x8)]
        public uint GroupInfoSize;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xC)]
    public struct ManifestGroupInfoV1
    {
        [FieldOffset(0x0)]
        public uint ArtResolution;

        [FieldOffset(0x4)]
        public uint IdOffset;

        [FieldOffset(0x8)]
        public uint ResourceCount;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
    public struct ManifestGroupInfoV3V4
    {
        [FieldOffset(0x0)]
        public uint ArtResolution;

        [FieldOffset(0x4)]
        public uint Localization;

        [FieldOffset(0x8)]
        public uint IdOffset;

        [FieldOffset(0xC)]
        public uint ResourceCount;
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

        [FieldOffset(0x8)]
        public uint ValueOffset;
    }
}
