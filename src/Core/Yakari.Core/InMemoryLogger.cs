using System;
using System.Collections.Generic;
using Yakari.Core.Interfaces;

namespace Yakari.Core
{
    /// <summary>
    ///     In Memory Logger
    /// </summary>
    public class InMemoryLogger : ILogger
    {
        readonly LogLevel _minimumLogLevel;
        /// <summary>
        ///     LogList
        /// </summary>
        public readonly List<string> LogList = new List<string>(1000);

        /// <summary>
        ///     Default ctor
        /// </summary>
        /// <param name="minimumLogLevel"></param>
        public InMemoryLogger(LogLevel minimumLogLevel)
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

        void LogInternal(LogLevel level, string message, Exception exception = null)
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
                LogList.Add(line);
            }, true);
        }
    }
}