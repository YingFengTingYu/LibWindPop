using LibWindPop.Utils.Extension;
using System;
using System.Buffers.Binary;
using System.Text;

namespace LibWindPop.Utils
{
    internal static class UInt32StringConvertor
    {
        private static readonly Encoding iso_8859_1 = EncodingType.iso_8859_1.GetEncoding();

        public static string? UInt32ToString(uint loc)
        {
            if (loc == 0)
            {
                return null;
            }
            Span<byte> strBuffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(strBuffer, loc);
            return iso_8859_1.GetString(strBuffer);
        }

        public static uint StringToUInt32(string? loc)
        {
            if (loc == null)
            {
                return 0u;
            }
            ReadOnlySpan<char> locSpan = loc;
            if (locSpan.Length > 8)
            {
                locSpan = locSpan[..8];
            }
            int locSize = iso_8859_1.GetByteCount(loc);
            Span<byte> locBuffer = stackalloc byte[locSize];
            iso_8859_1.GetBytes(locSpan, locBuffer);
            return BinaryPrimitives.ReadUInt32BigEndian(locBuffer);
        }
    }
}
