using System;
using System.Buffers;
using System.IO;

namespace LibWindPop.Utils
{
    internal class XorStream : Stream
    {
        public XorStream(Stream baseStream, byte key)
        {
            m_baseStream = baseStream;
            m_key = key;
            m_cryptBuffer = ArrayPool<byte>.Shared.Rent(81920);
        }

        private readonly Stream m_baseStream;
        private readonly byte m_key;

        private byte[]? m_cryptBuffer;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => m_baseStream.Length;

        public override long Position { get => m_baseStream.Position; set => m_baseStream.Position = value; }

        public override void Flush()
        {
            m_baseStream.Flush();
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

        public override void WriteByte(byte value)
        {
            m_baseStream.WriteByte((byte)(value ^ m_key));
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (m_cryptBuffer != null)
            {
                lock (m_cryptBuffer)
                {
                    if (m_cryptBuffer != null)
                    {
                        ReadOnlySpan<byte> bufferSpan = new ReadOnlySpan<byte>(buffer, offset, count);
                        for (int i = 0; i < bufferSpan.Length; i += m_cryptBuffer.Length)
                        {
                            int thisLength = Math.Min(bufferSpan.Length - i, m_cryptBuffer.Length);
                            for (int index = 0; index < thisLength; index++)
                            {
                                m_cryptBuffer[index] = (byte)(bufferSpan[index] ^ m_key);
                            }
                            m_baseStream.Write(m_cryptBuffer, 0, thisLength);
                        }
                    }
                }
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (m_cryptBuffer != null)
            {
                lock (m_cryptBuffer)
                {
                    if (m_cryptBuffer != null)
                    {
                        for (int i = 0; i < buffer.Length; i += m_cryptBuffer.Length)
                        {
                            int thisLength = Math.Min(buffer.Length - i, m_cryptBuffer.Length);
                            for (int index = 0; index < thisLength; index++)
                            {
                                m_cryptBuffer[index] = (byte)(buffer[index] ^ m_key);
                            }
                            m_baseStream.Write(new ReadOnlySpan<byte>(m_cryptBuffer, 0, thisLength));
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (m_cryptBuffer != null)
            {
                lock (m_cryptBuffer)
                {
                    if (m_cryptBuffer != null)
                    {
                        ArrayPool<byte>.Shared.Return(m_cryptBuffer);
                        m_cryptBuffer = null;
                    }
                }
            }
        }
    }
}
