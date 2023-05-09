using LibWindPop.Utils.Extension;
using LibWindPop.Utils.Security;
using System;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal sealed class PngChunkWriteOnlyStream : Stream
    {
        private readonly Stream _stream;
        private Crc32 _crc;
        private uint _currentLen;
        private long _currentWriteLengthPosition;
        private bool _disposed;
        private readonly byte[] _chunk_type;
        private readonly long _max_part_len;
        private readonly Crc32 _start_crc;

        public PngChunkWriteOnlyStream(Stream stream, byte[] chunk_type, long max_part_len)
        {
            ThrowHelper.ThrowWhen(chunk_type == null || chunk_type.Length != 4);
            _start_crc = new Crc32();
            _start_crc.Init();
            _start_crc.Update(chunk_type);
            _stream = stream;
            _disposed = false;
            _chunk_type = chunk_type!;
            _max_part_len = max_part_len;
            WriteBeginChunk();
        }

        public PngChunkWriteOnlyStream(Stream stream, byte[] chunk_type)
        {
            ThrowHelper.ThrowWhen(chunk_type == null || chunk_type.Length != 4);
            _start_crc = new Crc32();
            _start_crc.Init();
            _start_crc.Update(chunk_type);
            _stream = stream;
            _disposed = false;
            _chunk_type = chunk_type!;
            _max_part_len = long.MaxValue;
            WriteBeginChunk();
        }

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override long Length => throw new NotImplementedException();

        public override bool CanWrite => true;

        public override bool CanSeek => false;

        public override bool CanRead => false;

        public override void Flush()
        {
            _stream.Flush();
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
                WriteEndChunk();
                _disposed = true;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(buffer.AsSpan().Slice(offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _crc.Update(buffer);
            _stream.Write(buffer);
            _currentLen += (uint)buffer.Length;
            if (_currentLen >= _max_part_len)
            {
                WriteEndChunk();
                WriteBeginChunk();
            }
        }

        public override void WriteByte(byte value)
        {
            _crc.Update(value);
            _stream.WriteByte(value);
            _currentLen++;
            if (_currentLen >= _max_part_len)
            {
                WriteEndChunk();
                WriteBeginChunk();
            }
        }

        private void WriteEndChunk()
        {
            _stream.WriteUInt32BE(_crc.Value);
            long current_pos = _stream.Position;
            _stream.Seek(_currentWriteLengthPosition, SeekOrigin.Begin);
            _stream.WriteUInt32BE(_currentLen);
            _stream.Seek(current_pos, SeekOrigin.Begin);
        }

        private void WriteBeginChunk()
        {
            _currentWriteLengthPosition = _stream.Position;
            _stream.WriteUInt32BE(0u);
            _stream.Write(_chunk_type);
            _currentLen = 0;
            _crc = _start_crc;
        }
    }
}
