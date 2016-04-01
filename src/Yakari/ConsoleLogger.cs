using System;

namespace Yakari
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            var line = string.Format("{0} : {1}", DateTime.UtcNow, message);
            Console.WriteLine(line);
        }

        public void Log(LogLevel level, string message)
        {
            var line = string.Format("{0} : {1}", DateTime.UtcNow, message);
            Console.WriteLine(line);
        }

        public void Log(string message, Exception exception)
        {
            if (exception == null) exception = new ArgumentNullException("exception");
            var line = string.Format("{0} happend: {1} \r\n\t\t Stack Trace: {2}, ", message, exception.Message, exception.StackTrace);
            Console.WriteLine(line);
        }

        public void Log(LogLevel level, string message, Exception exception)
        {
            if (exception == null) exception = new ArgumentNullException("exception");
            var line = string.Format("{0} happend with Exception:\r\n\t {1} \r\n\t\t Stack Trace: {2}, ", message, exception.Message, exception.StackTrace);
            Console.WriteLine(line);
        }
    }
}