using System;

namespace LibWindPop.Utils
{
    internal static class ThrowHelper
    {
        public static void ThrowWhen(bool value)
        {
            if (value)
            {
                throw new Exception();
            }
        }
    }
}
