using System.CommandLine;
using Core;
using Core.Features;
using Database;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using MediatR;

string connectionString = $"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared";

var serviceProvider = new ServiceCollection()
    .AddNuecesHomiesCoreDependencies()
    .AddDatabaseDependencies(connectionString)
    .AddMigratorDependencies(connectionString)
    .BuildServiceProvider();

Console.WriteLine("Migrating database...");
var migrationRunner = serviceProvider.GetService<IMigrationRunner>();
migrationRunner.MigrateUp();

Command CreatePingCommand()
{
    var pingCommand = new Command("ping", "Test command");
    
    var messageOption = new Option<string>(
        name: "--message",
        description: "A message to echo back");
    pingCommand.AddOption(messageOption);
    
    pingCommand.SetHandler(async message =>
    {
        var request = new PingRequest { Message = message };
        var response = await serviceProvider.GetService<IMediator>()?.Send(request)!;
        Console.WriteLine(response);
    }, messageOption);

    return pingCommand;
}


var rootCommand = new RootCommand();
rootCommand.AddCommand(CreatePingCommand());
return await rootCommand.InvokeAsync(args);

