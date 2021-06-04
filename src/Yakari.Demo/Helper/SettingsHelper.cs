using System;
using Microsoft.Extensions.Configuration;

namespace Yakari.Demo.Helper
{
    public class SettingsHelper
    {
        static string _env = "Development";
        
        // => _env ?? (_env = Environment.GetEnvironmentVariable("env"));
        static string Env => _env;

        public static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Env}.json", optional: true);

                builder.AddEnvironmentVariables();
                return builder.Build();
            }
        }

        public static string Redis => Configuration["redis"];
    }
}