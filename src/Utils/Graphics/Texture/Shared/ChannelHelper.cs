using System.Runtime.CompilerServices;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal static class ChannelHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (byte, byte, byte) GetRGBFromL(byte l)
        {
            return (l, l, l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetLFromRGB(byte r, byte g, byte b)
        {
            return (byte)((r * 77 + g * 150 + b * 29) >> 8);
        }
    }
}
