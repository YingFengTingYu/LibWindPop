using LibWindPop.Utils.Json;

namespace LibWindPop.Packs.Rsb
{
    public class RsbPackInfo : IJsonVersionCheckable
    {
        public static uint JsonVersion => 1u;

        public uint MajorVersion { get; set; }

        public uint MinorVersion { get; set; }

        public bool UseBigEndian { get; set; }

        public bool UseExternalRsg { get; set; }

        public bool UseGroupFolder { get; set; }

        public bool UseGlobalFileIndexMap { get; set; }

        public bool UseManifest { get; set; }

        public uint TextureDescriptorPitch { get; set; }

        public string? ImageHandler { get; set; }

        public RsbPackGroupInfo[]? Groups { get; set; }

        public RsbPackCompositeInfo[]? Composites { get; set; }

        public RsbPackPoolInfo[]? Pools { get; set; }
    }

    public class RsbPackPoolInfo
    {
        public uint Id { get; set; }

        public string? Name { get; set; }

        public uint NumInstances { get; set; }
    }

    public class RsbPackCompositeInfo
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public RsbPackSubGroupInfo[]? SubGroups { get; set; }
    }

    public class RsbPackSubGroupInfo
    {
        public string? Id { get; set; }

        public uint Res { get; set; }

        public string? Loc { get; set; }
    }

    public class RsbPackGroupInfo
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public uint PoolId { get; set; }

        public uint CompressionFlags { get; set; }

        public RsbPackGroupResidentFileInfo[]? ResidentFileList { get; set; }

        public RsbPackGroupGPUFileInfo[]? GPUFileList { get; set; }
    }

    public class RsbPackGroupResidentFileInfo
    {
        public string? Path { get; set; }
    }

    public class RsbPackGroupGPUFileInfo
    {
        public string? Path { get; set; }

        public bool InFileIndexDataMap { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public uint Pitch { get; set; }

        public uint Format { get; set; }

        public uint Extend1 { get; set; }

        public uint Extend2 { get; set; }
    }
}
