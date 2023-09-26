// See https://aka.ms/new-console-template for more information

using Core;
using Database;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddNuecesHomiesCoreDependencies()
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton<InteractionService>(services => new InteractionService(services.GetRequiredService<DiscordSocketClient>()))
    .AddTransient<Application>()
    .AddSingleton<IServiceProvider>(services => services)
    .BuildServiceProvider();

var application = serviceProvider.GetRequiredService<Application>();
await application.RunAsync();




