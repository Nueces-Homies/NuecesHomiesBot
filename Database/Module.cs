﻿using System.Data;
using Database.Migrations;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Data.Sqlite;

namespace Database;

using Microsoft.Extensions.DependencyInjection;

public static class Module
{
    public static IServiceCollection AddDatabaseDependencies(this IServiceCollection serviceCollection, string connectionString)
    {
        return serviceCollection.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));
    }
    
    public static IServiceCollection AddMigratorDependencies(this IServiceCollection serviceCollection,
        string connectionString, string profile="")
    {
        return serviceCollection
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Migration_Test).Assembly).For.Migrations())
            .Configure<RunnerOptions>(cfg =>
            {
                 cfg.Profile = profile;
            });
            // .AddLogging(logging => logging.AddFluentMigratorConsole());
    }
}