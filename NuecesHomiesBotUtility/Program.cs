using System.CommandLine;
using Core;
using Core.Features;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using NuecesHomiesBotUtility;

var serviceProvider = new ServiceCollection()
    .AddNuecesHomiesCoreDependencies()
    .BuildServiceProvider();

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

var config = serviceProvider.GetRequiredService<IConfiguration>();

var rootCommand = new RootCommand();
rootCommand.AddCommand(CreatePingCommand());
rootCommand.AddCommand(MigrateCommandBuilder.BuildMigrateCommand(() => config["DATABASE_PATH"]!));
return await rootCommand.InvokeAsync(args);

