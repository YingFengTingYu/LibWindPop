using LibWindPop.Packs.Xpr.XprStructures;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Json;
using LibWindPop.Utils.Logger;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace LibWindPop.Packs.Xpr
{
    public static class XprUnpacker
    {
        public static unsafe void Unpack(string xprPath, string unpackPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(xprPath, nameof(xprPath));
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            Encoding encoding = EncodingType.iso_8859_1.GetEncoding();
            XprUnpackPathProvider paths = new XprUnpackPathProvider(unpackPath, fileSystem);

            logger.Log($"Read xpr info...");
            XprPackInfo packInfo = new XprPackInfo();
            XprPackFileInfo[] pathArray;
            using (Stream xprStream = fileSystem.OpenRead(xprPath))
            {
                Span<byte> buffer = stackalloc byte[12];
                xprStream.Read(buffer);
                uint magic = BinaryPrimitives.ReadUInt32BigEndian(buffer);
                if (magic != 0x58505232u) // XPR2
                {
                    logger.LogError($"Xpr magic mismatch: XPR2 expected but value is 0x{magic:X8}");
                }
                uint xprDataSize = BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(4, 4));
                uint textureDataSize = BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(8, 4));
                bool fileAlign = true;
                // Load to memory...
                using (NativeMemoryOwner owner = new NativeMemoryOwner(xprDataSize))
                {
                    bool reverse = BitConverter.IsLittleEndian;
                    nuint xprDataPtrNum = (nuint)owner.Pointer;
                    uint xprDataOffset = (uint)(xprStream.Length - xprDataSize);
                    packInfo.XprDataOffset = xprDataOffset;
                    xprStream.Seek(xprDataOffset, SeekOrigin.Begin);
                    xprStream.Read(xprDataPtrNum, xprDataSize);
                    uint fileCount = *(uint*)xprDataPtrNum;
                    if (reverse)
                    {
                        fileCount = BinaryPrimitives.ReverseEndianness(fileCount);
                    }
                    pathArray = new XprPackFileInfo[fileCount];
                    XprFileInfo* fileInfoPtr = (XprFileInfo*)(xprDataPtrNum + 4);
                    logger.Log($"Extract files from xpr...");
                    for (uint i = 0u; i < fileCount; i++)
                    {
                        uint type = (reverse ? BinaryPrimitives.ReverseEndianness(fileInfoPtr->Type) : fileInfoPtr->Type);
                        string recordPath = UnsafeStringHelper.GetUtf16String(xprDataPtrNum + (reverse ? BinaryPrimitives.ReverseEndianness(fileInfoPtr->PathOffset) : fileInfoPtr->PathOffset), encoding);
                        string filePath = paths.GetFilePath(recordPath);
                        logger.Log($"Extract file {recordPath}...");
                        pathArray[i] = new XprPackFileInfo
                        {
                            Type = UInt32StringConvertor.UInt32ToString(type),
                            Path = recordPath,
                        };
                        uint fileOffset = reverse ? BinaryPrimitives.ReverseEndianness(fileInfoPtr->FileOffset) : fileInfoPtr->FileOffset;
                        if ((fileOffset & 0xFu) != 0u)
                        {
                            fileAlign = false;
                        }
                        try
                        {
                            using (Stream fileStream = fileSystem.Create(filePath))
                            {
                                fileStream.Write(xprDataPtrNum + fileOffset, reverse ? BinaryPrimitives.ReverseEndianness(fileInfoPtr->FileSize) : fileInfoPtr->FileSize);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);
                        }
                        fileInfoPtr++;
                    }
                    packInfo.XprDataFileAlign = fileAlign;
                    packInfo.RecordFiles = pathArray;
                }
            }
            WindJsonSerializer.TrySerializeToFile(paths.InfoPackInfoPath, packInfo, fileSystem, logger);
        }
    }
}
