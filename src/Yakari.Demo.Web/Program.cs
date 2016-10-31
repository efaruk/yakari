using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Yakari.Demo.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var host = new WebHostBuilder()
                .UseKestrel()
                //.UseUrls("http://localhost:8880", "http://localhost:8881", "http://localhost:8882", "http://localhost:8883", "http://localhost:8884", "http://localhost:8885", "http://localhost:8886", "http://localhost:8887", "http://localhost:8888", "http://localhost:8889")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
