namespace Integrations.IGDBApi;

public class QueryBuilder<T>
{
    private readonly IGDBClient client;
    private readonly string endpoint;

    private readonly List<string> fields = new();
    private readonly List<string> exclude = new();

    private string? whereClause;
    private string? limitClause;
    private string? offsetClause;
    private string? sortClause;
    private string? searchClause;

    public QueryBuilder(string endpoint, IGDBClient igdbClient)
    {
        this.client = igdbClient;
        this.endpoint = endpoint;
    }

    public QueryBuilder<T> Select(params string[] toInclude)
    {
        this.fields.AddRange(toInclude);
        return this;
    }

    public QueryBuilder<T> Exclude(params string[] toExclude)
    {
        this.exclude.AddRange(toExclude);
        return this;
    }
    
    public QueryBuilder<T> Where(string clause)
    {
        this.whereClause = $"where {clause}";
        return this;
    }

    public QueryBuilder<T> And(string clause)
    {
        if (this.whereClause == null) throw new InvalidOperationException("Call Where first");

        this.whereClause += $" & {clause}";
        return this;
    }

    public QueryBuilder<T> Or(string clause)
    {
        if (this.whereClause == null) throw new InvalidOperationException("Call Where first");

        this.whereClause += $" | {clause}";
        return this;
    }

    public QueryBuilder<T> Limit(int count)
    {
        if (this.limitClause != null) throw new InvalidOperationException("Limit already set");
        this.limitClause = $"limit {count}; ";
        return this;
    }

    public QueryBuilder<T> Offset(int offset)
    {
        if (this.offsetClause != null) throw new InvalidOperationException("Limit already set");
        this.offsetClause = $"offset {offset}; ";
        return this;
    }

    public QueryBuilder<T> SortBy(string field, bool ascending)
    {
        if (this.sortClause != null) throw new InvalidOperationException("Sort already set");

        string direction = ascending ? "asc" : "desc"; 
        this.sortClause = $"sort {field} {direction}; ";
        return this;
    }

    public QueryBuilder<T> Search(string query)
    {
        if (this.searchClause != null) throw new InvalidOperationException("Search already set");

        this.searchClause = $"""search "{query}"; """;
        return this;
    }

    public string BuildQuery()
    {
        var query = "";

        if (this.fields.Count == 0 && this.whereClause != null)
        {
            this.fields.Add("*");
        }
        
        var fieldsList = string.Join(", ", this.fields);
        query += $"fields {fieldsList}; ";

        if (this.exclude.Count > 0)
        {
            var list = string.Join(", ", this.exclude);
            query += $"exclude {list}; ";
        }

        if (this.whereClause != null)
        {
            query += this.whereClause + "; ";
        }

        if (this.searchClause != null)
        {
            query += this.searchClause;
        }

        if (this.offsetClause != null)
        {
            query += this.offsetClause;
        }

        if (this.limitClause != null)
        {
            query += this.limitClause;
        }

        if (this.sortClause != null)
        {
            query += this.sortClause;
        }

        return query.Trim();
    }

    public async Task<T> ExecuteAsync()
    {
        string query = BuildQuery();
        T result = await this.client.QueryEndpointAsync<T>(this.endpoint, query);
        return result;
    }
}

public static class StringQueryExtensions
{
    public static string IsAll(this string field, params string[] values)
    {
        string list = string.Join(", ", values);
        string clause = $"{field} = [{list}]";
        return clause;
    }
    
    public static string IsNoneOf(this string field, params string[] values)
    {
        string list = string.Join(", ", values);
        string clause = $"{field} = ![{list}]";
        return clause;
    }
    
    public static string IsAny(this string field, params string[] values)
    {
        string list = string.Join(", ", values);
        string clause = $"{field} = ({list})";
        return clause;
    }
    
    public static string IsMissingAny(this string field, params string[] values)
    {
        string list = string.Join(", ", values);
        string clause = $"{field} = !({list})";
        return clause;
    }
    
    public static string Exactly(this string field, params string[] values)
    {
        string list = string.Join(", ", values);
        string clause = $"{field} = {{{list}}}";
        return clause;
    }
}