using System;
using Microsoft.Extensions.Configuration;

namespace Yakari.Demo.Helper
{
    public class SettingsHelper
    {
        static string _env;
        static string Env => _env ?? (_env = Environment.GetEnvironmentVariable("env"));

        static bool IsDevelopment => Env == "Dev";

        public static IConfigurationRoot Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Env}.json", optional: true);

                if (IsDevelopment) builder.AddUserSecrets();

                builder.AddEnvironmentVariables();
                return builder.Build();
            }
        }

        public static string Redis => Configuration["redis"];
    }
}