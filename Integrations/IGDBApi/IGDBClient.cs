namespace Integrations.IGDBApi;

using System.Text.Json.Serialization;
using Flurl;
using Flurl.Http;
using ProtoBuf;

public class IGDBClient
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

        IFlurlResponse response;
        try
        {
            response = await IGDBClient.IGDBBaseUrl
                             .AppendPathSegment($"{endpoint}.pb")
                             .WithHeader("Client-ID", this.clientId)
                             .WithOAuthBearerToken(accessToken)
                             .PostStringAsync(query);
        }
        catch (FlurlHttpException httpException)
        {
            if (httpException.StatusCode == 400)
            {
                throw new IGDBBadQueryException(query, endpoint, httpException);
            }
            
            throw new IGDBBadRequestException(query, endpoint, httpException);
        }

        return Serializer.Deserialize<T>(await response.GetStreamAsync());
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

public class IGDBClientException : Exception
{
    public IGDBClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class IGDBBadQueryException : IGDBClientException
{
    public IGDBBadQueryException(string query, string endpoint, FlurlHttpException innerException) 
        : base($"Invalid query for endpoint {endpoint}: {query}",
        innerException)
    {
    }
}
public class IGDBBadRequestException : IGDBClientException
{
    public IGDBBadRequestException(string query, string endpoint, FlurlHttpException innerException) 
        : base($"Unable to complete request to {endpoint} for query: {query}",
        innerException)
    {
    }
}
