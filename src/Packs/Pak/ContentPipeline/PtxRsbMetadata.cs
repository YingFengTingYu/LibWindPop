using LibWindPop.Utils.Graphics.FormatProvider.Dds;
using LibWindPop.Utils.Json;
using System;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    internal class PtxPS3Metadata : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public DdsEncodingFormat Format { get; set; }

        public DateTime PngModifyTimeUtc { get; set; }
    }
}
