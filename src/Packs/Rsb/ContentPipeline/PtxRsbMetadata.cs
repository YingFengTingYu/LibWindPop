using LibWindPop.Utils.Json;
using System;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    internal class PtxRsbMetadata : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public string? ImageHandler { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public uint Pitch { get; set; }

        public uint Format { get; set; }

        public uint AlphaSize { get; set; }

        public DateTime PngModifyTimeUtc { get; set; }
    }
}
