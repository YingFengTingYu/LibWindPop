using LibWindPop.Utils.FileSystem;

namespace LibWindPop.Packs.Xpr
{
    public readonly struct XprUnpackPathProvider
    {
        public readonly string UnpackPath;
        public readonly string InfoPackInfoPath;
        private readonly IFileSystem m_fileSystem;
        private readonly string m_resourcePath;

        public XprUnpackPathProvider(string unpackPath, IFileSystem fileSystem)
        {
            UnpackPath = unpackPath;
            m_fileSystem = fileSystem;
            m_resourcePath = m_fileSystem.Combine(UnpackPath, "resource");
            string infoPath = m_fileSystem.Combine(UnpackPath, "info");
            InfoPackInfoPath = m_fileSystem.Combine(infoPath, "pack_info.json");
        }

        public string GetFilePath(string recordPath)
        {
            return m_fileSystem.Combine(m_resourcePath, recordPath);
        }
    }
}
