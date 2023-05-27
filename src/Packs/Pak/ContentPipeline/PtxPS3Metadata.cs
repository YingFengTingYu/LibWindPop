using LibWindPop.Utils.Json;
using System;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    internal class PtxPS3Metadata : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public string? Format { get; set; }

        public DateTime PngModifyTimeUtc { get; set; }

        public string? LastPtxFormat { get; set; }
    }
}
