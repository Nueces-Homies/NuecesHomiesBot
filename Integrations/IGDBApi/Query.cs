namespace Integrations.IGDBApi;

public class Query
{
    private readonly List<string> fields = new();
    private readonly List<string> exclude = new();

    private string? whereClause;
    private string? limitClause;
    private string? offsetClause;
    private string? sortClause;
    private string? searchClause;

    public override string ToString() => BuildQuery();

    public static implicit operator string(Query query) => query.ToString();

    public Query Select(params string[] toInclude)
    {
        this.fields.AddRange(toInclude);
        return this;
    }

    public Query Exclude(params string[] toExclude)
    {
        this.exclude.AddRange(toExclude);
        return this;
    }
    
    public Query Where(string clause)
    {
        if (this.whereClause == null)
        {
            this.whereClause = $"where {clause}";
            return this;
        }

        return this.And(clause);
    }
    
    public Query Where<T1>(string clause, T1? value)
    {
        return this.Where(string.Format(clause, Utils.ConvertValue(value)));
    }
    
    public Query Where(string clause, params object[] values)
    {
        object[] convertedValues = values.Select(Utils.ConvertValue).ToArray()!;
        return this.Where(string.Format(clause, convertedValues));
    }

    public Query And(string clause)
    {
        if (this.whereClause == null) throw new InvalidOperationException("Call Where first");

        this.whereClause += $" & {clause}";
        return this;
    }
    
    public Query And<T1>(string clause, T1? value)
    {
        return this.And(string.Format(clause, Utils.ConvertValue(value)));
    }
    
    public Query And(string clause, params object[] values)
    {
        object[] convertedValues = values.Select(Utils.ConvertValue).ToArray()!;
        return this.And(string.Format(clause, convertedValues));
    }

    public Query Or(string clause)
    {
        if (this.whereClause == null) throw new InvalidOperationException("Call Where first");

        this.whereClause += $" | {clause}";
        return this;
    }
    
    public Query Or<T1>(string clause, T1? value)
    {
        return this.Or(string.Format(clause, Utils.ConvertValue(value)));
    }
    
    public Query Or(string clause, params object[] values)
    {
        object[] convertedValues = values.Select(Utils.ConvertValue).ToArray()!;
        return this.Or(string.Format(clause, convertedValues));
    }

    public Query Limit(int count)
    {
        if (this.limitClause != null) throw new InvalidOperationException("Limit already set");
        this.limitClause = $"limit {count}; ";
        return this;
    }

    public Query Offset(int offset)
    {
        if (this.offsetClause != null) throw new InvalidOperationException("Limit already set");
        this.offsetClause = $"offset {offset}; ";
        return this;
    }

    public Query SortBy(string field, bool ascending)
    {
        if (this.sortClause != null) throw new InvalidOperationException("Sort already set");

        string direction = ascending ? "asc" : "desc"; 
        this.sortClause = $"sort {field} {direction}; ";
        return this;
    }

    public Query Search(string query)
    {
        if (this.searchClause != null) throw new InvalidOperationException("Search already set");

        this.searchClause = $"""search {Utils.ConvertValue(query)};""";
        return this;
    }

    public string BuildQuery()
    {
        var query = "";

        if (
            this.fields.Count == 0 && 
            (this.whereClause != null || this.limitClause != null || this.offsetClause != null || this.searchClause != null || this.sortClause != null))
        {
            this.fields.Add("*");
        }
        
        if (this.fields.Count > 0)
        {
            var fieldsList = string.Join(", ", this.fields);
            query += $"fields {fieldsList}; ";
        }
        
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
}

file static class Utils
{
    public static object ConvertValue<TValue>(TValue? value)
    {
        if (value == null)
        {
            return "null";
        }
        
        object convertedValue = value;
        
        Type runtimeType = value.GetType();
        if (runtimeType.IsEnum)
        {
            convertedValue = Convert.ChangeType(value, Enum.GetUnderlyingType(runtimeType));
        }
        
        if (convertedValue is string)
        {
            return $"\"{convertedValue}\"";
        }

        return convertedValue;
    }
}

public static class StringQueryExtensions
{
    public static string IsAll<T>(this string field, IEnumerable<T> values)
    {
        string list = string.Join(", ", values.Select(Utils.ConvertValue));
        string clause = $"{field} = [{list}]";
        return clause;
    }

    public static string IsNoneOf<T>(this string field, IEnumerable<T> values)
    {
        string list = string.Join(", ", values.Select(Utils.ConvertValue));
        string clause = $"{field} = ![{list}]";
        return clause;
    }
    
    public static string IsAny<T>(this string field, IEnumerable<T> values)
    {
        string list = string.Join(", ", values.Select(Utils.ConvertValue));
        string clause = $"{field} = ({list})";
        return clause;
    }
    
    public static string IsMissingAny<T>(this string field, IEnumerable<T> values)
    {
        string list = string.Join(", ", values.Select(Utils.ConvertValue));
        string clause = $"{field} = !({list})";
        return clause;
    }
    
    
    public static string Exactly<T>(this string field, IEnumerable<T> values)
    {
        string list = string.Join(", ", values.Select(Utils.ConvertValue));
        string clause = $"{field} = {{{list}}}";
        return clause;
    }
}