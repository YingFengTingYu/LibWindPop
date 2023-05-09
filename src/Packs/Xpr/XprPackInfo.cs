namespace LibWindPop.Packs.Xpr
{
    public class XprPackInfo
    {
        public uint XprDataOffset { get; set; }

        public bool XprDataFileAlign { get; set; }

        public XprPackFileInfo[]? RecordFiles { get; set; }
    }

    public class XprPackFileInfo
    {
        public string? Type { get; set; }

        public string? Path { get; set; }
    }
}
