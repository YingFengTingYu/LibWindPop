using System;

namespace LibWindPop.Utils.Security
{
    internal abstract class GeneralDigest
    {
        private const int BYTE_LENGTH = 64;

        private byte[] xBuf;
        private int xBufOff;

        private long byteCount;

        internal GeneralDigest()
        {
            xBuf = new byte[4];
        }

        internal GeneralDigest(GeneralDigest t)
        {
            xBuf = new byte[t.xBuf.Length];
            CopyIn(t);
        }

        protected void CopyIn(GeneralDigest t)
        {
            Array.Copy(t.xBuf, 0, xBuf, 0, t.xBuf.Length);

            xBufOff = t.xBufOff;
            byteCount = t.byteCount;
        }

        public void Update(byte input)
        {
            xBuf[xBufOff++] = input;

            if (xBufOff == xBuf.Length)
            {
                ProcessWord(xBuf);
                xBufOff = 0;
            }

            byteCount++;
        }

        public void BlockUpdate(ReadOnlySpan<byte> input)
        {
            int length = input.Length;

            //
            // fill the current word
            //
            int i = 0;
            if (xBufOff != 0)
            {
                while (i < length)
                {
                    xBuf[xBufOff++] = input[i++];
                    if (xBufOff == 4)
                    {
                        ProcessWord(xBuf);
                        xBufOff = 0;
                        break;
                    }
                }
            }

            //
            // process whole words.
            //
            int limit = length - 3;
            for (; i < limit; i += 4)
            {
                ProcessWord(input.Slice(i, 4));
            }

            //
            // load in the remainder.
            //
            while (i < length)
            {
                xBuf[xBufOff++] = input[i++];
            }

            byteCount += length;
        }

        public void Finish()
        {
            long bitLength = (byteCount << 3);

            //
            // add the pad bytes.
            //
            Update((byte)128);

            while (xBufOff != 0) Update((byte)0);
            ProcessLength(bitLength);
            ProcessBlock();
        }

        public virtual void Reset()
        {
            byteCount = 0;
            xBufOff = 0;
            Array.Clear(xBuf, 0, xBuf.Length);
        }

        internal abstract void ProcessWord(ReadOnlySpan<byte> word);

        internal abstract void ProcessLength(long bitLength);

        internal abstract void ProcessBlock();

        public abstract int DoFinal(Span<byte> output);
    }
}
