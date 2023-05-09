using System;

namespace LibWindPop.Utils.Security
{
    public static unsafe class Md5
    {
        public static void Compute(ReadOnlySpan<byte> src, Span<byte> value)
        {
            MD5Digest digest = new MD5Digest();
            digest.BlockUpdate(src);
            digest.DoFinal(value);
        }
    }
}
