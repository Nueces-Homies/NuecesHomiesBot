using System.CommandLine;
using Core;
using Core.Features;
using Core.Features.Movies;
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
        var response = await serviceProvider.GetRequiredService<IMediator>().Send(request)!;
        Console.WriteLine(response);
    }, messageOption);

    return pingCommand;
}

Command CreateMoviesCommand()
{
    var moviesCommand = new Command("movies", "Movie related operations");

    var titleOption = new Option<string>(name: "--title", description: "The title to search for");

    var trackCommand = new Command("track", "Track a movie");
    trackCommand.AddOption(titleOption);
    trackCommand.SetHandler(async title =>
    {
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var searchRequest = new SearchForMovieRequest { Title = title };
        var searchResults = await mediator.Send(searchRequest);

        foreach (var movie in searchResults)
        {
            Console.WriteLine($"{movie.Id,7}: {movie.Title} ({movie.Year})");
        }
        
        var idString = Console.ReadLine();
        int.TryParse(idString, out var tmdbId);

        var trackRequest = new TrackMovieRequest { TmdbId = tmdbId, DiscordChannelId = 791827981264879626 };
        bool added = await mediator.Send(trackRequest);

        string message = added ? $"Tracked movie {tmdbId}" : $"Didn't add movie {tmdbId}";
        Console.WriteLine(message);

    }, titleOption);
    
    moviesCommand.AddCommand(trackCommand);
    return moviesCommand;
}

var config = serviceProvider.GetRequiredService<IConfiguration>();

var rootCommand = new RootCommand();
rootCommand.AddCommand(CreatePingCommand());
rootCommand.AddCommand(CreateMoviesCommand());
rootCommand.AddCommand(MigrateCommandBuilder.BuildMigrateCommand(() => config["DATABASE_PATH"]!));
return await rootCommand.InvokeAsync(args);

