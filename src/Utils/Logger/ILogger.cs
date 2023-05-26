using System;

namespace LibWindPop.Utils.Logger
{
    public interface ILogger
    {
        void LogDebug(string msg);

        void Log(string msg);

        void LogWarning(string msg);

        void LogError(string msg);

        void LogException(Exception ex);

        void Indent();

        void Unindent();
    }
}
