namespace Integrations.Test;

using System.Reflection;
using FluentAssertions;
using IGDBApi;
using Microsoft.Extensions.Configuration;

public class IGDBTest
{
    private readonly string clientId;
    private readonly string clientSecret;
    
    public IGDBTest()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();
        
        this.clientId = config["TWITCH_CLIENT_ID"] ?? throw new Exception("TWITCH_CLIENT_ID missing");
        this.clientSecret = config["TWITCH_CLIENT_SECRET"] ?? throw new Exception("TWITCH_CLIENT_SECRET missing");
    }

    [Fact]
    public async Task CanFetchGame()
    {
        var igdb = new IGDBClient(this.clientId, this.clientSecret);
        
        const int crisisCoreReunionId = 205201;
        var games = await igdb.Games.Where($"id = {crisisCoreReunionId}").ExecuteAsync();
        var game = games.First();

        game.Id.Should().Be(crisisCoreReunionId);
        game.Name.Should().Be("Crisis Core: Final Fantasy VII - Reunion");
    }
}