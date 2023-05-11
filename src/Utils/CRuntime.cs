using System;

namespace LibWindPop.Utils
{
    internal readonly unsafe struct CType_H
    {
        public static byte ToUpper(byte value)
        {
            if (value >= 'a' && value <= 'z')
            {
                value ^= 32;
            }
            return value;
        }
    }

    internal readonly unsafe struct String_H
    {
        public static int StrLen(byte* str)
        {
            int len = 0;
            while (*str++ != 0)
            {
                len++;
            }
            return len;
        }

        public static int StrLen(byte* str, uint maxLen)
        {
            int len = 0;
            while (*str++ != 0 && len < maxLen)
            {
                len++;
            }
            return len;
        }

        public static int StrLen(ReadOnlySpan<byte> str)
        {
            int len = 0;
            int ptr = 0;
            while (str[ptr++] != 0 && len < str.Length)
            {
                len++;
            }
            return len;
        }

        public static int StrCmp(byte* sl, byte* s2)
        {
            for (; *sl == *s2; ++sl, ++s2)
            {
                if (*sl == 0)
                {
                    return 0;
                }
            }
            return (*sl < *s2) ? -1 : 1;
        }

        public static void MemSet(void* mem, byte value, nuint memSize)
        {
            byte* memPtr = (byte*)mem;
            for (nuint i = 0; i < memSize; i++)
            {
                *memPtr++ = value;
            }
        }
    }
}
