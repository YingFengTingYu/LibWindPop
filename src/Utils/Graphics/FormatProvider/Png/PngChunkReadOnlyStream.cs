using System;
using System.Buffers.Binary;
using System.IO;

namespace LibWindPop.Utils.Graphics.FormatProvider.Png
{
    internal sealed class PngChunkReadOnlyStream : Stream
    {
        private readonly Stream _stream;
        private uint _len;
        private uint _currentLen;
        private uint _currentPosition;
        private uint _currentChunk;
        private uint _chunk_num;
        private bool _disposed;
        public byte[] ChunkType;
        private bool _this_already_read_end;

        public PngChunkReadOnlyStream(Stream stream) : this(stream, 1)
        {

        }

        public PngChunkReadOnlyStream(Stream stream, uint max_chunk_num)
        {
            _stream = stream;
            _currentPosition = 0u;
            _currentChunk = 0u;
            _disposed = false;
            _len = 0u;
            uint len;
            long pos = _stream.Position;
            _this_already_read_end = true;
            while ((len = ReadHead()) != uint.MaxValue && _chunk_num < max_chunk_num)
            {
                _len += len;
                _chunk_num++;
                _stream.Seek(len, SeekOrigin.Current);
                ReadEnd();
            }
            _stream.Seek(pos, SeekOrigin.Begin);
            _this_already_read_end = true;
            _currentLen = ReadHead();
            ChunkType ??= new byte[4];
        }

        private uint ReadHead()
        {
            if (!_this_already_read_end)
            {
                return uint.MaxValue;
            }
            if (_stream.Position >= _stream.Length)
            {
                return uint.MaxValue;
            }
            Span<byte> buffer = stackalloc byte[4];
            _stream.Read(buffer);
            uint len = BinaryPrimitives.ReadUInt32BigEndian(buffer);
            _stream.Read(buffer);
            if (ChunkType == null)
            {
                ChunkType = new byte[4];
                buffer.CopyTo(ChunkType);
                _this_already_read_end = false;
                return len;
            }
            else
            {
                if (buffer[0] == ChunkType[0]
                    && buffer[1] == ChunkType[1]
                    && buffer[2] == ChunkType[2]
                    && buffer[3] == ChunkType[3])
                {
                    _this_already_read_end = false;
                    return len;
                }
            }
            return uint.MaxValue;
        }

        private void ReadEnd()
        {
            if (!_this_already_read_end)
            {
                _this_already_read_end = true;
                _stream.Seek(0x4, SeekOrigin.Current);
            }
        }

        public override long Position { get => _currentPosition; set => throw new NotImplementedException(); }

        public override long Length => _len;

        public override bool CanWrite => false;

        public override bool CanSeek => false;

        public override bool CanRead => true;

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan().Slice(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            int already_read_len = 0;
            int delta;
            while ((delta = buffer.Length - already_read_len) > 0)
            {
                already_read_len += ReadInternal(buffer.Slice(already_read_len, delta));
                if (_currentLen == _currentPosition)
                {
                    if (!TryMoveNext())
                    {
                        return already_read_len;
                    }
                }
            }
            return already_read_len;
        }

        private int ReadInternal(Span<byte> buffer)
        {
            int max_read_len = (int)((long)_currentLen - _currentPosition);
            if (buffer.Length >= max_read_len)
            {
                buffer = buffer[..max_read_len];
            }
            int read_len = _stream.Read(buffer);
            _currentPosition += (uint)read_len;
            return read_len;
        }

        public override int ReadByte()
        {
            if (_currentPosition == _currentLen)
            {
                if (TryMoveNext())
                {
                    return ReadByte();
                }
                return -1;
            }
            int read_byte = _stream.ReadByte();
            if (read_byte != -1)
            {
                _currentPosition++;
            }
            return read_byte;
        }

        private bool TryMoveNext()
        {
            ToChunkEnd();
            ReadEnd();
            _currentChunk++;
            if (_currentChunk >= _chunk_num)
            {
                return false;
            }
            _currentLen = ReadHead();
            _currentPosition = 0;
            return true;
        }

        public void ToChunkEnd()
        {
            int max_read_len = (int)((long)_currentLen - _currentPosition);
            if (max_read_len > 0)
            {
                _stream.Seek(max_read_len, SeekOrigin.Current);
                _currentPosition = _currentLen;
            }
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
                while (TryMoveNext()) ;
                _disposed = true;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            _stream.Flush();
        }
    }
}
