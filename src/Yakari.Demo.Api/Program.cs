using System;
using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using Nancy.Hosting.Self;
namespace Yakari.Demo.Api
{
    internal class Program
    {
        private static SynchronizationContext _syncContext;

        // Resource: https://github.com/NancyFx/Nancy/wiki/Hosting-Nancy-with-Nginx-on-Ubuntu
        private static void Main(string[] args)
        {
            
            var uri = "http://localhost:8888/";
            AsciiArt.AsciiArt.Write("Yakari");
            AsciiArt.AsciiArt.WriteLineSeparator('~');
            Console.WriteLine("Starting Nancy Host on {0}\r\n", uri);
            var configuration = new HostConfiguration
            {
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };
            var bootStrapper = new HostAutofacBootstrapper();
            // initialize an instance of NancyHost
            var host = new NancyHost(new Uri(uri), bootStrapper, configuration);
            Start(host); // start hosting

            // check if we're running on mono
            if (Type.GetType("Mono.Runtime") != null)
            {
                Console.WriteLine("Running on Mono");
                var p = (int)Environment.OSVersion.Platform;
                if ((p == 4) || (p == 6) || (p == 128))
                {
                    Console.WriteLine("Running on Unix Like System");
                    WaitForTerminationOnUnix();
                }
                else
                {
                    Console.WriteLine("Running on Windows");
                    WaitForTerminationOnWindows();
                }
            }
            else
            {
                Console.WriteLine("Running on .Net");
                WaitForTerminationOnWindows();
            }
            Console.WriteLine("Stopping Nancy Host");
            AsciiArt.AsciiArt.WriteLineSeparator('~');
            Stop(host); // stop hosting
        }

        private static void Start(NancyHost host)
        {
            _syncContext = SynchronizationContext.Current;
            host.Start(); // start hosting
        }

        private static void Stop(NancyHost host)
        {
            if (_syncContext == SynchronizationContext.Current)
            {
                host.Dispose();
            }
            else
            {
                _syncContext.Post((state) =>
                {
                    host.Dispose();
                }, null);
            }
            try
            {
                host.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception: {0}\r\n Stack Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        private static void WaitForTerminationOnWindows()
        {
            // Little modification for process control
            var t = string.Empty;
            while (!t.Equals("q", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("To stop process press Q and accept.");
                AsciiArt.AsciiArt.WriteLineSeparator('~');
                t = Console.ReadLine() ?? string.Empty;
            }
        }

        private static void WaitForTerminationOnUnix()
        {
            Console.WriteLine("To stop process send a termination signal.");
            AsciiArt.AsciiArt.WriteLineSeparator('~');
            // on mono, processes will usually run as daemons - this allows you to listen
            // for termination signals (ctrl+c, shutdown, etc) and finalize correctly
            UnixSignal.WaitAny(new[]
            {
                new UnixSignal(Signum.SIGINT),
                new UnixSignal(Signum.SIGTERM),
                new UnixSignal(Signum.SIGQUIT),
                new UnixSignal(Signum.SIGHUP)
            });
        }
    }
}
