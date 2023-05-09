using System;
using System.Runtime.CompilerServices;

namespace LibWindPop.Utils
{
    /// <summary>
    /// help to deal with bit operation
    /// </summary>
    internal static class BitHelper
    {
        /// <summary>
        /// Scale 1 bit to 8 bits
        /// </summary>
        /// <param name="value">1 bit value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int OneBitToEightBit(int value)
        {
            return (value & 0b1) == 0 ? 0 : 0xFF;
        }

        /// <summary>
        /// Scale 8 bits to 1 bit
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>1 bit value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToOneBit(int value)
        {
            return (value & 0b10000000) >> 7;
        }

        /// <summary>
        /// Scale 2 bits to 8 bits
        /// </summary>
        /// <param name="value">2 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TwoBitToEightBit(int value)
        {
            value &= 0b11;
            value = (value << 2) | value;
            return (value << 4) | value;
        }

        /// <summary>
        /// Scale 8 bits to 2 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>2 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToTwoBit(int value)
        {
            return (value & 0b11000000) >> 6;
        }

        /// <summary>
        /// Scale 3 bits to 8 bits
        /// </summary>
        /// <param name="value">3 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ThreeBitToEightBit(int value)
        {
            value &= 0b111;
            return (value << 5) | (value << 2) | (value >> 1);
        }

        /// <summary>
        /// Scale 8 bits to 3 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>3 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToThreeBit(int value)
        {
            return (value & 0b11100000) >> 5;
        }

        /// <summary>
        /// Scale 4 bits to 8 bits
        /// </summary>
        /// <param name="value">4 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FourBitToEightBit(int value)
        {
            value &= 0b1111;
            return (value << 4) | value;
        }

        /// <summary>
        /// Scale 8 bits to 4 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>4 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToFourBit(int value)
        {
            return (value & 0b11110000) >> 4;
        }

        /// <summary>
        /// Scale 5 bits to 8 bits
        /// </summary>
        /// <param name="value">5 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FiveBitToEightBit(int value)
        {
            value &= 0b11111;
            return (value << 3) | (value >> 2);
        }
        /// <summary>
        /// Scale 8 bits to 5 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>5 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToFiveBit(int value)
        {
            return (value & 0b11111000) >> 3;
        }

        /// <summary>
        /// Scale 6 bits to 8 bits
        /// </summary>
        /// <param name="value">6 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SixBitToEightBit(int value)
        {
            value &= 0b111111;
            return (value << 2) | (value >> 4);
        }

        /// <summary>
        /// Scale 8 bits to 6 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>6 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToSixBit(int value)
        {
            return (value & 0b11111100) >> 2;
        }

        /// <summary>
        /// Scale 7 bits to 8 bits
        /// </summary>
        /// <param name="value">7 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SevenBitToEightBit(int value)
        {
            value &= 0b1111111;
            return (value << 1) | (value >> 6);
        }

        /// <summary>
        /// Scale 8 bits to 7 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>7 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToSevenBit(int value)
        {
            return (value & 0b11111110) >> 1;
        }

        /// <summary>
        /// Scale 8 bits to 8 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EightBitToEightBit(int value)
        {
            return value & 0b11111111;
        }

        /// <summary>
        /// Scale 1 bits to 8 bits
        /// </summary>
        /// <param name="value">1 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong OneBitToEightBit(ulong value)
        {
            return (value & 0b1) == 0 ? 0ul : 0xFFul;
        }

        /// <summary>
        /// Scale 8 bits to 1 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>1 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToOneBit(ulong value)
        {
            return (value & 0b10000000) >> 7;
        }

        /// <summary>
        /// Scale 2 bits to 8 bits
        /// </summary>
        /// <param name="value">2 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong TwoBitToEightBit(ulong value)
        {
            value &= 0b11;
            value = (value << 2) | value;
            return (value << 4) | value;
        }

        /// <summary>
        /// Scale 8 bits to 2 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>2 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToTwoBit(ulong value)
        {
            return (value & 0b11000000) >> 6;
        }

        /// <summary>
        /// Scale 3 bits to 8 bits
        /// </summary>
        /// <param name="value">3 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ThreeBitToEightBit(ulong value)
        {
            value &= 0b111;
            return (value << 5) | (value << 2) | (value >> 1);
        }

        /// <summary>
        /// Scale 8 bits to 3 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>3 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToThreeBit(ulong value)
        {
            return (value & 0b11100000) >> 5;
        }

        /// <summary>
        /// Scale 4 bits to 8 bits
        /// </summary>
        /// <param name="value">4 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FourBitToEightBit(ulong value)
        {
            value &= 0b1111;
            return (value << 4) | value;
        }

        /// <summary>
        /// Scale 8 bits to 4 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>4 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToFourBit(ulong value)
        {
            return (value & 0b11110000) >> 4;
        }

        /// <summary>
        /// Scale 5 bits to 8 bits
        /// </summary>
        /// <param name="value">5 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FiveBitToEightBit(ulong value)
        {
            value &= 0b11111;
            return (value << 3) | (value >> 2);
        }

        /// <summary>
        /// Scale 8 bits to 5 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>5 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToFiveBit(ulong value)
        {
            return (value & 0b11111000) >> 3;
        }

        /// <summary>
        /// Scale 6 bits to 8 bits
        /// </summary>
        /// <param name="value">6 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SixBitToEightBit(ulong value)
        {
            value &= 0b111111;
            return (value << 2) | (value >> 4);
        }

        /// <summary>
        /// Scale 8 bits to 6 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>6 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToSixBit(ulong value)
        {
            return (value & 0b11111100) >> 2;
        }

        /// <summary>
        /// Scale 7 bits to 8 bits
        /// </summary>
        /// <param name="value">7 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SevenBitToEightBit(ulong value)
        {
            value &= 0b1111111;
            return (value << 1) | (value >> 6);
        }

        /// <summary>
        /// Scale 8 bits to 7 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>7 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToSevenBit(ulong value)
        {
            return (value & 0b11111110) >> 1;
        }

        /// <summary>
        /// Scale 8 bits to 8 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong EightBitToEightBit(ulong value)
        {
            return value & 0b11111111;
        }

        /// <summary>
        /// Scale 8 bits to 8 bits
        /// </summary>
        /// <param name="value">8 bits value</param>
        /// <returns>8 bits value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Endian8In16(Span<byte> data)
        {
            int len = data.Length >> 1 << 1;
            for (int i = 0; i < len; i += 2)
            {
                (data[i], data[i | 1]) = (data[i | 1], data[i]);
            }
        }

        /// <summary>
        /// check if the value is power of two
        /// </summary>
        /// <param name="value">checked value</param>
        /// <returns>if the value is power of two, return true; otherwise, return false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetClosestPowerOfTwoAbove(int value)
        {
            int ans = 1;
            while (ans < value)
            {
                ans <<= 1;
            }
            return ans;
        }
    }
}
