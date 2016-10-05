using System;
using System.Diagnostics;
using System.Threading;

namespace Yakari.Core
{
    internal static class ThreadHelper
    {
        public static void RunOnDifferentThread(Action action, bool swallowException = false)
        {
            ThreadPool.QueueUserWorkItem(t =>
            //Task.Run(() =>
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
            //Task.Run(() =>
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

        public static void WaitFor(TimeSpan timeout)
        {
            Thread.Sleep((int)timeout.TotalMilliseconds);
        }

        public static void WaitFor(int timeout)
        {
            Thread.Sleep(timeout);
        }

        public static void WaitForSeconds(int timeout)
        {
            Thread.Sleep(timeout * 1000);
        }

        /// <summary>
        ///     Waits for NOT NULL result, until timeout pass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static T WaitForResult<T>(Func<T> func, TimeSpan timeout)
        {
            var sw = new Stopwatch();
            sw.Start();
            var waitFor = timeout.TotalMilliseconds;
            T t;
            do
            {
                t = func();
                if (t != null) break;
            } while (sw.ElapsedMilliseconds < waitFor);
            sw.Stop();
            return t;
        }
    }
}