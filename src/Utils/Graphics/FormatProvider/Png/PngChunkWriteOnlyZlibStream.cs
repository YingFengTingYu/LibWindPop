using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal sealed class PngChunkWriteOnlyZlibStream : Stream
    {
        private PngChunkWriteOnlyStream _png_chunk_stream;
        private DeflaterOutputStream _zlib_stream;
        private bool _disposed;

        public PngChunkWriteOnlyZlibStream(Stream stream, byte[] chunk_type, long max_part_len, int level)
        {
            _png_chunk_stream = new PngChunkWriteOnlyStream(stream, chunk_type, max_part_len);
            _zlib_stream = new DeflaterOutputStream(_png_chunk_stream, new Deflater(level));
            _zlib_stream.IsStreamOwner = false;
            _disposed = false;
        }

        public PngChunkWriteOnlyZlibStream(Stream stream, byte[] chunk_type, int level)
        {
            _png_chunk_stream = new PngChunkWriteOnlyStream(stream, chunk_type);
            _zlib_stream = new DeflaterOutputStream(_png_chunk_stream, new Deflater(level));
            _zlib_stream.IsStreamOwner = false;
            _disposed = false;
        }

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override long Length => throw new NotImplementedException();

        public override bool CanWrite => true;

        public override bool CanSeek => false;

        public override bool CanRead => false;

        public override void Flush()
        {
            _zlib_stream.Flush();
            _png_chunk_stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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
            _zlib_stream.Write(buffer, offset, count);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _zlib_stream.Write(buffer);
        }

        public override void WriteByte(byte value)
        {
            _zlib_stream.WriteByte(value);
        }
    }
}
