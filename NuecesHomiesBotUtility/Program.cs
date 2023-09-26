using System.CommandLine;
using Core;
using Core.Features;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

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


var rootCommand = new RootCommand();
rootCommand.AddCommand(CreatePingCommand());
return await rootCommand.InvokeAsync(args);

