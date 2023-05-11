using System;

namespace LibWindPop.Packs.Pak
{
    public class PakPackInfo
    {
        public bool UseEncrypt { get; set; }

        public bool UseZlib { get; set; }

        public bool UseAlign { get; set; }

        public PakPackFileInfo[]? RecordFiles { get; set; }
    }

    public class PakPackFileInfo
    {
        public string? Path { get; set; }

        public bool UseZlib { get; set; }

        public bool UseAlign4K { get; set; }

        public DateTime TimeUtc { get; set; }
    }
}
