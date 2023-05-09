using System.Collections.Generic;

namespace LibWindPop.Packs.Rsb
{
    public class RsbManifestInfo
    {
        public RsbManifestCompositeInfo[]? CompositeGroups { get; set; }
    }

    public class RsbManifestCompositeInfo
    {
        public string? Id { get; set; }

        public RsbManifestGroupInfo[]? SubGroups { get; set; }
    }

    public class RsbManifestGroupInfo
    {
        public string? Id { get; set; }

        public uint Res { get; set; }

        public string? Loc { get; set; }

        public RsbManifestResourceInfo[]? Resources { get; set; }
    }

    public class RsbManifestResourceInfo
    {
        public uint Type { get; set; }

        public string? Id { get; set; }

        public string? Path { get; set; }

        public RsbManifestImageProperty? ImageProperties { get; set; }

        public Dictionary<string, string>? UniversalProperties { get; set; }
    }

    public class RsbManifestImageProperty
    {
        public uint Atlas { get; set; }

        public uint AtlasFlags { get; set; }

        public uint OffsetX { get; set; }

        public uint OffsetY { get; set; }

        public uint AtlasX { get; set; }

        public uint AtlasY { get; set; }

        public uint AtlasWidth { get; set; }

        public uint AtlasHeight { get; set; }

        public uint Rows { get; set; }

        public uint Cols { get; set; }

        public string? Parent { get; set; }
    }
}
