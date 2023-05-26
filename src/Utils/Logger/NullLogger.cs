using System;

namespace LibWindPop.Utils.Logger
{
    public readonly struct NullLogger : ILogger
    {
        public readonly bool ThrowException;

        public NullLogger(bool throwException)
        {
            ThrowException = throwException;
        }

        public readonly void LogDebug(string msg)
        {

        }

        public readonly void Log(string msg)
        {

        }

        public readonly void LogWarning(string msg)
        {

        }

        public readonly void LogError(string msg)
        {
            if (ThrowException)
            {
                throw new LoggerException(msg);
            }
        }

        public readonly void LogException(Exception ex)
        {
            if (ThrowException)
            {
                throw new LoggerException(ex.Message, ex);
            }
        }

        public readonly void Indent()
        {

        }

        public readonly void Unindent()
        {

        }
    }
}
