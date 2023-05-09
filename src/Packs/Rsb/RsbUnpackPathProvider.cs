using LibWindPop.Utils.FileSystem;

namespace LibWindPop.Packs.Rsb
{
    public readonly struct RsbUnpackPathProvider
    {
        public readonly string UnpackPath;
        public readonly string InfoPackInfoPath;
        public readonly string InfoManifestPath;
        public readonly string InfoContentPipelinePath;
        public readonly bool UseGroupFolder;

        private readonly IFileSystem m_fileSystem;

        private readonly string m_resourcePath;
        private readonly string m_rgtempPath;
        private readonly string m_unusedPath;
        private readonly string m_groupPath;


        public RsbUnpackPathProvider(string unpackPath, IFileSystem fileSystem, bool UseGroupFolder)
        {
            UnpackPath = unpackPath;
            m_fileSystem = fileSystem;
            this.UseGroupFolder = UseGroupFolder;
            m_resourcePath = m_fileSystem.Combine(UnpackPath, "resource");
            m_rgtempPath = m_fileSystem.Combine(UnpackPath, "rgtemp");
            m_unusedPath = m_fileSystem.Combine(UnpackPath, "unused");
            m_groupPath = m_fileSystem.Combine(UnpackPath, "group");
            string infoPath = m_fileSystem.Combine(UnpackPath, "info");
            InfoPackInfoPath = m_fileSystem.Combine(infoPath, "packinfo.json");
            InfoManifestPath = m_fileSystem.Combine(infoPath, "manifest.json");
            InfoContentPipelinePath = m_fileSystem.Combine(infoPath, "pipeline.json");
        }

        public string GetRsgPathByGroupId(string? groupId)
        {
            return UseGroupFolder ? m_fileSystem.Combine(m_fileSystem.Combine(m_groupPath, groupId), $"{groupId}.rsg") : m_fileSystem.Combine(m_rgtempPath, $"{groupId}.rsg");
        }

        public string GetResourcePathByGroupIdAndPath(string? groupId, string? path)
        {
            return UseGroupFolder
                ? m_fileSystem.Combine(m_groupPath, groupId, "resource", path)
                : m_fileSystem.Combine(m_resourcePath, path);
        }

        public string GetUnusedResourcePathByGroupIdAndIndex(string? groupId, uint index, out string recordPath)
        {
            recordPath = $"{groupId}_{index}.PTX";
            return UseGroupFolder
                ? m_fileSystem.Combine(m_groupPath, groupId, "unused", recordPath)
                : m_fileSystem.Combine(m_unusedPath, recordPath);
        }

        public string GetUnusedResourcePathByGroupIdAndPath(string? groupId, string? recordPath)
        {
            return UseGroupFolder
                ? m_fileSystem.Combine(m_groupPath, groupId, "unused", recordPath)
                : m_fileSystem.Combine(m_unusedPath, recordPath);
        }

        public string AppendMetaExtension(string rawPath)
        {
            return rawPath + ".meta.json";
        }
    }
}
