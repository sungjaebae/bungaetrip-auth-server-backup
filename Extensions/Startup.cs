using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Extensions
{
    public static class Startup
    {
        public static IConfigurationSection SetupConfiguration(IServiceCollection services)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            return appSettingsSection;
        }

    }
}
