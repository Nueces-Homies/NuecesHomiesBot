using System.Data;
using Dapper;
using Database.TypeHandlers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Database;

using Microsoft.Extensions.DependencyInjection;

public static class Module
{
    public static IServiceCollection AddDatabaseDependencies(this IServiceCollection serviceCollection)
    {
        SqlMapper.AddTypeHandler(new HumanTimeTypeHandler());
        SqlMapper.AddTypeHandler(new CrystalTypeHandler());
        
        return serviceCollection.AddScoped<IDbConnection>(
            serviceProvider =>
            {
                var path = serviceProvider.GetRequiredService<IConfiguration>()["DATABASE_PATH"];
                var connectionString = $"Data Source={path}";
                return new SqliteConnection(connectionString);
            });
    }
}