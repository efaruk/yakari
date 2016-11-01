using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Yakari.Demo.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = Parse(args);
            var url = string.Format("http://localhost:{0}", options.PortNumber);
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        private static Options Parse(string[] args)
        {
            var options = new Options();
            if (args.Length == 0) return options;
            if (args.Length == 2 && args[0].ToLowerInvariant().EndsWith("-json"))
            {
                options = JsonConvert.DeserializeObject<Options>(args[1]);
                return options;
            }
            if (args[0].ToLowerInvariant().EndsWith("-p"))
            {
                options.PortNumber = int.Parse(args[1]);
            }
            return options;
        }
    }

    public class Options
    {
        public int PortNumber { get; set; } = 5555;
    }
}
