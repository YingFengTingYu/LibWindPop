using System;
using System.IO;

namespace LibWindPop.Utils
{
    internal class XorReadStream : Stream
    {
        public XorReadStream(Stream baseStream, byte key)
        {
            m_baseStream = baseStream;
            m_key = key;
        }

        private readonly Stream m_baseStream;
        private readonly byte m_key;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => m_baseStream.Length;

        public override long Position { get => m_baseStream.Position; set => m_baseStream.Position = value; }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int ReadByte()
        {
            int ans = m_baseStream.ReadByte();
            return ans == -1 ? -1 : (ans ^ m_key);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = m_baseStream.Read(buffer, offset, count);
            for (long i = offset; i < offset + num; i++)
            {
                buffer[i] ^= m_key;
            }
            return num;
        }

        public override int Read(Span<byte> buffer)
        {
            int num = m_baseStream.Read(buffer);
            for (int i = 0; i < num; i++)
            {
                buffer[i] ^= m_key;
            }
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
