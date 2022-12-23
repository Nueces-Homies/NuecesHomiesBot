namespace Integrations.IGDBApi;

using System.Text.Json.Serialization;
using Flurl;
using Flurl.Http;
using ProtoBuf;

public partial class IGDBClient
{
    private const string TokenUrl = "https://id.twitch.tv/oauth2/token";
    private const string IGDBBaseUrl = "https://api.igdb.com/v4/";
    
    private readonly string clientId;
    private readonly string clientSecret;

    private Token token = default;
    
    public IGDBClient(string clientId, string clientSecret)
    {
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    private async Task<string> GetTokenAsync()
    {
        if (this.token.IsExpired)
        {
            this.token = await IGDBClient.TokenUrl.SetQueryParams(
                 new
                 {
                     client_id = this.clientId,
                     client_secret = this.clientSecret,
                     grant_type = "client_credentials",
                 })
             .PostAsync()
             .ReceiveJson<Token>();
        }

        return this.token.AccessToken;
    }

    public async Task<T> QueryEndpointAsync<T>(string endpoint, string query)
    {
        var accessToken = await this.GetTokenAsync();

        var result = await IGDBClient.IGDBBaseUrl
                         .AppendPathSegment($"{endpoint}.pb")
                         .WithHeader("Client-ID", this.clientId)
                         .WithOAuthBearerToken(accessToken)
                         .PostStringAsync(query);

        if (result.StatusCode >= 400)
        {
            throw new IGDBClientException(result.StatusCode, await result.GetStringAsync());
        }

        return Serializer.Deserialize<T>(await result.GetStreamAsync());
    }

    private readonly record struct Token(
        [property: JsonPropertyName("access_token")]
        string AccessToken,
        [property: JsonPropertyName("expires_in")]
        int ExpirationTimestamp,
        [property: JsonPropertyName("token_type")]
        string TokenType
    )
    {
        public bool IsExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= this.ExpirationTimestamp;
    }
}

internal class IGDBClientException : Exception
{
    private readonly int statusCode;
    private readonly string body;
    
    public IGDBClientException(int statusCode, string data) : base($"IGDB Request Failed. Status Code={statusCode}")
    {
        this.statusCode = statusCode;
        this.body = data;
    }
}