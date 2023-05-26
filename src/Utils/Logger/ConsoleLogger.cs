using System;

namespace LibWindPop.Utils.Logger
{
    public struct ConsoleLogger : ILogger
    {
        public readonly int LogLevel;
        public readonly bool ThrowException;
        private int m_indent;

        public ConsoleLogger(int logLevel, bool throwException)
        {
            LogLevel = logLevel;
            ThrowException = throwException;
        }

        public readonly void LogDebug(string msg)
        {
            if (LogLevel <= -1)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                string append = GetSpaceString(m_indent);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
        }

        public readonly void Log(string msg)
        {
            if (LogLevel <= 0)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                string append = GetSpaceString(m_indent);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
        }

        public readonly void LogWarning(string msg)
        {
            if (LogLevel <= 1)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                string append = GetSpaceString(m_indent);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
        }

        public readonly void LogError(string msg)
        {
            if (LogLevel <= 2)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                string append = GetSpaceString(m_indent);
                Console.WriteLine(append + msg.Replace(Environment.NewLine, Environment.NewLine + append));
                Console.ForegroundColor = backup;
            }
            if (ThrowException)
            {
                throw new LoggerException(msg);
            }
        }

        public readonly void LogException(Exception ex)
        {
            if (LogLevel <= 3)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                string append = GetSpaceString(m_indent);
                Console.WriteLine(append + ex.Message.Replace(Environment.NewLine, Environment.NewLine + append));
                if (ex.StackTrace != null)
                {
                    Console.WriteLine(append + ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + append));
                }
                Console.ForegroundColor = backup;
            }
            if (ThrowException)
            {
                throw new LoggerException(ex.Message, ex);
            }
        }

        public void Indent()
        {
            m_indent++;
        }

        public void Unindent()
        {
            if (m_indent > 0)
            {
                m_indent--;
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
                case 9:
                    return "                                    ";
                case 10:
                    return "                                        ";
                case 11:
                    return "                                            ";
                case 12:
                    return "                                                ";
                case 13:
                    return "                                                    ";
                case 14:
                    return "                                                        ";
                case 15:
                    return "                                                            ";
                case 16:
                    return "                                                                ";
                case 17:
                    return "                                                                    ";
                case 18:
                    return "                                                                        ";
                case >= 19:
                    return "                                                                            ";
            }
        }
    }
}
