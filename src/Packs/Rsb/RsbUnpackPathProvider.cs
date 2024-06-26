﻿using LibWindPop.Utils.FileSystem;

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

        private const string DEFAULT_GROUP_ID = "__DEFAULT__";
        private const string DEFAULT_PATH = "default";

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
            InfoPackInfoPath = m_fileSystem.Combine(infoPath, "pack_info.json");
            InfoManifestPath = m_fileSystem.Combine(infoPath, "manifest.json");
            InfoContentPipelinePath = m_fileSystem.Combine(infoPath, "pipeline.json");
        }

        public readonly string GetRsgPathByGroupId(string? groupId)
        {
            groupId ??= DEFAULT_GROUP_ID;
            return UseGroupFolder ? m_fileSystem.Combine(m_fileSystem.Combine(m_groupPath, groupId), $"{groupId}.rsg") : m_fileSystem.Combine(m_rgtempPath, $"{groupId}.rsg");
        }

        public readonly string GetResourcePathByGroupIdAndPath(string? groupId, string? path)
        {
            path ??= DEFAULT_PATH;
            groupId ??= DEFAULT_GROUP_ID;
            return UseGroupFolder
                ? m_fileSystem.Combine(m_groupPath, groupId, "resource", path)
                : m_fileSystem.Combine(m_resourcePath, path);
        }

        public readonly string GetUnusedResourcePathByGroupIdAndIndex(string? groupId, uint index, out string recordPath)
        {
            groupId ??= DEFAULT_GROUP_ID;
            recordPath = $"{groupId}_{index}.PTX";
            return UseGroupFolder
                ? m_fileSystem.Combine(m_groupPath, groupId, "unused", recordPath)
                : m_fileSystem.Combine(m_unusedPath, recordPath);
        }

        public readonly string GetUnusedResourcePathByGroupIdAndPath(string? groupId, string? recordPath)
        {
            recordPath ??= DEFAULT_PATH;
            groupId ??= DEFAULT_GROUP_ID;
            return UseGroupFolder
                ? m_fileSystem.Combine(m_groupPath, groupId, "unused", recordPath)
                : m_fileSystem.Combine(m_unusedPath, recordPath);
        }

        public readonly string AppendMetaExtension(string rawPath)
        {
            return rawPath + ".meta.json";
        }
    }
}
