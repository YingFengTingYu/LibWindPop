using System;

namespace LibWindPop.Utils.Logger
{
    public struct ConsoleLogger : ILogger
    {
        public readonly int LogLevel;

        public ConsoleLogger(int log_level)
        {
            LogLevel = log_level;
        }

        public void LogDebug(string msg, int space)
        {
            if (LogLevel <= -1)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                string append = GetSpaceString(space);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
        }

        public void Log(string msg, int space)
        {
            if (LogLevel <= 0)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                string append = GetSpaceString(space);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
        }

        public void LogWarning(string msg, int space)
        {
            if (LogLevel <= 1)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                string append = GetSpaceString(space);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
        }

        public void LogError(string msg, int space, bool throwException)
        {
            if (LogLevel <= 2)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                string append = GetSpaceString(space);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
            if (throwException)
            {
                throw new LoggerException(msg);
            }
        }

        public void LogException(Exception ex, int space, bool throwException)
        {
            if (LogLevel <= 3)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                string append = GetSpaceString(space);
                Console.WriteLine(append + ex.Message.Replace(Environment.NewLine, Environment.NewLine + append));
                if (ex.StackTrace != null)
                {
                    Console.WriteLine(append + ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + append));
                }
                Console.ForegroundColor = backup;
            }
            if (throwException)
            {
                throw new LoggerException(ex.Message, ex);
            }
        }

        private static string GetSpaceString(int space)
        {
            switch (space)
            {
                case <= 0:
                    return string.Empty;
                case 1:
                    return "    ";
                case 2:
                    return "        ";
                case 3:
                    return "            ";
                case 4:
                    return "                ";
                case 5:
                    return "                    ";
                case 6:
                    return "                        ";
                case 7:
                    return "                            ";
                case 8:
                    return "                                ";
                case >= 9:
                    return "                                    ";
            }
        }
    }
}
