using System.Data;
using DbUp;
using DbUp.Builder;
using DbUp.SQLite.Helpers;

namespace Database;

public class MigratorBuilder
{
    public static UpgradeEngineBuilder GetMigrator(string dbPath, bool includeTestScripts)
    {
        var connectionString = $"Data Source={dbPath}";

        return DeployChanges.To
            .SQLiteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                typeof(MigratorBuilder).Assembly,
                script => includeTestScripts || !script.EndsWith("_Test.sql"));
    }
    
    public static UpgradeEngineBuilder GetMigrator(IDbConnection connection, bool includeTestScripts)
    {
        var sharedConnection = new SharedConnection(connection);
        
        return DeployChanges.To
            .SQLiteDatabase(sharedConnection)
            .WithScriptsEmbeddedInAssembly(
                typeof(MigratorBuilder).Assembly,
                script => includeTestScripts || !script.EndsWith("_Test.sql"));
    }
}