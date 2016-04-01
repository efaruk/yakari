using System;

namespace Yakari
{
    public class NullLogger : ILogger
    {
        public void Log(string message) { }

        public void Log(LogLevel level, string message) { }

        public void Log(string message, Exception exception) { }

        public void Log(LogLevel level, string message, Exception exception) { }
    }
}