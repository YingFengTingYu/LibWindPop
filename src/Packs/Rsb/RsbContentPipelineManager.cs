using LibWindPop.Packs.Rsb.ContentPipeline;
using LibWindPop.Utils;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System.Collections.Generic;

namespace LibWindPop.Packs.Rsb
{
    public static class RsbContentPipelineManager
    {
        private static Dictionary<string, IRsbContentPipeline?> m_ContentPipelineMap = new Dictionary<string, IRsbContentPipeline?>
        {
            { nameof(UpdateRsgCache), new UpdateRsgCache() },
            { nameof(EncodePtxFromPng), new EncodePtxFromPng() },
        };

        public static IRsbContentPipeline? GetContentPipeline(string? contentName)
        {
            return contentName == null ? null : (m_ContentPipelineMap.TryGetValue(contentName, out IRsbContentPipeline? content) ? content : null);
        }

        public static void AddContentPipeline(string unpackPath, string pipelineName, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);
            RsbContentPipelineInfo pipelineInfo = WindJsonSerializer.TryDeserializeFromFile<RsbContentPipelineInfo>(paths.InfoContentPipelinePath, 0u, fileSystem, new NullLogger(), false) ?? new RsbContentPipelineInfo();
            pipelineInfo.Pipelines ??= new List<string>();
            if (!pipelineInfo.Pipelines.Contains(pipelineName))
            {
                pipelineInfo.Pipelines.Add(pipelineName);
                IRsbContentPipeline? pipeline = GetContentPipeline(pipelineName);
                if (pipeline == null)
                {
                    logger.LogError($"Can not find content pipeline: {pipelineName}", 0, throwException);
                }
                else
                {
                    pipeline.Add(unpackPath, fileSystem, logger, throwException);
                }
                WindJsonSerializer.TrySerializeToFile(paths.InfoContentPipelinePath, pipelineInfo, 0u, fileSystem, logger, throwException);
            }
        }

        internal static void BuildContentPipeline(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read rsb content pipeline info...", 0);
            RsbContentPipelineInfo? pipelineInfo = WindJsonSerializer.TryDeserializeFromFile<RsbContentPipelineInfo>(paths.InfoContentPipelinePath, 0u, fileSystem, logger, throwException);

            if (pipelineInfo != null && pipelineInfo.Pipelines != null)
            {
                for (int i = pipelineInfo.Pipelines.Count - 1; i >= 0; i--)
                {
                    string? pipelineName = pipelineInfo.Pipelines[i];
                    IRsbContentPipeline? pipeline = GetContentPipeline(pipelineName);
                    if (pipeline == null)
                    {
                        logger.LogError($"Can not find content pipeline: {pipelineName}", 0, throwException);
                    }
                    else
                    {
                        logger.Log($"Build content pipeline: {pipelineName}", 0);
                        pipeline.Build(unpackPath, fileSystem, logger, throwException);
                    }
                }
            }
        }
    }
}
