using Microsoft.Extensions.Configuration;

namespace Common.Helper.Helper
{
    public static class ConfigurationHelper
    {
        private static IConfiguration? _configuration;
        public static void Configure(IConfiguration? configuration)
        {
            _configuration = configuration;
        }

        public static IConfiguration? Current => _configuration;
    }
}
