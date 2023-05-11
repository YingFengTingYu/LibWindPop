﻿using System;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    internal class RsgMetadata
    {
        public uint Version { get; set; }

        public uint MinorVersion { get; set; }

        public bool UseBigEndian { get; set; }

        public string? ImageHandler { get; set; }

        public uint CompressionFlags { get; set; }

        public uint HeaderSize { get; set; }

        public uint ResidentDataOffset { get; set; }

        public uint ResidentDataCompressedSize { get; set; }

        public uint ResidentDataUncompressedSize { get; set; }

        public uint GPUDataOffset { get; set; }

        public uint GPUDataCompressedSize { get; set; }

        public uint GPUDataUncompressedSize { get; set; }

        public RsgMetadataResidentFileInfo[]? ResidentFileList { get; set; }

        public RsgMetadataGPUFileInfo[]? GPUFileList { get; set; }
    }

    internal class RsgMetadataResidentFileInfo
    {
        public string? Path { get; set; }

        public DateTime ModifyTimeUtc { get; set; }
    }

    internal class RsgMetadataGPUFileInfo
    {
        public string? Path { get; set; }

        public bool InResMap { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public DateTime ModifyTimeUtc { get; set; }
    }
}