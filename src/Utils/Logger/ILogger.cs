using System;
using System.Runtime.Serialization;

namespace LibWindPop.Utils.Logger
{
    public interface ILogger
    {
        void LogDebug(string msg, int space);

        void Log(string msg, int space);

        void LogWarning(string msg, int space);

        void LogError(string msg, int space, bool throwException);

        void LogException(Exception ex, int space, bool throwException);
    }

    public class LoggerException : Exception
    {
        public LoggerException()
        {
        }

        public LoggerException(string? message) : base(message)
        {
        }

        public LoggerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
