using LibWindPop.Utils.FileSystem;

namespace LibWindPop.Packs.Pak
{
    public readonly struct PakUnpackPathProvider
    {
        public readonly string UnpackPath;
        public readonly string InfoPath;
        public readonly string InfoPackInfoPath;
        public readonly string InfoContentPipelinePath;
        public readonly string ResourcePath;
        private readonly IFileSystem m_fileSystem;

        public PakUnpackPathProvider(string unpackPath, IFileSystem fileSystem)
        {
            UnpackPath = unpackPath;
            m_fileSystem = fileSystem;
            ResourcePath = m_fileSystem.Combine(UnpackPath, "resource");
            InfoPath = m_fileSystem.Combine(UnpackPath, "info");
            InfoPackInfoPath = m_fileSystem.Combine(InfoPath, "pack_info.json");
            InfoContentPipelinePath = m_fileSystem.Combine(InfoPath, "pipeline.json");
        }

        public string GetFilePath(string recordPath)
        {
            return m_fileSystem.Combine(ResourcePath, recordPath);
        }
    }
}
