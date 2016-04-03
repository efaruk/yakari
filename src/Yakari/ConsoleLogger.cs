using System;

namespace Yakari
{
    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel _minimumLogLevel;

        public ConsoleLogger(LogLevel minimumLogLevel)
        {
            _minimumLogLevel = minimumLogLevel;
        }

        public void Log(string message)
        {
            LogInternal(LogLevel.Info, message);
        }

        public void Log(LogLevel level, string message)
        {
            LogInternal(LogLevel.Error, message);
        }

        public void Log(string message, Exception exception)
        {
            LogInternal(LogLevel.Error, message, exception);
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            LogInternal(level, message, exception);
        }

        private void LogInternal(LogLevel level, string message, Exception exception = null)
        {
            ThreadHelper.RunOnDifferentThread(() =>
            {
                if (_minimumLogLevel > level)
                    return;
                string line;
                if (exception != null)
                    line = string.Format("{0} happend with Exception:\r\n\t {1} \r\n\t\t Stack Trace: {2}, \r\n ---------------------------------------------------------------------",
                        message, exception.Message, exception.StackTrace);
                else
                    line = string.Format("{0} : {1}", DateTime.UtcNow, message);
                Console.WriteLine(line);
            }, true);
        }
    }
}