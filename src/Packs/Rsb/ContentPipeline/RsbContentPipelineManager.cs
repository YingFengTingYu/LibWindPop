﻿using LibWindPop.Packs.Common;
using LibWindPop.Utils;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System.Collections.Generic;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public static class RsbContentPipelineManager
    {
        private static readonly Dictionary<string, IContentPipeline?> m_ContentPipelineMap = new Dictionary<string, IContentPipeline?>
        {
            { nameof(UpdateRsgCache), new UpdateRsgCache() },
            { nameof(EncodePtxFromPng), new EncodePtxFromPng() },
        };

        public static IContentPipeline? GetContentPipeline(string? contentName)
        {
            return contentName == null ? null : (m_ContentPipelineMap.TryGetValue(contentName, out IContentPipeline? content) ? content : null);
        }

        public static bool RegistPipeline(string pipelineName, IContentPipeline pipeline)
        {
            return m_ContentPipelineMap.TryAdd(pipelineName, pipeline);
        }

        public static void AddContentPipeline(string unpackPath, string pipelineName, bool atFirst, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);
            ContentPipelineInfo pipelineInfo = WindJsonSerializer.TryDeserializeFromFile<ContentPipelineInfo>(paths.InfoContentPipelinePath, 0u, fileSystem, new NullLogger(), false) ?? new ContentPipelineInfo();
            pipelineInfo.Pipelines ??= new List<string>();
            if (!pipelineInfo.Pipelines.Contains(pipelineName))
            {
                if (atFirst)
                {
                    pipelineInfo.Pipelines.Insert(0, pipelineName);
                }
                else
                {
                    pipelineInfo.Pipelines.Add(pipelineName);
                }
                IContentPipeline? pipeline = GetContentPipeline(pipelineName);
                if (pipeline == null)
                {
                    logger.LogError($"Can not find content pipeline: {pipelineName}", 0, throwException);
                }
                else
                {
                    pipeline.OnAdd(unpackPath, fileSystem, logger, throwException);
                }
                WindJsonSerializer.TrySerializeToFile(paths.InfoContentPipelinePath, pipelineInfo, 0u, fileSystem, logger, throwException);
            }
        }

        internal static void StartBuildContentPipeline(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read content pipeline info...", 0);
            ContentPipelineInfo? pipelineInfo = WindJsonSerializer.TryDeserializeFromFile<ContentPipelineInfo>(paths.InfoContentPipelinePath, 0u, fileSystem, logger, throwException);

            if (pipelineInfo != null && pipelineInfo.Pipelines != null)
            {
                for (int i = 0; i < pipelineInfo.Pipelines.Count; i++)
                {
                    string? pipelineName = pipelineInfo.Pipelines[i];
                    IContentPipeline? pipeline = GetContentPipeline(pipelineName);
                    if (pipeline == null)
                    {
                        logger.LogError($"Can not find content pipeline: {pipelineName}", 0, throwException);
                    }
                    else
                    {
                        logger.Log($"Build content pipeline: {pipelineName}", 0);
                        pipeline.OnStartBuild(unpackPath, fileSystem, logger, throwException);
                    }
                }
            }
        }

        internal static void EndBuildContentPipeline(string rsbPath, string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read content pipeline info...", 0);
            ContentPipelineInfo? pipelineInfo = WindJsonSerializer.TryDeserializeFromFile<ContentPipelineInfo>(paths.InfoContentPipelinePath, 0u, fileSystem, logger, throwException);

            if (pipelineInfo != null && pipelineInfo.Pipelines != null)
            {
                for (int i = 0; i < pipelineInfo.Pipelines.Count; i++)
                {
                    string? pipelineName = pipelineInfo.Pipelines[i];
                    IContentPipeline? pipeline = GetContentPipeline(pipelineName);
                    if (pipeline == null)
                    {
                        logger.LogError($"Can not find content pipeline: {pipelineName}", 0, throwException);
                    }
                    else
                    {
                        logger.Log($"Build content pipeline: {pipelineName}", 0);
                        pipeline.OnEndBuild(rsbPath, fileSystem, logger, throwException);
                    }
                }
            }
        }
    }
}
