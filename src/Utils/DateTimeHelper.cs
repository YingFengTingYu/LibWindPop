using System;

namespace LibWindPop.Utils
{
    public static class DateTimeHelper
    {
        public static bool IsEqual(DateTime a, DateTime b)
        {
            return Math.Abs((a - b).TotalSeconds) < 3.0;
        }
    }
}
