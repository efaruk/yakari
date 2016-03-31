using System;
using System.Threading;

namespace Yakari
{
    internal static class ThreadHelper
    {
        public static void RunOnDifferentThread(Action action, bool swallowException = false)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                try
                {
                    action();
                }
                catch (Exception)
                {
                    if (!swallowException)
                        throw;
                }
            });

        }

        public static void RunOnDifferentThread(Action action, Action<Exception> catchAction)
        {
            ThreadPool.QueueUserWorkItem(t =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    catchAction(ex);
                }
            });
        }

        public static void WaitFor(TimeSpan timeOut)
        {
            Thread.Sleep((int)timeOut.TotalMilliseconds);
        }

        public static void WaitFor(int timeOut)
        {
            Thread.Sleep(timeOut);
        }

        public static void WaitForSeconds(int timeOut)
        {
            Thread.Sleep(timeOut * 1000);
        }
    }
}