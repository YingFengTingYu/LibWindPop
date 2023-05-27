using LibWindPop.Packs.Common;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Collections.Generic;
using LibWindPop.Images.PtxPS3;
using LibWindPop.Utils.Json;

namespace LibWindPop.Packs.Pak.ContentPipeline
{
    public sealed class PakPtxPS3AutoEncoder : IContentPipeline
    {
        private const PtxPS3PixelFormat DEFAULTFORMAT = PtxPS3PixelFormat.RGBA_BC3_UByte;

        private static PtxPS3PixelFormat GetFormatFromString(string? str)
        {
            return str switch
            {
                nameof(PtxPS3PixelFormat.RGBA_BC1_UByte) => PtxPS3PixelFormat.RGBA_BC1_UByte,
                nameof(PtxPS3PixelFormat.RGBA_BC2_UByte) => PtxPS3PixelFormat.RGBA_BC2_UByte,
                nameof(PtxPS3PixelFormat.RGBA_BC3_UByte) => PtxPS3PixelFormat.RGBA_BC3_UByte,
                nameof(PtxPS3PixelFormat.L8_UByte) => PtxPS3PixelFormat.L8_UByte,
                nameof(PtxPS3PixelFormat.R8_G8_B8_A8_UByte) => PtxPS3PixelFormat.R8_G8_B8_A8_UByte,
                nameof(PtxPS3PixelFormat.R8_G8_B8_X8_UByte) => PtxPS3PixelFormat.R8_G8_B8_X8_UByte,
                nameof(PtxPS3PixelFormat.R8_G8_B8_UByte) => PtxPS3PixelFormat.R8_G8_B8_UByte,
                _ => PtxPS3PixelFormat.RGBA_BC3_UByte
            };
        }

        private static string GetStringFromFormat(PtxPS3PixelFormat format)
        {
            return format switch
            {
                PtxPS3PixelFormat.RGBA_BC1_UByte => nameof(PtxPS3PixelFormat.RGBA_BC1_UByte),
                PtxPS3PixelFormat.RGBA_BC2_UByte => nameof(PtxPS3PixelFormat.RGBA_BC2_UByte),
                PtxPS3PixelFormat.RGBA_BC3_UByte => nameof(PtxPS3PixelFormat.RGBA_BC3_UByte),
                PtxPS3PixelFormat.L8_UByte => nameof(PtxPS3PixelFormat.L8_UByte),
                PtxPS3PixelFormat.R8_G8_B8_A8_UByte => nameof(PtxPS3PixelFormat.R8_G8_B8_A8_UByte),
                PtxPS3PixelFormat.R8_G8_B8_X8_UByte => nameof(PtxPS3PixelFormat.R8_G8_B8_X8_UByte),
                PtxPS3PixelFormat.R8_G8_B8_UByte => nameof(PtxPS3PixelFormat.R8_G8_B8_UByte),
                _ => nameof(PtxPS3PixelFormat.RGBA_BC3_UByte)
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
                        if (file != null && file.Path != null && file.Path.EndsWith(".ptx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string nativePtxPath = paths.GetFilePath(file.Path);
                            string nativePngPath = GetNativePngPath(unpackPath, file.Path, fileSystem);
                            string nativeMetaPath = GetNativeMetaPath(unpackPath, file.Path, fileSystem);
                            bool convert = true;
                            PtxPS3Metadata? meta = null;
                            if (fileSystem.FileExists(nativeMetaPath))
                            {
                                meta = WindJsonSerializer.TryDeserializeFromFile<PtxPS3Metadata>(nativeMetaPath, fileSystem, new NullLogger());
                                if (meta != null && meta.PngModifyTimeUtc == fileSystem.GetModifyTimeUtc(nativePngPath) && meta.Format == meta.LastPtxFormat)
                                {
                                    convert = false;
                                }
                            }
                            if (convert)
                            {
                                meta ??= new PtxPS3Metadata { Format = GetStringFromFormat(DEFAULTFORMAT) };
                                logger.Log($"Create {file.Path} by format {meta.Format}");
                                PtxPS3Coder.Encode(nativePngPath, nativePtxPath, fileSystem, logger, GetFormatFromString(meta.Format));
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
                        if (file != null && file.Path != null && file.Path.EndsWith(".ptx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string nativePtxPath = paths.GetFilePath(file.Path);
                            string nativePngPath = GetNativePngPath(unpackPath, file.Path, fileSystem);
                            string nativeMetaPath = GetNativeMetaPath(unpackPath, file.Path, fileSystem);
                            PtxPS3PixelFormat format = PtxPS3Coder.Decode(nativePtxPath, nativePngPath, fileSystem, logger);
                            string formatStr = GetStringFromFormat(format);
                            PtxPS3Metadata meta = new PtxPS3Metadata
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

        private static string GetNativePngPath(string unpackPath, string recordPath, IFileSystem fileSystem)
        {
            return fileSystem.Combine(unpackPath, "ptx_ps3_raw_image", fileSystem.ChangeExtension(recordPath, ".png"));
        }

        private static string GetNativeMetaPath(string unpackPath, string recordPath, IFileSystem fileSystem)
        {
            return fileSystem.Combine(unpackPath, "ptx_ps3_raw_image", recordPath + ".meta.json");
        }
    }
}
