using LibWindPop.Packs.Common;
using LibWindPop.Utils;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using System;
using System.Collections.Generic;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    public sealed class PakRebuildFile : IContentPipeline
    {
        public void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            BuildInternal(unpackPath, fileSystem, logger);
        }

        public void OnEndBuild(string packPath, IFileSystem fileSystem, ILogger logger)
        {

        }

        public void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            AddInternal(unpackPath, fileSystem, logger);
        }

        private static string GetSettingPath(PakUnpackPathProvider paths, IFileSystem fileSystem)
        {
            return fileSystem.Combine(paths.InfoPath, "pak_rebuild_file.setting.json");
        }

        private static bool IsPtxFile(string? path)
        {
            return path != null && path.EndsWith(".ptx", StringComparison.CurrentCultureIgnoreCase);
        }

        private static void BuildInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            PakUnpackPathProvider paths = new PakUnpackPathProvider(unpackPath, fileSystem);
            logger.Log("Read pak pack info...");

            PakRebuildFileSetting? setting = WindJsonSerializer.TryDeserializeFromFile<PakRebuildFileSetting>(GetSettingPath(paths, fileSystem), fileSystem, logger);

            setting ??= new PakRebuildFileSetting { PathSeparator = "\\", UnusedPathSeparator = "/", PtxAlign4K = false };
            setting.PathSeparator ??= string.Empty;
            setting.UnusedPathSeparator ??= string.Empty;

            PakPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<PakPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo == null)
            {
                logger.LogError("Pack info is null");
            }
            else
            {
                List<string> nativeFiles = new List<string>();
                GetAllFiles(paths.ResourcePath, string.Empty, setting.PathSeparator, fileSystem, nativeFiles);
                List<PakPackFileInfo> pakFiles = packInfo.RecordFiles ?? new List<PakPackFileInfo>(512);
                for (int i = pakFiles.Count - 1; i >= 0; i--)
                {
                    bool delete = true;
                    PakPackFileInfo? pakFile = pakFiles[i];
                    if (pakFile != null && !string.IsNullOrEmpty(pakFile.Path))
                    {
                        string recordPath = pakFile.Path.Replace(setting.UnusedPathSeparator, setting.PathSeparator);
                        if (nativeFiles.Remove(recordPath))
                        {
                            pakFile.Path = recordPath;
                            delete = false;
                        }
                    }
                    if (delete)
                    {
                        logger.Log($"Delete file info {pakFile?.Path}");
                        pakFiles.RemoveAt(i);
                    }
                }
                foreach (string newPakFile in nativeFiles)
                {
                    logger.Log($"Add file info {newPakFile}");
                    pakFiles.Add(new PakPackFileInfo
                    {
                        Path = newPakFile,
                        UseZlib = false,
                        UseAlign4K = setting.PtxAlign4K && IsPtxFile(newPakFile),
                        TimeUtc = DateTime.UtcNow
                    });
                }
                packInfo.RecordFiles = pakFiles;
                WindJsonSerializer.TrySerializeToFile(paths.InfoPackInfoPath, packInfo, fileSystem, logger);
            }
        }

        private static void AddInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            string pathSeparator = "\\";
            string unusedPathSeparator = "/";
            
            bool ptxAlign4K = false;

            PakUnpackPathProvider paths = new PakUnpackPathProvider(unpackPath, fileSystem);
            PakPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<PakPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            if (packInfo != null && packInfo.RecordFiles != null)
            {
                int pathSeparatorCount = 0;
                int unusedPathSeparatorCount = 0;
                foreach (PakPackFileInfo? pakFileInfo in packInfo.RecordFiles)
                {
                    if (pakFileInfo != null)
                    {
                        if (pakFileInfo.Path != null)
                        {
                            if (pakFileInfo.Path.IndexOf(pathSeparator) != -1)
                            {
                                pathSeparatorCount++;
                            }
                            if (pakFileInfo.Path.IndexOf(unusedPathSeparator) != -1)
                            {
                                unusedPathSeparatorCount++;
                            }
                            if ((!ptxAlign4K) && pakFileInfo.UseAlign4K && IsPtxFile(pakFileInfo.Path))
                            {
                                ptxAlign4K = true;
                            }
                        }
                    }
                }
                if (unusedPathSeparatorCount > pathSeparatorCount)
                {
                    (pathSeparator, unusedPathSeparator) = (unusedPathSeparator, pathSeparator);
                }
            }
            PakRebuildFileSetting setting = new PakRebuildFileSetting
            {
                PathSeparator = pathSeparator,
                UnusedPathSeparator = unusedPathSeparator,
                PtxAlign4K = ptxAlign4K
            };
            WindJsonSerializer.TrySerializeToFile(fileSystem.Combine(paths.InfoPath, GetSettingPath(paths, fileSystem)), setting, fileSystem, logger);
        }

        private static void GetAllFiles(string path, string add, string separator, IFileSystem fileSystem, List<string> answer)
        {
            if ((!string.IsNullOrEmpty(add)) && !add.EndsWith(separator))
            {
                add += separator;
            }
            foreach (string folder in fileSystem.GetFolders(path))
            {
                GetAllFiles(folder, add + fileSystem.GetFileName(folder), separator, fileSystem, answer);
            }
            foreach (string file in fileSystem.GetFiles(path))
            {
                answer.Add(add + fileSystem.GetFileName(file));
            }
        }
    }
}
