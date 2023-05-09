using System;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    internal class PtxMetadata
    {
        public string? ImageHandler { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public uint Pitch { get; set; }

        public uint Format { get; set; }

        public uint AlphaSize { get; set; }

        public DateTime PngModifyTimeUtc { get; set; }
    }
}
