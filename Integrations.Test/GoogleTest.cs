using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Integrations.Test;

public class GoogleTests
{
    private string credentialsJson;
    
    public GoogleTests()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
            .AddEnvironmentVariables()
            .Build();

        var base64String = config["GOOGLE_CREDENTIALS"] ?? throw new Exception("Could not find GOOGLE_CREDENTIALS");
        byte[] data = Convert.FromBase64String(base64String);
        this.credentialsJson = System.Text.Encoding.Default.GetString(data);
    }
    
    [Fact]
    public async Task CanGetCalendarEvents()
    {
        var google = new GoogleApi.Google(this.credentialsJson);

        // Summer Games Fest calendar
        const string calendarId = "s71id26u0afr69leltrq0us0b97jp35k@import.calendar.google.com";
        var result = await google.GetEventsAsync(calendarId);

        result.Events.Should().NotBeEmpty();
        result.Updated.Year.Should().BeGreaterOrEqualTo(2022);
    }

    [Fact]
    public async Task CanLookupBook()
    {
        var google = new GoogleApi.Google(this.credentialsJson);
        const string isbn = "0316440884";
        var result = await google.LookupBookAsync(isbn);

        result.Should().NotBeNull();
        result?.Title.Should().Be("Jade City");
    }
}