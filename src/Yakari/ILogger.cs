using System;
using System.Collections.Generic;

namespace Yakari
{
    public interface ILogger
    {
        void Log(string message);

        void Log(LogLevel level, string message);

        void Log(string message, Exception exception);

        void Log(LogLevel level, string message, Exception exception);
    }

    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public class SimpleContext
    {
        public SimpleContext()
        {
            Items = new Dictionary<string, object>(10);
        }

        public SimpleContext(Exception exception) : this()
        {
            Exception = exception;
        }

        public Dictionary<string, object> Items { get; }

        public object this[string name]
        {
            get { return Items[name]; }
            set { Items[name] = value; }
        }

        public Exception Exception { get; }
    }
}