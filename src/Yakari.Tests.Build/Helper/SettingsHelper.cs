using System.Configuration;

namespace Yakari.Tests.Helper
{
    public class SettingsHelper
    {
        public static string Redis => ConfigurationManager.AppSettings["redis"];
    }
}