using System;

namespace LibWindPop.Utils.Logger
{
    public struct NullLogger : ILogger
    {
        public void LogDebug(string msg, int space)
        {

        }

        public void Log(string msg, int space)
        {

        }

        public void LogWarning(string msg, int space)
        {

        }

        public void LogError(string msg, int space, bool throwException)
        {
            if (throwException)
            {
                throw new LoggerException(msg);
            }
        }

        public void LogException(Exception ex, int space, bool throwException)
        {
            if (throwException)
            {
                throw new LoggerException(ex.Message, ex);
            }
        }
    }
}
