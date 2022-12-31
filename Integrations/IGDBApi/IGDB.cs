namespace Integrations.IGDBApi;

public interface IIGDB
{
    Task<Game?> GetGameAsync(ulong igdbId);
    Task<IList<Game>> SearchForGamesAsync(string query);
    Task<IList<Game>> GetGamesAsync(IEnumerable<ulong> igdbIds);
}

/// <summary>
/// Wrapper class around IGDBClient to abstract certain queries
/// </summary>
public class IGDB : IIGDB
{
    private readonly IGDBClient client;

    private readonly Query searchBaseQuery;
    private readonly Query lookupBaseQuery;
    private readonly Query bulkLookupBaseQuery;

    public IGDB(string clientId, string clientSecret)
    {
        this.client = new IGDBClient(clientId, clientSecret);

        this.searchBaseQuery = new Query()
            .Select("id", "name")
            .Select("release_dates.human", "release_dates.category", "release_dates.date")
            .Select("involved_companies.company.name", "involved_companies.developer")
            .Offset(0);

        this.lookupBaseQuery = new Query()
            .Select("id", "name", "url", "summary", "first_release_date", "aggregated_rating")
            .Select("genres.name", "themes.name", "player_perspectives.name", "game_modes.name")
            .Select("platforms.name", "cover.url")
            .Select("release_dates.human", "release_dates.category", "release_dates.date",
                "release_dates.platform.name")
            .Select("involved_companies.company.name", "involved_companies.developer");

        this.bulkLookupBaseQuery = new Query()
            .Select("id", "name", "release_dates.human", "release_dates.date", "release_dates.platform.id")
            .Limit(500);
    }

    public async Task<Game?> GetGameAsync(ulong igdbId)
    {
        string query = this.lookupBaseQuery.Where("id = {0}", igdbId);
        var result = await this.client.QueryEndpointAsync<GameResult>("games", query);
        return result.Games.FirstOrDefault();
    }

    public async Task<IList<Game>> SearchForGamesAsync(string searchTerm)
    {
        string query = this.searchBaseQuery.Search(searchTerm);
        var result = await this.client.QueryEndpointAsync<GameResult>("games", query);
        return result.Games;
    }

    public async Task<IList<Game>> GetGamesAsync(IEnumerable<ulong> igdbIds)
    {
        string query = this.bulkLookupBaseQuery.Where("id".IsAny(igdbIds));
        var result = await this.client.QueryEndpointAsync<GameResult>("games", query);
        return result.Games;
    }
}