using LibWindPop.Utils.Json;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    public class PakRebuildFileSetting : IJsonVersionCheckable
    {
        public static uint JsonVersion => 0u;

        public string? PathSeparator { get; set; }

        public string? UnusedPathSeparator { get; set; }

        public bool PtxAlign4K { get; set; }
    }
}
