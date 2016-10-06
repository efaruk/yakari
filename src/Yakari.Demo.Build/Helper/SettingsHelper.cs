using System.Configuration;

namespace Yakari.Demo.Helper
{
    public class SettingsHelper
    {
        public static string Redis => ConfigurationManager.AppSettings["redis"];
    }
}