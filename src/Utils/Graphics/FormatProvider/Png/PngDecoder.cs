using LibWindPop.Utils.Graphics.Bitmap;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal static class PngDecoder
    {
        public static bool PeekPngWidthHeight(Stream stream, out uint width, out uint height)
        {
            long pos = stream.Position;
            CheckPngMagic(stream);
            Span<byte> chunkTypeBuffer = stackalloc byte[4];
            IHDRInfo ihdr = new IHDRInfo();
            bool ans = false;
            while (PeekPngChunkType(stream, chunkTypeBuffer))
            {
                if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_IHDR))
                {
                    ihdr = ReadIHDRChunk(stream);
                    ans = true;
                    break;
                }
                else
                {
                    ReadUnknowChunk(stream);
                }
            }
            stream.Seek(pos, SeekOrigin.Begin);
            width = ihdr.width;
            height = ihdr.height;
            return ans;
        }

        public static void DecodePng(Stream stream, RefBitmap bitmap)
        {
            if (CheckPngMagic(stream))
            {
                Span<byte> chunkTypeBuffer = stackalloc byte[4];
                IHDRInfo? ihdr = null;
                Span<YFColor> palette = stackalloc YFColor[256];
                RedGreenBlueGrey64? tRns = null;
                bool CgBI = false;
                while (PeekPngChunkType(stream, chunkTypeBuffer))
                {
                    if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_IHDR))
                    {
                        ihdr = ReadIHDRChunk(stream);
                    }
                    else if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_CgBI))
                    {
                        CgBI = true;
                        ReadUnknowChunk(stream);
                    }
                    else
                    {
                        if (ihdr == null)
                        {
                            throw new Exception("Must have IHDR chunk at first");
                        }
                        if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_PLTE))
                        {
                            ReadPLTEChunk(stream, palette);
                        }
                        else if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_tRNS))
                        {
                            tRns = ReadtRNSChunk(stream, ihdr.Value.bitDepth, ihdr.Value.colorType, palette);
                        }
                        else if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_IDAT))
                        {
                            ReadIDATChunk(stream, bitmap, ihdr.Value, palette, tRns, CgBI);
                        }
                        else if (chunkTypeBuffer.SequenceEqual(PngConstants.Chunk_IEND))
                        {
                            ReadIENDChunk(stream);
                            break;
                        }
                        else
                        {
                            ReadUnknowChunk(stream);
                        }
                    }
                }
            }
        }

        private static RedGreenBlueGrey64? ReadtRNSChunk(Stream stream, byte bitDepth, PngColorType colorType, Span<YFColor> palette)
        {
            using (PngChunkReadOnlyStream chunkStream = new PngChunkReadOnlyStream(stream))
            {
                if (colorType == PngColorType.Greyscale)
                {
                    int mask = (1 << bitDepth) - 1;
                    Span<byte> buffer = stackalloc byte[2];
                    chunkStream.Read(buffer);
                    return new RedGreenBlueGrey64
                    {
                        Grey = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer) & mask)
                    };
                }
                else if (colorType == PngColorType.Truecolour)
                {
                    int mask = (1 << bitDepth) - 1;
                    Span<byte> buffer = stackalloc byte[6];
                    chunkStream.Read(buffer);
                    RedGreenBlueGrey64 ans = new RedGreenBlueGrey64
                    {
                        Red = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer[..2]) & mask),
                        Green = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(2, 2)) & mask),
                        Blue = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(4, 2)) & mask)
                    };
                }
                else if (colorType == PngColorType.Indexedcolour)
                {
                    Span<byte> buffer = stackalloc byte[(int)chunkStream.Length];
                    chunkStream.Read(buffer);
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        palette[i].Alpha = buffer[i];
                    }
                    for (int i = buffer.Length; i < palette.Length; i++)
                    {
                        palette[i].Alpha = 0xFF;
                    }
                }
                return null;
            }
        }

        private static unsafe void ReadIDATChunk(Stream stream, RefBitmap bitmap, IHDRInfo info, Span<YFColor> palette, RedGreenBlueGrey64? tRns, bool CgBI)
        {
            using (PngChunkReadOnlyZlibStream chunkStream = new PngChunkReadOnlyZlibStream(stream, uint.MaxValue, CgBI))
            {
                if (info.compressionMethod != 0)
                {
                    throw new Exception($"Unsupport compression method {info.compressionMethod}");
                }
                if (info.filterMethod != 0)
                {
                    throw new Exception($"Unsupport filter method {info.compressionMethod}");
                }
                if (info.interlaceMethod != 0 && info.interlaceMethod != 1)
                {
                    throw new Exception($"Unsupport interlace method {info.compressionMethod}");
                }
                int row_byte_size = (int)((info.width * info.bitDepth * GetChannelCount(info.colorType) + 7) >> 3);
                using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(1 + 2 * row_byte_size))
                {
                    Span<byte> memory_span = memoryOwner.Memory.Span;
                    if (info.interlaceMethod == 0)
                    {
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 0u, 0u, 1u, 1u, bitmap, palette, tRns);
                    }
                    else
                    {
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 0u, 0u, 8u, 8u, bitmap, palette, tRns);
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 4u, 0u, 8u, 8u, bitmap, palette, tRns);
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 0u, 4u, 4u, 8u, bitmap, palette, tRns);
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 2u, 0u, 4u, 4u, bitmap, palette, tRns);
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 0u, 2u, 2u, 4u, bitmap, palette, tRns);
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 1u, 0u, 2u, 2u, bitmap, palette, tRns);
                        FillBitmap(chunkStream, info.colorType, info.bitDepth, info.width, info.height, memory_span, 0u, 1u, 1u, 2u, bitmap, palette, tRns);
                    }
                }
                // handle CgBI
                if (CgBI)
                {
                    Span<YFColor> row;
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        row = bitmap[y];
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            row[x].SwapRedBlue();
                            row[x].PremultiplyAlpha();
                        }
                    }
                }
            }
        }

        private static void ReadPLTEChunk(Stream stream, Span<YFColor> palette)
        {
            using (PngChunkReadOnlyStream chunkStream = new PngChunkReadOnlyStream(stream))
            {
                int len = (int)chunkStream.Length;
                int palette_len = len / 3;
                if (palette_len * 3 != len || palette_len > 256)
                {
                    throw new Exception("Can not read PLTE chunk!");
                }
                Span<byte> buffer = stackalloc byte[len];
                chunkStream.Read(buffer);
                for (int i = 0; i < palette_len; i++)
                {
                    palette[i] = new YFColor(buffer[3 * i], buffer[3 * i + 1], buffer[3 * i + 2], 0xFF);
                }
            }
        }

        private static void ReadIENDChunk(Stream stream)
        {
            using (PngChunkReadOnlyStream chunkStream = new PngChunkReadOnlyStream(stream))
            {

            }
        }

        private static IHDRInfo ReadIHDRChunk(Stream stream)
        {
            using (PngChunkReadOnlyStream chunkStream = new PngChunkReadOnlyStream(stream))
            {
                Span<byte> buffer = stackalloc byte[13];
                chunkStream.Read(buffer);
                return new IHDRInfo(
                    BinaryPrimitives.ReadUInt32BigEndian(buffer[..0x4]),
                    BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(0x4, 0x4)),
                    buffer[0x8],
                    (PngColorType)buffer[0x9],
                    buffer[0xA],
                    buffer[0xB],
                    buffer[0xC]
                    );
            }
        }

        private static bool PeekPngChunkType(Stream stream, Span<byte> chunkType)
        {
            if (stream.Position >= stream.Length)
            {
                return false;
            }
            long pos = stream.Position;
            stream.Seek(0x4, SeekOrigin.Current);
            stream.Read(chunkType);
            stream.Seek(pos, SeekOrigin.Begin);
            return true;
        }

        private static void ReadUnknowChunk(Stream stream)
        {
            using (PngChunkReadOnlyStream chunkStream = new PngChunkReadOnlyStream(stream))
            {

            }
        }

        private static bool CheckPngMagic(Stream stream)
        {
            Span<byte> buffer = stackalloc byte[PngConstants.Magic.Length];
            stream.Read(buffer);
            return buffer.SequenceEqual(PngConstants.Magic);
        }

        public static bool IsPng(Stream stream)
        {
            long pos = stream.Position;
            Span<byte> buffer = stackalloc byte[PngConstants.Magic.Length];
            stream.Read(buffer);
            bool isPng = buffer.SequenceEqual(PngConstants.Magic);
            stream.Seek(pos, SeekOrigin.Begin);
            return isPng;
        }

        private static unsafe void FillBitmap(Stream chunkStream, PngColorType colorType, int bitDepth, uint width, uint height, Span<byte> data_span, uint startX, uint startY, uint deltaX, uint deltaY, RefBitmap bitmap, Span<YFColor> palette, RedGreenBlueGrey64? tRns)
        {
            int row_len = (int)(((width - startX + deltaX - 1) / deltaX * bitDepth * GetChannelCount(colorType) + 7) / 8);
            Span<byte> raw_row_data_span = data_span[..(row_len + 1)];
            ref byte row_fliter = ref data_span[0];
            Span<byte> fliter_span = data_span.Slice(1, row_len);
            Span<byte> backup_span = data_span.Slice(1 + row_len, row_len);
            backup_span.Fill(0x0);
            if (colorType == PngColorType.Indexedcolour)
            {
                if (bitDepth != 1 && bitDepth != 2 && bitDepth != 4 && bitDepth != 8)
                {
                    throw new Exception("Can not read IDAT chunk with color type Indexedcolour!");
                }
                int single_byte_index_count = 8 / bitDepth;
                byte buffer_byte;
                uint x;
                for (uint y = startY; y < height; y += deltaY)
                {
                    chunkStream.Read(raw_row_data_span);
                    GetFliterData(fliter_span, backup_span, row_fliter, 1, bitDepth);
                    x = startX;
                    Span<YFColor> bitmap_row_span = bitmap[(int)y];
                    for (int index = 0; index < row_len; index++)
                    {
                        buffer_byte = fliter_span[index];
                        for (int c = 0; c < single_byte_index_count; c++)
                        {
                            if (x < width)
                            {
                                bitmap_row_span[(int)x] = palette[buffer_byte >> (8 - bitDepth)];
                            }
                            buffer_byte = (byte)((buffer_byte << bitDepth) & 0xFF);
                            x += deltaX;
                        }
                    }
                    fliter_span.CopyTo(backup_span);
                }
            }
            else if (colorType == PngColorType.Truecolour)
            {
                if (bitDepth != 8 && bitDepth != 16)
                {
                    throw new Exception("Can not read IDAT chunk with color type Truecolour!");
                }
                uint x;
                if (bitDepth == 8)
                {
                    byte r, g, b, a;
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 3, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 3)
                        {
                            r = fliter_span[index];
                            g = fliter_span[index + 1];
                            b = fliter_span[index + 2];
                            a = (tRns != null && tRns.Value.EqualRGB(r, g, b)) ? (byte)0x0 : (byte)0xFF;
                            bitmap_row_span[(int)x] = new YFColor(r, g, b, a);
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
                else if (bitDepth == 16)
                {
                    ushort r, g, b;
                    byte a;
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 3, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 6)
                        {
                            r = BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index, 2));
                            g = BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index + 2, 2));
                            b = BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index + 4, 2));
                            a = (tRns != null && tRns.Value.EqualRGB(r, g, b)) ? (byte)0x0 : (byte)0xFF;
                            bitmap_row_span[(int)x] = new YFColor((byte)(r >> 8), (byte)(g >> 8), (byte)(b >> 8), a);
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
            }
            else if (colorType == PngColorType.TruecolourWithAlpha)
            {
                if (bitDepth != 8 && bitDepth != 16)
                {
                    throw new Exception("Can not read IDAT chunk with color type TruecolourWithAlpha!");
                }
                uint x;
                if (bitDepth == 8)
                {
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 4, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 4)
                        {
                            bitmap_row_span[(int)x] = new YFColor(fliter_span[index], fliter_span[index + 1], fliter_span[index + 2], fliter_span[index + 3]);
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
                else if (bitDepth == 16)
                {
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 4, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 8)
                        {
                            bitmap_row_span[(int)x] = new YFColor((byte)(BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index, 2)) >> 8), (byte)(BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index + 2, 2)) >> 8), (byte)(BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index + 4, 2)) >> 8), (byte)(BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index + 6, 2)) >> 8));
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
            }
            else if (colorType == PngColorType.Greyscale)
            {
                if (bitDepth != 1 && bitDepth != 2 && bitDepth != 4 && bitDepth != 8 && bitDepth != 16)
                {
                    throw new Exception("Can not read IDAT chunk with color type Greyscale!");
                }
                uint x;
                byte grey, a;
                int greyint;
                if (bitDepth <= 8)
                {
                    int single_byte_index_count = 8 / bitDepth;
                    byte buffer_byte;
                    delegate*<int, int> expand = bitDepth switch
                    {
                        1 => &BitHelper.OneBitToEightBit,
                        2 => &BitHelper.TwoBitToEightBit,
                        4 => &BitHelper.FourBitToEightBit,
                        _ => &BitHelper.EightBitToEightBit,
                    };
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 1, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index++)
                        {
                            buffer_byte = fliter_span[index];
                            for (int c = 0; c < single_byte_index_count; c++)
                            {
                                if (x < width)
                                {
                                    greyint = buffer_byte >> (8 - bitDepth);
                                    a = (tRns != null && tRns.Value.EqualGrey(greyint)) ? (byte)0x0 : (byte)0xFF;
                                    grey = (byte)expand(greyint);
                                    bitmap_row_span[(int)x] = new YFColor(grey, grey, grey, a);
                                }
                                buffer_byte = (byte)((buffer_byte << bitDepth) & 0xFF);
                                x += deltaX;
                            }
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
                else if (bitDepth == 16)
                {
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 1, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 2)
                        {
                            greyint = BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index, 2));
                            a = (tRns != null && tRns.Value.EqualGrey(greyint)) ? (byte)0x0 : (byte)0xFF;
                            grey = (byte)(greyint >> 8);
                            bitmap_row_span[(int)x] = new YFColor(grey, grey, grey, a);
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
            }
            else if (colorType == PngColorType.GreyscaleWithAlpha)
            {
                if (bitDepth != 8 && bitDepth != 16)
                {
                    throw new Exception("Can not read IDAT chunk with color type GreyscaleWithAlpha!");
                }
                uint x;
                byte grey;
                if (bitDepth == 8)
                {
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 2, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 2)
                        {
                            grey = fliter_span[index];
                            bitmap_row_span[(int)x] = new YFColor(grey, grey, grey, fliter_span[index + 1]);
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
                else if (bitDepth == 16)
                {
                    for (uint y = startY; y < height; y += deltaY)
                    {
                        chunkStream.Read(raw_row_data_span);
                        GetFliterData(fliter_span, backup_span, row_fliter, 2, bitDepth);
                        x = startX;
                        Span<YFColor> bitmap_row_span = bitmap[(int)y];
                        for (int index = 0; index < row_len; index += 4)
                        {
                            grey = (byte)(BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index, 2)) >> 8);
                            bitmap_row_span[(int)x] = new YFColor(grey, grey, grey, (byte)(BinaryPrimitives.ReadUInt16BigEndian(fliter_span.Slice(index + 2, 2)) >> 8));
                            x += deltaX;
                        }
                        fliter_span.CopyTo(backup_span);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PaethPredictor(byte a, byte b, byte c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);
            int Pr;
            if (pa <= pb && pa <= pc)
            {
                Pr = a;
            }
            else if (pb <= pc)
            {
                Pr = b;
            }
            else
            {
                Pr = c;
            }
            return Pr;
        }

        private static int GetChannelCount(PngColorType colorType)
        {
            return colorType switch
            {
                PngColorType.Greyscale => 1,
                PngColorType.TruecolourWithAlpha => 4,
                PngColorType.Truecolour => 3,
                PngColorType.Indexedcolour => 1,
                PngColorType.GreyscaleWithAlpha => 2,
                _ => 0
            };
        }

        private static void GetFliterData(Span<byte> fliter, Span<byte> backup, int fliterType, int channelCount, int bitDepth)
        {
            if (fliterType != 0)
            {
                int x_delta = bitDepth < 8 ? 1 : (channelCount * bitDepth / 8);
                if (fliterType == 1)
                {
                    for (int i = 0; i < fliter.Length; i++)
                    {
                        fliter[i] = (byte)((fliter[i] + (i < x_delta ? 0 : fliter[i - x_delta])) & 0xFF);
                    }
                }
                else if (fliterType == 2)
                {
                    for (int i = 0; i < fliter.Length; i++)
                    {
                        fliter[i] = (byte)((fliter[i] + backup[i]) & 0xFF);
                    }
                }
                else if (fliterType == 3)
                {
                    for (int i = 0; i < fliter.Length; i++)
                    {
                        fliter[i] = (byte)((fliter[i] + (((i < x_delta ? 0 : fliter[i - x_delta]) + backup[i]) >> 1)) & 0xFF);
                    }
                }
                else if (fliterType == 4)
                {
                    byte a, b, c;
                    for (int i = 0; i < fliter.Length; i++)
                    {
                        a = i < x_delta ? (byte)0 : fliter[i - x_delta];
                        b = backup[i];
                        c = i < x_delta ? (byte)0 : backup[i - x_delta];
                        fliter[i] = (byte)((fliter[i] + PaethPredictor(a, b, c)) & 0xFF);
                    }
                }
                else
                {
                    throw new Exception($"Unknow filter type {fliterType}");
                }
            }
        }
    }
}
