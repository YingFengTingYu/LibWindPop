using LibWindPop.Utils.Json;
using System;
using System.Collections.Generic;

namespace LibWindPop.Packs.Pak
{
    public class PakPackInfo : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public bool UseEncrypt { get; set; }

        public bool UseZlib { get; set; }

        public int ZlibLevel { get; set; }

        public bool UseAlign { get; set; }

        public List<PakPackFileInfo>? RecordFiles { get; set; }
    }

    public class PakPackFileInfo
    {
        public string? Path { get; set; }

        public bool UseZlib { get; set; }

        public bool UseAlign4K { get; set; }

        public DateTime TimeUtc { get; set; }
    }
}
