using System.Data;
using Database.Migrations;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Database;

using Microsoft.Extensions.DependencyInjection;

public static class Module
{
    public static IServiceCollection AddDatabaseDependencies(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<IDbConnection>(
            serviceProvider =>
            {
                var connectionString = serviceProvider.GetRequiredService<IConfiguration>()["DATABASE_PATH"];
                return new SqliteConnection(connectionString);
            });
    }
    
    public static IServiceCollection AddMigratorDependencies(this IServiceCollection serviceCollection, string profile="")
    {
        return serviceCollection
            .AddFluentMigratorCore()
            .AddSingleton<IConnectionStringReader>(serviceProvider =>
            {
                var connectionString = serviceProvider.GetRequiredService<IConfiguration>()["DATABASE_PATH"];
                return new PassThroughConnectionStringReader(connectionString);
            })
            .ConfigureRunner(runner => runner
                .AddSQLite()
                .ScanIn(typeof(Migration_Test).Assembly).For.Migrations())
            .Configure<RunnerOptions>(cfg =>
            {
                 cfg.Profile = profile;
            })
            .AddLogging(logging => logging.AddFluentMigratorConsole());
    }
}