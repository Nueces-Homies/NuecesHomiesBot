using System.Reflection;
using Database;
using Integrations.IGDBApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Module
{
    private static IServiceCollection AddConfiguration(this IServiceCollection serviceCollection)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();

        return serviceCollection.AddSingleton<IConfiguration>(config);
    }
    
    private static IServiceCollection AddExternalIntegrations(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IIGDB>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                var clientId = config["TWITCH_CLIENT_ID"] ?? throw new Exception("TWITCH_CLIENT_ID missing");
                var clientSecret = config["TWITCH_CLIENT_SECRET"] ??
                                   throw new Exception("TWITCH_CLIENT_SECRET missing");
                return new IGDB(clientId, clientSecret);
            });
    }

    private static IServiceCollection AddDataDependencies(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddDatabaseDependencies();
    }

    public static IServiceCollection AddNuecesHomiesCoreDependencies(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddConfiguration()
            .AddExternalIntegrations()
            .AddDataDependencies()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}