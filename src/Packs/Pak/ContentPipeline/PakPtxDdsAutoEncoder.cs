using LibWindPop.Packs.Common;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Collections.Generic;
using LibWindPop.Images.PtxDds;
using LibWindPop.Utils.Json;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    public sealed class PakPtxDdsAutoEncoder : IContentPipeline
    {
        private const PtxDdsPixelFormat DEFAULTFORMAT = PtxDdsPixelFormat.RGBA_BC3_UByte;

        private static PtxDdsPixelFormat GetFormatFromString(string? str)
        {
            return str switch
            {
                nameof(PtxDdsPixelFormat.RGB_BC1_UByte) => PtxDdsPixelFormat.RGB_BC1_UByte,
                nameof(PtxDdsPixelFormat.RGBA_BC1_UByte) => PtxDdsPixelFormat.RGBA_BC1_UByte,
                nameof(PtxDdsPixelFormat.RGBA_BC2_UByte) => PtxDdsPixelFormat.RGBA_BC2_UByte,
                nameof(PtxDdsPixelFormat.RGBA_BC3_UByte) => PtxDdsPixelFormat.RGBA_BC3_UByte,
                nameof(PtxDdsPixelFormat.L8_UByte) => PtxDdsPixelFormat.L8_UByte,
                nameof(PtxDdsPixelFormat.A8_UByte) => PtxDdsPixelFormat.A8_UByte,
                nameof(PtxDdsPixelFormat.R8_G8_B8_A8_UByte) => PtxDdsPixelFormat.R8_G8_B8_A8_UByte,
                nameof(PtxDdsPixelFormat.R8_G8_B8_X8_UByte) => PtxDdsPixelFormat.R8_G8_B8_X8_UByte,
                nameof(PtxDdsPixelFormat.R8_G8_B8_UByte) => PtxDdsPixelFormat.R8_G8_B8_UByte,
                nameof(PtxDdsPixelFormat.B8_G8_R8_A8_UByte) => PtxDdsPixelFormat.B8_G8_R8_A8_UByte,
                nameof(PtxDdsPixelFormat.B8_G8_R8_X8_UByte) => PtxDdsPixelFormat.B8_G8_R8_X8_UByte,
                nameof(PtxDdsPixelFormat.B8_G8_R8_UByte) => PtxDdsPixelFormat.B8_G8_R8_UByte,
                _ => PtxDdsPixelFormat.RGBA_BC3_UByte
            };
        }

        private static string GetStringFromFormat(PtxDdsPixelFormat format)
        {
            return format switch
            {
                PtxDdsPixelFormat.RGB_BC1_UByte => nameof(PtxDdsPixelFormat.RGB_BC1_UByte),
                PtxDdsPixelFormat.RGBA_BC1_UByte => nameof(PtxDdsPixelFormat.RGBA_BC1_UByte),
                PtxDdsPixelFormat.RGBA_BC2_UByte => nameof(PtxDdsPixelFormat.RGBA_BC2_UByte),
                PtxDdsPixelFormat.RGBA_BC3_UByte => nameof(PtxDdsPixelFormat.RGBA_BC3_UByte),
                PtxDdsPixelFormat.L8_UByte => nameof(PtxDdsPixelFormat.L8_UByte),
                PtxDdsPixelFormat.A8_UByte => nameof(PtxDdsPixelFormat.A8_UByte),
                PtxDdsPixelFormat.R8_G8_B8_A8_UByte => nameof(PtxDdsPixelFormat.R8_G8_B8_A8_UByte),
                PtxDdsPixelFormat.R8_G8_B8_X8_UByte => nameof(PtxDdsPixelFormat.R8_G8_B8_X8_UByte),
                PtxDdsPixelFormat.R8_G8_B8_UByte => nameof(PtxDdsPixelFormat.R8_G8_B8_UByte),
                PtxDdsPixelFormat.B8_G8_R8_A8_UByte => nameof(PtxDdsPixelFormat.B8_G8_R8_A8_UByte),
                PtxDdsPixelFormat.B8_G8_R8_X8_UByte => nameof(PtxDdsPixelFormat.B8_G8_R8_X8_UByte),
                PtxDdsPixelFormat.B8_G8_R8_UByte => nameof(PtxDdsPixelFormat.B8_G8_R8_UByte),
                _ => nameof(PtxDdsPixelFormat.RGBA_BC3_UByte)
            };
        }

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
                        if (file != null && file.Path != null && IsPtxOrDds(file.Path))
                        {
                            string nativePtxPath = paths.GetFilePath(file.Path);
                            string nativePngPath = GetNativePngPath(unpackPath, file.Path, fileSystem);
                            string nativeMetaPath = GetNativeMetaPath(unpackPath, file.Path, fileSystem);
                            bool convert = true;
                            PtxPakMetadata? meta = null;
                            if (fileSystem.FileExists(nativeMetaPath))
                            {
                                meta = WindJsonSerializer.TryDeserializeFromFile<PtxPakMetadata>(nativeMetaPath, fileSystem, new NullLogger());
                                if (meta != null && meta.PngModifyTimeUtc == fileSystem.GetModifyTimeUtc(nativePngPath) && meta.Format == meta.LastPtxFormat && fileSystem.FileExists(nativePtxPath))
                                {
                                    convert = false;
                                }
                            }
                            if (convert)
                            {
                                meta ??= new PtxPakMetadata { Format = GetStringFromFormat(DEFAULTFORMAT) };
                                logger.Log($"Create {file.Path} by format {meta.Format}");
                                PtxDdsCoder.Encode(nativePngPath, nativePtxPath, fileSystem, logger, GetFormatFromString(meta.Format));
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
                        if (file != null && file.Path != null && IsPtxOrDds(file.Path))
                        {
                            string nativePtxPath = paths.GetFilePath(file.Path);
                            string nativePngPath = GetNativePngPath(unpackPath, file.Path, fileSystem);
                            string nativeMetaPath = GetNativeMetaPath(unpackPath, file.Path, fileSystem);
                            PtxDdsPixelFormat format = PtxDdsCoder.Decode(nativePtxPath, nativePngPath, fileSystem, logger);
                            string formatStr = GetStringFromFormat(format);
                            PtxPakMetadata meta = new PtxPakMetadata
                            {
                                Format = formatStr,
                                PngModifyTimeUtc = fileSystem.GetModifyTimeUtc(nativePngPath),
                                LastPtxFormat = formatStr
                            };
                            WindJsonSerializer.TrySerializeToFile(nativeMetaPath, meta, fileSystem, logger);
                        }
                    }
                }
            }
        }

        private static bool IsPtxOrDds(string path)
        {
            return path.EndsWith(".ptx", StringComparison.CurrentCultureIgnoreCase) || path.EndsWith(".dds", StringComparison.CurrentCultureIgnoreCase);
        }

        private static string GetNativePngPath(string unpackPath, string recordPath, IFileSystem fileSystem)
        {
            return fileSystem.Combine(unpackPath, "ptx_dds_raw_image", fileSystem.ChangeExtension(recordPath, ".png"));
        }

        private static string GetNativeMetaPath(string unpackPath, string recordPath, IFileSystem fileSystem)
        {
            return fileSystem.Combine(unpackPath, "ptx_dds_raw_image", recordPath + ".meta.json");
        }
    }
}
