using LibWindPop.Packs.Common;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.FormatProvider;
using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using LibWindPop.Utils;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using LibWindPop.Utils.Atlas;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public sealed class AtlasCreator : IContentPipeline
    {
        private record struct AtlasPacker(string path, AtlasMetadata meta);
        private record struct SubImagePacker(List<AtlasSubImageInfo> subInfos, Dictionary<string, RsbManifestResourceInfo> manifestSubInfos);
        private record struct AtlasPacker2(string path, AtlasMetadata meta, Dictionary<string, RsbManifestResourceInfo> subImageMap);
        private const int ATLAS_ISATLAS = 0b10;
        private const int ATLAS_ISSUBIMAGE = 0b100;

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

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read rsb pack info...");

            RsbPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<RsbPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            logger.Log("Read rsb manifest info...");

            RsbManifestInfo? manifestInfo = WindJsonSerializer.TryDeserializeFromFile<RsbManifestInfo>(paths.InfoManifestPath, fileSystem, logger);

            if (packInfo == null || packInfo.Groups == null || manifestInfo == null || manifestInfo.CompositeGroups == null)
            {
                logger.LogError("Pack or manifest info is null");
            }
            else
            {
                if (packInfo.UseGroupFolder)
                {
                    logger.LogError("Can not use group folder for atlas creator");
                }
                Dictionary<string, AtlasPacker2> atlases = new Dictionary<string, AtlasPacker2>();
                Dictionary<string, SubImagePacker> subimages = new Dictionary<string, SubImagePacker>();
                RsbManifestCompositeInfo?[] composites = manifestInfo.CompositeGroups;
                for (int i = 0; i < composites.Length; i++)
                {
                    RsbManifestCompositeInfo? composite = composites[i];
                    if (composite != null && composite.SubGroups != null)
                    {
                        RsbManifestGroupInfo?[]? groups = composite.SubGroups;
                        for (int j = 0; j < groups.Length; j++)
                        {
                            RsbManifestGroupInfo? group = groups[j];
                            if (group != null && group.Resources != null)
                            {
                                RsbManifestResourceInfo?[]? resources = group.Resources;
                                for (int k = 0; k < resources.Length; k++)
                                {
                                    RsbManifestResourceInfo? resource = resources[k];
                                    if (resource != null)
                                    {
                                        if (resource.Type == 0 && !string.IsNullOrEmpty(resource.Id) && !string.IsNullOrEmpty(resource.Path) && resource.ImageProperties != null)
                                        {
                                            if ((resource.ImageProperties.Atlas & ATLAS_ISATLAS) != 0)
                                            {
                                                AtlasMetadata meta = new AtlasMetadata
                                                {
                                                    Id = resource.Id,
                                                    AFlags = resource.ImageProperties.AFlags,
                                                    Width = resource.ImageProperties.AW,
                                                    Height = resource.ImageProperties.AH
                                                };
                                                AtlasPacker2 metaPacker = new AtlasPacker2($"{resource.Path}.PNG".ToUpper(), meta, new Dictionary<string, RsbManifestResourceInfo>());
                                                atlases.Add(resource.Id, metaPacker);
                                            }
                                            if ((resource.ImageProperties.Atlas & ATLAS_ISSUBIMAGE) != 0 && !string.IsNullOrEmpty(resource.ImageProperties.Parent))
                                            {
                                                string thisPath = $"{resource.Path}.PNG".ToUpper();
                                                string thisNativePath = paths.GetResourcePathByGroupIdAndPath(null, thisPath);
                                                int aw, ah;
                                                using (Stream tempStream = fileSystem.OpenRead(thisNativePath))
                                                {
                                                    ImageCoder.PeekImageInfo(tempStream, out aw, out ah, out _);
                                                }
                                                AtlasSubImageInfo subimage = new AtlasSubImageInfo
                                                {
                                                    Id = resource.Id,
                                                    Path = thisPath,
                                                    AX = 0,
                                                    AY = 0,
                                                    AW = (uint)aw,
                                                    AH = (uint)ah,
                                                    PngModifyTimeUtc = fileSystem.GetModifyTimeUtc(thisNativePath)
                                                };
                                                string parentId = resource.ImageProperties.Parent;
                                                if (!parentId.Contains('|'))
                                                {
                                                    if (group.Res != 0)
                                                    {
                                                        parentId = $"{parentId}|{group.Res}";
                                                    }
                                                    if (group.Loc != null)
                                                    {
                                                        uint loc = UInt32StringConvertor.StringToUInt32(group.Loc);
                                                        parentId = $"{parentId}||{loc:x8}";
                                                    }
                                                }
                                                if (!subimages.ContainsKey(parentId))
                                                {
                                                    SubImagePacker packer = new SubImagePacker(new List<AtlasSubImageInfo>(), new Dictionary<string, RsbManifestResourceInfo>());
                                                    subimages.Add(parentId, packer);
                                                }
                                                subimages[parentId].subInfos.Add(subimage);
                                                subimages[parentId].manifestSubInfos.Add(resource.Id, resource);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<string, SubImagePacker> subimagePair in subimages)
                {
                    if (atlases.TryGetValue(subimagePair.Key, out AtlasPacker2 packer))
                    {
                        packer.meta.SubImages = subimagePair.Value.subInfos;
                        foreach (KeyValuePair<string, RsbManifestResourceInfo> pair in subimagePair.Value.manifestSubInfos)
                        {
                            packer.subImageMap.Add(pair.Key, pair.Value);
                        }
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                foreach (KeyValuePair<string, AtlasPacker2> atlasPair in atlases)
                {
                    bool create = true;
                    string rawPath = paths.GetResourcePathByGroupIdAndPath(null, atlasPair.Value.path);
                    AtlasMetadata? atlasMeta = WindJsonSerializer.TryDeserializeFromFile<AtlasMetadata>(paths.AppendMetaExtension(rawPath), fileSystem, new NullLogger(false));
                    if (atlasMeta != null)
                    {
                        AtlasMetadata allocNewMeta = atlasPair.Value.meta;
                        if (atlasMeta.Id == allocNewMeta.Id && atlasMeta.AFlags == allocNewMeta.AFlags && atlasMeta.Width == allocNewMeta.Width && atlasMeta.Height == allocNewMeta.Height)
                        {
                            if (atlasMeta.SubImages == null && allocNewMeta.SubImages == null)
                            {
                                create = false;
                            }
                            else if (atlasMeta.SubImages != null && allocNewMeta.SubImages != null && atlasMeta.SubImages.Count == allocNewMeta.SubImages.Count)
                            {
                                create = false;
                                int subimageCount = atlasMeta.SubImages.Count;
                                for (int i = 0; i < subimageCount; i++)
                                {
                                    if (atlasMeta.SubImages[i].Id != allocNewMeta.SubImages[i].Id || atlasMeta.SubImages[i].Path != allocNewMeta.SubImages[i].Path || atlasMeta.SubImages[i].AW != allocNewMeta.SubImages[i].AW || atlasMeta.SubImages[i].AH != allocNewMeta.SubImages[i].AH || (atlasMeta.SubImages[i].PngModifyTimeUtc != allocNewMeta.SubImages[i].PngModifyTimeUtc))
                                    {
                                        create = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (create)
                    {
                        logger.Log($"Create atlas {atlasPair.Value.path}...");
                        atlasMeta = atlasPair.Value.meta;
                        if (atlasMeta.SubImages != null)
                        {
                            List<AtlasSubImageInfo>? thisSubImages = atlasMeta.SubImages;
                            // compute atlas info
                            List<SubImageInfo> subImageInfoList = new List<SubImageInfo>();
                            foreach (AtlasSubImageInfo subInfo in atlasMeta.SubImages)
                            {
                                if (subInfo.Id != null)
                                {
                                    subImageInfoList.Add(new SubImageInfo((int)subInfo.AW, (int)subInfo.AH, subInfo.Id));
                                }
                            }
                            subImageInfoList.Sort(static (SubImageInfo b, SubImageInfo a) => a.Width * a.Height - b.Width * b.Height);
                            Dictionary<string, SubImageInfo> idSubImageInfoMap = new Dictionary<string, SubImageInfo>();
                            MaxRectsBinPack maxRectsBinPack = new MaxRectsBinPack((int)atlasMeta.Width, (int)atlasMeta.Height, false);
                            foreach (SubImageInfo subImageInfo in subImageInfoList)
                            {
                                string? id = subImageInfo.ID;
                                if (id != null)
                                {
                                    subImageInfo.SetPos(maxRectsBinPack.Insert(subImageInfo, MaxRectsBinPack.FreeRectChoiceHeuristic.RectBestAreaFit));
                                    idSubImageInfoMap.Add(id, subImageInfo);
                                    if (atlasPair.Value.subImageMap.TryGetValue(id, out RsbManifestResourceInfo? manifestResInfo) && manifestResInfo != null)
                                    {
                                        if (manifestResInfo.ImageProperties != null)
                                        {
                                            manifestResInfo.ImageProperties.AX = (uint)subImageInfo.X;
                                            manifestResInfo.ImageProperties.AY = (uint)subImageInfo.Y;
                                            manifestResInfo.ImageProperties.AW = (uint)subImageInfo.Width;
                                            manifestResInfo.ImageProperties.AH = (uint)subImageInfo.Height;
                                        }
                                    }
                                    foreach (AtlasSubImageInfo subInfo in atlasMeta.SubImages)
                                    {
                                        if (subInfo.Id == id)
                                        {
                                            subInfo.AX = (uint)subImageInfo.X;
                                            subInfo.AY = (uint)subImageInfo.Y;
                                            subInfo.AW = (uint)subImageInfo.Width;
                                            subInfo.AH = (uint)subImageInfo.Height;
                                            break;
                                        }
                                    }
                                }
                            }
                            using (NativeBitmap atlasBitmap = new NativeBitmap((int)atlasMeta.Width, (int)atlasMeta.Height))
                            {
                                RefBitmap atlasBitmapRef = atlasBitmap.AsRefBitmap();
                                atlasBitmapRef.Data.Clear();
                                foreach (AtlasSubImageInfo subimage in atlasMeta.SubImages)
                                {
                                    if (subimage.Path != null)
                                    {
                                        string imageRealPath = paths.GetResourcePathByGroupIdAndPath(null, subimage.Path);
                                        using (Stream subImageStream = fileSystem.OpenRead(imageRealPath))
                                        {
                                            ImageCoder.PeekImageInfo(subImageStream, out int aw, out int ah, out ImageFormat format);
                                            using (NativeBitmap subImageBitmap = new NativeBitmap(aw, ah))
                                            {
                                                RefBitmap subImageBitmapRef = subImageBitmap.AsRefBitmap();
                                                ImageCoder.DecodeImage(subImageStream, subImageBitmapRef, format);
                                                for (int y = 0; y < subImageBitmapRef.Height; y++)
                                                {
                                                    int ay = y + (int)subimage.AY;
                                                    if (ay >= 0 && ay < atlasBitmapRef.Height)
                                                    {
                                                        Span<YFColor> atlasBitmapLine = atlasBitmapRef[ay];
                                                        int ax = (int)subimage.AX;
                                                        if (ax >= 0 && ax < atlasBitmapLine.Length)
                                                        {
                                                            atlasBitmapLine = atlasBitmapLine[ax..];
                                                            Span<YFColor> subImageBitmapLine = subImageBitmapRef[y];
                                                            int minLen = Math.Min(atlasBitmapLine.Length, subImageBitmapLine.Length);
                                                            subImageBitmapLine[..minLen].CopyTo(atlasBitmapLine[..minLen]);
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                using (Stream atlasImageStream = fileSystem.Create(rawPath))
                                {
                                    ImageCoder.EncodeImage(atlasImageStream, atlasBitmapRef, ImageFormat.Png, null);
                                }
                            }
                        }
                        WindJsonSerializer.TrySerializeToFile(paths.AppendMetaExtension(rawPath), atlasMeta, fileSystem, logger);
                    }
                }
                WindJsonSerializer.TrySerializeToFile(paths.InfoManifestPath, manifestInfo, fileSystem, logger);
            }
        }

        private static unsafe void AddInternal(string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            logger.Log("Get pack info...");

            // define base path
            RsbUnpackPathProvider paths = new RsbUnpackPathProvider(unpackPath, fileSystem, false);

            logger.Log("Read rsb pack info...");

            RsbPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<RsbPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);

            logger.Log("Read rsb manifest info...");

            RsbManifestInfo? manifestInfo = WindJsonSerializer.TryDeserializeFromFile<RsbManifestInfo>(paths.InfoManifestPath, fileSystem, logger);

            if (packInfo == null || packInfo.Groups == null || manifestInfo == null || manifestInfo.CompositeGroups == null)
            {
                logger.LogError("Pack or manifest info is null");
            }
            else
            {
                if (packInfo.UseGroupFolder)
                {
                    logger.LogError("Can not use group folder for atlas creator");
                }
                Dictionary<string, AtlasPacker> atlases = new Dictionary<string, AtlasPacker>();
                Dictionary<string, List<AtlasSubImageInfo>> subimages = new Dictionary<string, List<AtlasSubImageInfo>>();
                RsbManifestCompositeInfo?[] composites = manifestInfo.CompositeGroups;
                for (int i = 0; i < composites.Length; i++)
                {
                    RsbManifestCompositeInfo? composite = composites[i];
                    if (composite != null && composite.SubGroups != null)
                    {
                        RsbManifestGroupInfo?[]? groups = composite.SubGroups;
                        for (int j = 0; j < groups.Length; j++)
                        {
                            RsbManifestGroupInfo? group = groups[j];
                            if (group != null && group.Resources != null)
                            {
                                RsbManifestResourceInfo?[]? resources = group.Resources;
                                for (int k = 0; k < resources.Length; k++)
                                {
                                    RsbManifestResourceInfo? resource = resources[k];
                                    if (resource != null)
                                    {
                                        if (resource.Type == 0 && !string.IsNullOrEmpty(resource.Id) && !string.IsNullOrEmpty(resource.Path) && resource.ImageProperties != null)
                                        {
                                            if ((resource.ImageProperties.Atlas & ATLAS_ISATLAS) != 0)
                                            {
                                                AtlasMetadata meta = new AtlasMetadata
                                                {
                                                    Id = resource.Id,
                                                    AFlags = resource.ImageProperties.AFlags,
                                                    Width = resource.ImageProperties.AW,
                                                    Height = resource.ImageProperties.AH
                                                };
                                                AtlasPacker metaPacker = new AtlasPacker($"{resource.Path}.PNG".ToUpper(), meta);
                                                atlases.Add(resource.Id, metaPacker);
                                            }
                                            if ((resource.ImageProperties.Atlas & ATLAS_ISSUBIMAGE) != 0 && !string.IsNullOrEmpty(resource.ImageProperties.Parent))
                                            {
                                                AtlasSubImageInfo subimage = new AtlasSubImageInfo
                                                {
                                                    Id = resource.Id,
                                                    Path = $"{resource.Path}.PNG".ToUpper(),
                                                    AX = resource.ImageProperties.AX,
                                                    AY = resource.ImageProperties.AY,
                                                    AW = resource.ImageProperties.AW,
                                                    AH = resource.ImageProperties.AH
                                                };
                                                string parentId = resource.ImageProperties.Parent;
                                                if (!parentId.Contains('|'))
                                                {
                                                    if (group.Res != 0)
                                                    {
                                                        parentId = $"{parentId}|{group.Res}";
                                                    }
                                                    if (group.Loc != null)
                                                    {
                                                        uint loc = UInt32StringConvertor.StringToUInt32(group.Loc);
                                                        parentId = $"{parentId}||{loc:x8}";
                                                    }
                                                }
                                                if (!subimages.ContainsKey(parentId))
                                                {
                                                    subimages.Add(parentId, new List<AtlasSubImageInfo>());
                                                }
                                                subimages[parentId].Add(subimage);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<string, List<AtlasSubImageInfo>> subimagePair in subimages)
                {
                    if (atlases.TryGetValue(subimagePair.Key, out AtlasPacker packer))
                    {
                        packer.meta.SubImages = subimagePair.Value;
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
                foreach (KeyValuePair<string, AtlasPacker> atlasPair in atlases)
                {
                    logger.Log($"Cut atlas {atlasPair.Value.path}...");
                    List<AtlasSubImageInfo>? thisSubImages = atlasPair.Value.meta.SubImages;
                    string rawPath = paths.GetResourcePathByGroupIdAndPath(null, atlasPair.Value.path);
                    if (thisSubImages != null)
                    {
                        // Open Image
                        using (Stream atlasImageStream = fileSystem.OpenRead(rawPath))
                        {
                            ImageCoder.PeekImageInfo(atlasImageStream, out int width, out int height, out ImageFormat format);
                            using (NativeBitmap atlasBitmap = new NativeBitmap(width, height))
                            {
                                RefBitmap atlasBitmapRef = atlasBitmap.AsRefBitmap();
                                ImageCoder.DecodeImage(atlasImageStream, atlasBitmapRef, format);
                                // cut image
                                foreach (AtlasSubImageInfo subimage in thisSubImages)
                                {
                                    if (subimage.Path != null)
                                    {
                                        using (NativeBitmap subImageBitmap = new NativeBitmap((int)subimage.AW, (int)subimage.AH))
                                        {
                                            RefBitmap subImageBitmapRef = subImageBitmap.AsRefBitmap();
                                            subImageBitmapRef.Data.Clear();
                                            for (int y = 0; y < subImageBitmapRef.Height; y++)
                                            {
                                                int ay = y + (int)subimage.AY;
                                                if (ay >= 0 && ay < atlasBitmapRef.Height)
                                                {
                                                    Span<YFColor> atlasBitmapLine = atlasBitmapRef[ay];
                                                    int ax = (int)subimage.AX;
                                                    if (ax >= 0 && ax < atlasBitmapLine.Length)
                                                    {
                                                        atlasBitmapLine = atlasBitmapLine[ax..];
                                                        Span<YFColor> subImageBitmapLine = subImageBitmapRef[y];
                                                        int minLen = Math.Min(atlasBitmapLine.Length, subImageBitmapLine.Length);
                                                        atlasBitmapLine[..minLen].CopyTo(subImageBitmapLine[..minLen]);
                                                    }
                                                }
                                            }
                                            string imageRealPath = paths.GetResourcePathByGroupIdAndPath(null, subimage.Path);
                                            using (Stream subImageStream = fileSystem.Create(imageRealPath))
                                            {
                                                ImageCoder.EncodeImage(subImageStream, subImageBitmapRef, ImageFormat.Png, null);
                                            }
                                            subimage.PngModifyTimeUtc = fileSystem.GetModifyTimeUtc(imageRealPath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    WindJsonSerializer.TrySerializeToFile(paths.AppendMetaExtension(rawPath), atlasPair.Value.meta, fileSystem, logger);
                }
            }
        }
    }
}
