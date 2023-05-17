using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibWindPop.Utils
{
    internal class UnsafeStringHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void StringToUpper(nuint stringPtrNumber)
        {
            byte* ptr = (byte*)stringPtrNumber;
            while (*ptr != 0)
            {
                *ptr = CType_H.ToUpper(*ptr);
                ptr++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetUtf16String(in nuint pointer, in Encoding encoding)
        {
            return encoding.GetString(new ReadOnlySpan<byte>((byte*)pointer, String_H.StrLen((byte*)pointer)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetUtf16String(in nuint pointer, in uint maxLength, in Encoding encoding)
        {
            return encoding.GetString(new ReadOnlySpan<byte>((byte*)pointer, String_H.StrLen((byte*)pointer, maxLength)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SetUtf16String(in string? str, in void* pointer, in uint max_length, in Encoding encoding)
        {
            if (str != null)
            {
                ReadOnlySpan<char> strSpan = str;
                if (strSpan.Length > (int)max_length)
                {
                    strSpan = strSpan[..(int)max_length];
                }
                int max = (int)max_length;
                int count = encoding.GetByteCount(strSpan);
                if (count <= max)
                {
                    return encoding.GetBytes(strSpan, new Span<byte>(pointer, count));
                }
                else
                {
                    Span<byte> buffer = stackalloc byte[count];
                    encoding.GetBytes(strSpan, buffer);
                    buffer[..max].CopyTo(new Span<byte>(pointer, max));
                    return max;
                }
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SetUtf16String(in string? str, in nuint pointer, in Encoding encoding)
        {
            if (str != null)
            {
                int count = encoding.GetByteCount(str);
                encoding.GetBytes(str, new Span<byte>((void*)pointer, count));
                return count;
            }
            return 0;
        }
    }
}
