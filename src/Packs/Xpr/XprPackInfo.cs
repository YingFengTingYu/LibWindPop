using LibWindPop.Utils.Json;

namespace LibWindPop.Packs.Xpr
{
    public class XprPackInfo : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public uint XprDataOffset { get; set; }

        public bool XprDataFileAlign { get; set; }

        public XprPackFileInfo[]? RecordFiles { get; set; }
    }

    public class XprPackFileInfo : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public string? Type { get; set; }

        public string? Path { get; set; }
    }
}
