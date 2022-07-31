using System;
using System.Text;

namespace TCPClient
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Exception
    }

    public static class Logger
    {
        public delegate void ErrorEventHandler(LogLevel level);
        public static event ErrorEventHandler OnErrorEvent;

        private const LogLevel defaultLevel = LogLevel.Info;

        public static void Log(string message)
        {
            Log(defaultLevel, message);
        }

        private static void HandleLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    //nofity listeners about an error
                    OnErrorEvent(level);
                    break;

                case LogLevel.Exception:
                    //nofity listeners about an exception
                    OnErrorEvent(level);
                    break;

                default:
                    break;
            }
        }
        public static void Log(LogLevel level, string message)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}] [Client] [{level}] {message}");
            HandleLogLevel(level);
        }
    }
}
