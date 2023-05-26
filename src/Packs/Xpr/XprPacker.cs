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
    public static class XprPacker
    {
        public static unsafe void Pack(string unpackPath, string xprPath, IFileSystem fileSystem, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(unpackPath, nameof(unpackPath));
            ArgumentNullException.ThrowIfNull(xprPath, nameof(xprPath));
            ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            Encoding encoding = EncodingType.iso_8859_1.GetEncoding();
            XprUnpackPathProvider paths = new XprUnpackPathProvider(unpackPath, fileSystem);

            logger.Log("Get pack info...");

            XprPackInfo? packInfo = WindJsonSerializer.TryDeserializeFromFile<XprPackInfo>(paths.InfoPackInfoPath, fileSystem, logger);
            if (packInfo == null)
            {
                logger.LogError("Pack info is null");
            }
            else if (packInfo.RecordFiles == null)
            {
                logger.LogError("Record paths is null");
            }
            else
            {
                XprPackFileInfo[] recordPaths = packInfo.RecordFiles;
                uint fileCount = (uint)packInfo.RecordFiles.Length;
                uint fileInfoSize = (uint)sizeof(XprFileInfo) * fileCount;
                uint fileNamePoolSize = 0u;
                for (uint i = 0u; i < fileCount; i++)
                {
                    fileNamePoolSize += (uint)encoding.GetByteCount(recordPaths[i].Path ?? string.Empty) + 1u;
                }
                uint headerSize = 4u + fileInfoSize + 4u + fileNamePoolSize;
                if ((headerSize & 0xFu) != 0u)
                {
                    headerSize |= 0xFu;
                    headerSize++;
                }
                using (NativeMemoryOwner owner = new NativeMemoryOwner(headerSize))
                {
                    owner.Clear();
                    nuint xprDataPtrNum = (nuint)owner.Pointer;
                    using (Stream xprStream = fileSystem.Create(xprPath))
                    {
                        xprStream.SetLength(headerSize + packInfo.XprDataOffset);
                        xprStream.Seek(headerSize + packInfo.XprDataOffset, SeekOrigin.Begin);
                        uint fileNamePoolOffset = 4u + fileInfoSize + 4u;
                        XprFileInfo* fileInfoPtr = (XprFileInfo*)(xprDataPtrNum + 4u);
                        bool reverse = BitConverter.IsLittleEndian;
                        *(uint*)xprDataPtrNum = reverse ? BinaryPrimitives.ReverseEndianness(fileCount) : fileCount;
                        *(uint*)(xprDataPtrNum + 4u + fileInfoSize) = 0u;
                        for (uint i = 0u; i < fileCount; i++)
                        {
                            if (packInfo.XprDataFileAlign)
                            {
                                if (((xprStream.Length - packInfo.XprDataOffset) & 0xF) != 0)
                                {
                                    long newLength = xprStream.Length - packInfo.XprDataOffset;
                                    newLength |= 0xF;
                                    newLength++;
                                    newLength += packInfo.XprDataOffset;
                                    xprStream.SetLength(newLength);
                                    xprStream.Seek(newLength, SeekOrigin.Begin);
                                }
                            }
                            string nativePath = paths.GetFilePath(recordPaths[i].Path ?? string.Empty);
                            using (Stream fileStream = fileSystem.OpenRead(nativePath))
                            {
                                if (reverse)
                                {
                                    fileInfoPtr->Type = BinaryPrimitives.ReverseEndianness(UInt32StringConvertor.StringToUInt32(recordPaths[i].Type));
                                    fileInfoPtr->FileOffset = BinaryPrimitives.ReverseEndianness((uint)(xprStream.Length - packInfo.XprDataOffset));
                                    fileInfoPtr->FileSize = BinaryPrimitives.ReverseEndianness((uint)fileStream.Length);
                                    fileInfoPtr->PathOffset = BinaryPrimitives.ReverseEndianness(fileNamePoolOffset);
                                }
                                else
                                {
                                    fileInfoPtr->Type = UInt32StringConvertor.StringToUInt32(recordPaths[i].Type);
                                    fileInfoPtr->FileOffset = (uint)(xprStream.Length - packInfo.XprDataOffset);
                                    fileInfoPtr->FileSize = (uint)fileStream.Length;
                                    fileInfoPtr->PathOffset = fileNamePoolOffset;
                                }
                                fileStream.CopyTo(xprStream);
                            }
                            int strSize = UnsafeStringHelper.SetUtf16String(recordPaths[i].Path ?? string.Empty, xprDataPtrNum + fileNamePoolOffset, encoding);
                            fileNamePoolOffset += (uint)strSize;
                            *(byte*)(xprDataPtrNum + fileNamePoolOffset) = 0;
                            fileNamePoolOffset++;
                            fileInfoPtr++;
                        }
                        if (((xprStream.Length - packInfo.XprDataOffset) & 0x7FF) != 0)
                        {
                            long newLength = xprStream.Length - packInfo.XprDataOffset;
                            newLength |= 0x7FF;
                            newLength++;
                            newLength += packInfo.XprDataOffset;
                            xprStream.SetLength(newLength);
                        }
                        Span<byte> buffer = stackalloc byte[12];
                        BinaryPrimitives.WriteUInt32BigEndian(buffer, 0x58505232u);
                        BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(4, 4), (uint)(xprStream.Length - packInfo.XprDataOffset));
                        BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(8, 4), 0u);
                        xprStream.Seek(0, SeekOrigin.Begin);
                        xprStream.Write(buffer);
                        xprStream.Seek(packInfo.XprDataOffset, SeekOrigin.Begin);
                        xprStream.Write(xprDataPtrNum, headerSize);
                    }
                }
            }
        }
    }
}
