using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal sealed class PngChunkReadOnlyZlibStream : Stream
    {
        private readonly PngChunkReadOnlyStream _png_chunk_stream;
        private readonly InflaterInputStream _zlib_stream;
        private bool _disposed;
        public readonly byte[] ChunkType;

        public PngChunkReadOnlyZlibStream(Stream stream)
        {
            _png_chunk_stream = new PngChunkReadOnlyStream(stream);
            _zlib_stream = new InflaterInputStream(_png_chunk_stream, new Inflater());
            _zlib_stream.IsStreamOwner = false;
            _disposed = false;
            ChunkType = _png_chunk_stream.ChunkType;
        }

        public PngChunkReadOnlyZlibStream(Stream stream, uint max_chunk_num)
        {
            _png_chunk_stream = new PngChunkReadOnlyStream(stream, max_chunk_num);
            _zlib_stream = new InflaterInputStream(_png_chunk_stream, new Inflater());
            _zlib_stream.IsStreamOwner = false;
            _disposed = false;
            ChunkType = _png_chunk_stream.ChunkType;
        }

        public PngChunkReadOnlyZlibStream(Stream stream, uint max_chunk_num, bool CgBI)
        {
            _png_chunk_stream = new PngChunkReadOnlyStream(stream, max_chunk_num);
            _zlib_stream = new InflaterInputStream(_png_chunk_stream, new Inflater(CgBI));
            _zlib_stream.IsStreamOwner = false;
            _disposed = false;
            ChunkType = _png_chunk_stream.ChunkType;
        }

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override long Length => throw new NotImplementedException();

        public override bool CanWrite => false;

        public override bool CanSeek => false;

        public override bool CanRead => true;

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _zlib_stream.Read(buffer, offset, count);
        }

        public override int Read(Span<byte> buffer)
        {
            int already_read_len = 0;
            int zero_count = -3;
            int read_len;
            while (already_read_len < buffer.Length)
            {
                read_len = _zlib_stream.Read(buffer);
                already_read_len += read_len;
                if (read_len == 0)
                {
                    zero_count++;
                    if (zero_count >= 0)
                    {
                        return already_read_len;
                    }
                }
            }
            return already_read_len;
        }

        public override int ReadByte()
        {
            return _zlib_stream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _zlib_stream.Dispose();
                _png_chunk_stream.Dispose();
                _disposed = true;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            _zlib_stream.Flush();
            _png_chunk_stream.Flush();
        }
    }
}
