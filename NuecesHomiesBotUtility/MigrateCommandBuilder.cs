using System.CommandLine;
using Database;

namespace NuecesHomiesBotUtility;

public static class MigrateCommandBuilder
{
    public static Command BuildMigrateCommand(Func<string> dbPathFactory)
    {
        var command = new Command("migrate", "Run database migrations");

        var pathOption = new Option<string>("--path", "The path to the sqlite db");
        pathOption.SetDefaultValueFactory(dbPathFactory);

        var testOption = new Option<bool>("--test", "Whether test data should be generated");
        testOption.SetDefaultValue(false);

        command.AddOption(pathOption);
        command.AddOption(testOption);

        command.SetHandler((path, isTest) =>
        {
            var result = MigratorBuilder.GetMigrator(path, isTest)
                .LogToConsole()
                .Build()
                .PerformUpgrade();

            if (result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Console.ResetColor();
            }
    
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
        }, pathOption, testOption);

        return command;
    }
}