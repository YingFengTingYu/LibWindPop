using LibWindPop.Utils.Json;
using System;
using System.Collections.Generic;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public class AtlasMetadata : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0;

        public string? Id { get; set; }

        public uint AFlags { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public List<AtlasSubImageInfo>? SubImages { get; set; }
    }

    public class AtlasSubImageInfo
    {
        public string? Id { get; set; }

        public string? Path { get; set; }

        public uint AX { get; set; }

        public uint AY { get; set; }

        public uint AW { get; set; }

        public uint AH { get; set; }

        public DateTime PngModifyTimeUtc { get; set; }
    }
}
