using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;

namespace DiscordBot;

public class Application
{
    private readonly IMigrationRunner migrationRunner;
    
    private readonly IConfiguration configuration;
    private readonly DiscordSocketClient discordClient;
    private readonly InteractionService interactionService;
    
    // Necessary to pass to Interaction Service setup
    private readonly IServiceProvider serviceProvider;


    public Application(IConfiguration configuration, DiscordSocketClient discordClient, InteractionService interactionService, IServiceProvider serviceProvider, IMigrationRunner migrationRunner)
    {
        this.configuration = configuration;
        this.discordClient = discordClient;
        this.interactionService = interactionService;
        this.serviceProvider = serviceProvider;
        this.migrationRunner = migrationRunner;
    }

    public async Task RunAsync()
    {
        migrationRunner.MigrateUp();
        
        var token = configuration["DISCORD_TOKEN"] ?? throw new Exception("No discord token found");

        interactionService.Log += async message => Console.WriteLine(message.Message);
        discordClient.Log += async message => Console.WriteLine(message.Message);

        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        discordClient.Ready += OnDiscordClientReady;
        discordClient.InteractionCreated += HandleInteraction;

        await discordClient.LoginAsync(TokenType.Bot, token);
        await discordClient.StartAsync();
        await Task.Delay(-1);
    }

    private async Task OnDiscordClientReady()
    {
        var guild = ulong.Parse(configuration["GUILD_ID"] ?? throw new Exception("No guild id found"));
        await interactionService.RegisterCommandsToGuildAsync(guild, deleteMissing: true);
    }
    
    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(discordClient, interaction);

            // Execute the incoming command.
            await interactionService.ExecuteCommandAsync(context,serviceProvider);
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }
}