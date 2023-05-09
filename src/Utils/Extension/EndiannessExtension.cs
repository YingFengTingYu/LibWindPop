using System;

namespace LibWindPop.Utils.Extension
{
    public static class EndiannessExtension
    {
        public static bool IsBigEndian(this Endianness endianness)
        {
            return endianness == Endianness.Big || (endianness == Endianness.Native && !BitConverter.IsLittleEndian) || (endianness == Endianness.NoneNative && BitConverter.IsLittleEndian);
        }

        public static bool IsLittleEndian(this Endianness endianness)
        {
            return endianness == Endianness.Little || (endianness == Endianness.Native && BitConverter.IsLittleEndian) || (endianness == Endianness.NoneNative && !BitConverter.IsLittleEndian);
        }

        public static bool IsNativeEndian(this Endianness endianness)
        {
            return endianness == Endianness.Native || (endianness == Endianness.Little && BitConverter.IsLittleEndian) || (endianness == Endianness.Big && !BitConverter.IsLittleEndian);
        }

        public static bool IsNoneNativeEndian(this Endianness endianness)
        {
            return endianness == Endianness.NoneNative || (endianness == Endianness.Little && !BitConverter.IsLittleEndian) || (endianness == Endianness.Big && BitConverter.IsLittleEndian);
        }

        public static bool IsNoneEndian(this Endianness endianness)
        {
            return endianness != Endianness.Little && endianness != Endianness.Big && endianness != Endianness.Native && endianness != Endianness.NoneNative;
        }

        public static bool IsEqual(this Endianness endianness, Endianness expected)
        {
            return endianness.IsLittleEndian() && expected.IsLittleEndian();
        }
    }
}
