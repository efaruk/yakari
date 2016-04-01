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
            if (_minimumLogLevel > LogLevel.Info) return;
            var line = string.Format("{0} : {1}", DateTime.UtcNow, message);
            Console.WriteLine(line);
        }

        public void Log(LogLevel level, string message)
        {
            if (_minimumLogLevel > level) return;
            var line = string.Format("{0} : {1}", DateTime.UtcNow, message);
            Console.WriteLine(line);
        }

        public void Log(string message, Exception exception)
        {
            if (_minimumLogLevel > LogLevel.Error) return;
            if (exception == null) exception = new ArgumentNullException("exception");
            var line = string.Format("{0} happend: {1} \r\n\t\t Stack Trace: {2}, \r\n ---------------------------------------------------------------------"
                , message, exception.Message, exception.StackTrace);
            Console.WriteLine(line);
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            if (_minimumLogLevel > level) return;
            if (exception == null) exception = new ArgumentNullException("exception");
            var line = string.Format("{0} happend with Exception:\r\n\t {1} \r\n\t\t Stack Trace: {2}, \r\n ---------------------------------------------------------------------",
                message, exception.Message, exception.StackTrace);
            Console.WriteLine(line);
        }
    }
}