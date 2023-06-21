using LibWindPop.Utils.Json;
using System.Collections.Generic;

namespace LibWindPop.Packs.Rsb
{
    public class RsbManifestInfo : IJsonVersionCheckable
    {
        public static uint JsonVersion => 1u;

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

        public uint AFlags { get; set; }

        public uint X { get; set; }

        public uint Y { get; set; }

        public uint AX { get; set; }

        public uint AY { get; set; }

        public uint AW { get; set; }

        public uint AH { get; set; }

        public uint Rows { get; set; }

        public uint Cols { get; set; }

        public string? Parent { get; set; }
    }
}
