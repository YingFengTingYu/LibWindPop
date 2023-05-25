using LibWindPop.Utils.Extension;
using LibWindPop.Utils.Graphics.Bitmap;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    /// <summary>
    /// https://www.w3.org/TR/png/
    /// </summary>
    internal static class PngEncoder
    {
        public static void EncodePng(Stream stream, RefBitmap bitmap, IImageEncoderArgument? args)
        {
            WritePngMagic(stream);
            WriteIHDRChunk(stream, new IHDRInfo((uint)bitmap.Width, (uint)bitmap.Height, 8, PngColorType.TruecolourWithAlpha, 0, 0, 0));
            WritetEXtChunk(stream, "Software", "WindLib by YingFengTingYu");
            WriteIDATChunk(stream, bitmap, args is PngEncoderArgument pngArgs ? pngArgs.ZlibLevel : 1);
            WriteIENDChunk(stream);
        }

        private static void WritetEXtChunk(Stream stream, string key, string value)
        {
            using (PngChunkWriteOnlyStream chunkStream = new PngChunkWriteOnlyStream(stream, PngConstants.Chunk_tEXt))
            {
                Encoding encoding = EncodingType.iso_8859_1.GetEncoding();
                int keyCount = encoding.GetByteCount(key);
                int bufferLen = keyCount + encoding.GetByteCount(value) + 1;
                using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(bufferLen))
                {
                    Span<byte> buffer = owner.Memory.Span[..bufferLen];
                    encoding.GetBytes(key, buffer[..keyCount]);
                    buffer[keyCount] = 0;
                    encoding.GetBytes(value, buffer[(keyCount + 1)..]);
                    chunkStream.Write(buffer);
                }
            }
        }

        private static void WriteIDATChunk(Stream stream, RefBitmap bitmap, int level)
        {
            using (PngChunkWriteOnlyZlibStream deflateChunkStream = new PngChunkWriteOnlyZlibStream(stream, PngConstants.Chunk_IDAT, 0xFFFF, level))
            {
                int rowLen = 1 + (bitmap.Width << 2);
                // DeflateStream only support array
                byte[]? poolArray = null;
                try
                {
                    poolArray = ArrayPool<byte>.Shared.Rent(rowLen);
                    unsafe
                    {
                        fixed (byte* ptr = &poolArray[0])
                        {
                            byte* span_ptr;
                            Span<YFColor> row;
                            YFColor color;
                            for (int y = 0; y < bitmap.Height; y++)
                            {
                                row = bitmap[y];
                                span_ptr = ptr;
                                *span_ptr++ = 0;
                                for (int x = 0; x < bitmap.Width; x++)
                                {
                                    color = row[x];
                                    *span_ptr++ = color.Red;
                                    *span_ptr++ = color.Green;
                                    *span_ptr++ = color.Blue;
                                    *span_ptr++ = color.Alpha;
                                }
                                deflateChunkStream.Write(poolArray, 0, rowLen);
                            }
                        }
                    }
                }
                finally
                {
                    if (poolArray != null)
                    {
                        ArrayPool<byte>.Shared.Return(poolArray);
                    }
                }
            }
        }

        private static void WriteIHDRChunk(Stream stream, IHDRInfo info)
        {
            using (PngChunkWriteOnlyStream chunkStream = new PngChunkWriteOnlyStream(stream, PngConstants.Chunk_IHDR))
            {
                Span<byte> chunk_data = stackalloc byte[13];
                BinaryPrimitives.WriteUInt32BigEndian(chunk_data[..0x4], info.width);
                BinaryPrimitives.WriteUInt32BigEndian(chunk_data.Slice(0x4, 0x4), info.height);
                chunk_data[0x8] = info.bitDepth;
                chunk_data[0x9] = (byte)info.colorType;
                chunk_data[0xA] = info.compressionMethod;
                chunk_data[0xB] = info.filterMethod;
                chunk_data[0xC] = info.interlaceMethod;
                chunkStream.Write(chunk_data);
            }
        }

        private static void WriteIENDChunk(Stream stream)
        {
            using (PngChunkWriteOnlyStream chunkStream = new PngChunkWriteOnlyStream(stream, PngConstants.Chunk_IEND))
            {

            }
        }

        private static void WritePngMagic(Stream stream)
        {
            stream.Write(PngConstants.Magic);
        }
    }
}
