using LibWindPop.Packs.Common;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Collections.Generic;
using LibWindPop.Utils.Json;
using LibWindPop.Images.PtxXbox360;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    public sealed class PakPtxXbox360AutoEncoder : IContentPipeline
    {
        public void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            BuildInternal(unpackPath, fileSystem, logger);
        }

        public void OnEndBuild(string rsbPath, IFileSystem fileSystem, ILogger logger)
        {
            // Nothing to do
        }

        public void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            AddInternal(unpackPath, fileSystem, logger);
        }

        private static unsafe void BuildInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            // define base path
            PakUnpackPathProvider paths = new PakUnpackPathProvider(unpackPath, fileSystem);

            logger.Log("Read pak pack info...");

            PakPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<PakPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo == null || packInfo.RecordFiles == null)
            {
                logger.LogError("Pack info is null");
            }
            else
            {
                List<PakPackFileInfo> files = packInfo.RecordFiles;
                {
                    for (int i = files.Count - 1; i >= 0; i--)
                    {
                        PakPackFileInfo? file = files[i];
                        if (file != null && file.UseAlign4K && file.Path != null && file.Path.EndsWith(".ptx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string nativePtxPath = paths.GetFilePath(file.Path);
                            string nativePngPath = GetNativePngPath(unpackPath, file.Path, fileSystem);
                            string nativeMetaPath = GetNativeMetaPath(unpackPath, file.Path, fileSystem);
                            bool convert = true;
                            PtxPakMetadata? meta = null;
                            if (fileSystem.FileExists(nativeMetaPath))
                            {
                                meta = WindJsonSerializer.TryDeserializeFromFile<PtxPakMetadata>(nativeMetaPath, fileSystem, new NullLogger());
                                if (meta != null && meta.PngModifyTimeUtc == fileSystem.GetModifyTimeUtc(nativePngPath) && fileSystem.FileExists(nativePtxPath))
                                {
                                    convert = false;
                                }
                            }
                            if (convert)
                            {
                                meta ??= new PtxPakMetadata { Format = null };
                                logger.Log($"Create {file.Path} by format {meta.Format}");
                                PtxXbox360Coder.Encode(nativePngPath, nativePtxPath, fileSystem, logger);
                                meta.PngModifyTimeUtc = fileSystem.GetModifyTimeUtc(nativePngPath);
                                meta.LastPtxFormat = meta.Format;
                                WindJsonSerializer.TrySerializeToFile(nativeMetaPath, meta, fileSystem, logger);
                            }
                        }
                    }
                }
            }
        }

        private static unsafe void AddInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            // define base path
            PakUnpackPathProvider paths = new PakUnpackPathProvider(unpackPath, fileSystem);

            logger.Log("Read pak pack info...");

            PakPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<PakPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo == null || packInfo.RecordFiles == null)
            {
                logger.LogError("Pack info is null");
            }
            else
            {
                List<PakPackFileInfo> files = packInfo.RecordFiles;
                {
                    for (int i = files.Count - 1; i >= 0; i--)
                    {
                        PakPackFileInfo? file = files[i];
                        if (file != null && file.UseAlign4K && file.Path != null && file.Path.EndsWith(".ptx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string nativePtxPath = paths.GetFilePath(file.Path);
                            string nativePngPath = GetNativePngPath(unpackPath, file.Path, fileSystem);
                            string nativeMetaPath = GetNativeMetaPath(unpackPath, file.Path, fileSystem);
                            PtxXbox360Coder.Decode(nativePtxPath, nativePngPath, fileSystem, logger);
                            PtxPakMetadata meta = new PtxPakMetadata
                            {
                                Format = null,
                                PngModifyTimeUtc = fileSystem.GetModifyTimeUtc(nativePngPath),
                                LastPtxFormat = null
                            };
                            WindJsonSerializer.TrySerializeToFile(nativeMetaPath, meta, fileSystem, logger);
                        }
                    }
                }
            }
        }

        private static string GetNativePngPath(string unpackPath, string recordPath, IFileSystem fileSystem)
        {
            return fileSystem.Combine(unpackPath, "ptx_xbox360_raw_image", fileSystem.ChangeExtension(recordPath, ".png"));
        }

        private static string GetNativeMetaPath(string unpackPath, string recordPath, IFileSystem fileSystem)
        {
            return fileSystem.Combine(unpackPath, "ptx_xbox360_raw_image", recordPath + ".meta.json");
        }
    }
}
