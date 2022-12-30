namespace Integrations.Test;

using FluentAssertions;
using IGDBApi;

public class QueryTests
{
    private enum TestEnum
    {
        Foo = 1,
        Bar = 2,
    }
    
    [Fact]
    public void SelectSingleFieldTest()
    {
        var qb = new Query();
        string query = qb.Select("name");
        query.Should().Be("fields name;");
    }
    
    [Fact]
    public void SelectMultipleFieldsTest()
    {
        var qb = new Query();
        string query = qb.Select("name", "date");
        query.Should().Be("fields name, date;");
    }

    [Fact]
    public void ExcludeSingleFieldTest()
    {
        var qb = new Query();
        string query = qb.Exclude("name");
        query.Should().Be("exclude name;");
    }
    
    [Fact]
    public void ExcludeMultipleFieldTest()
    {
        var qb = new Query();
        string query = qb.Exclude("name", "date");
        query.Should().Be("exclude name, date;");
    }

    [Fact]
    public void SimpleWhereClauseTest()
    {
        var qb = new Query();
        string query = qb.Where("id = 5");
        query.Should().Be("fields *; where id = 5;");
    }

    [Fact]
    public void StringValueWhereClauseTest()
    {
        var qb = new Query();
        string query = qb.Where("name = {0}", "Halo");
        query.Should().Be("fields *; where name = \"Halo\";");
    }

    [Fact]
    public void EnumValueWhereClauseTest()
    {
        var qb = new Query();
        string query = qb.Where("value = {0}", TestEnum.Bar);
        query.Should().Be("fields *; where value = 2;");
    }

    [Fact]
    public void MultipleValueWhereClauseTest()
    {
        var qb = new Query();
        string query = qb
            .Where("name = {0} & id = {1} & value = {2}", "Halo", 42, TestEnum.Foo);

        query.Should().Be("fields *; where name = \"Halo\" & id = 42 & value = 1;");
    }

    [Fact]
    public void MultipleWhereClausesBecomeAnd()
    {
        string query = new Query().Where("id = 42").Where("value = 5");
        query.Should().Be("fields *; where id = 42 & value = 5;");
    }

    [Fact]
    public void OnlyAndThrowsException()
    {
        Action action = () => new Query().And("id = 5");
        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void SingleAndWorks()
    {
        string query = new Query().Where("id = 5").And("value = 42");
        query.Should().Be("fields *; where id = 5 & value = 42;");
    }

    [Fact]
    public void MultipleAndsWork()
    {
        string query = new Query().Where("id = 5").And("value = 42").And("name = {0}", "Mario");
        query.Should().Be("""fields *; where id = 5 & value = 42 & name = "Mario";""");
    }

    [Fact]
    public void SingleAndWithTwoParamsWorks()
    {
        string query = new Query().Where("id = 5").And("x = {0} & y = {1}", 10, 12);
        query.Should().Be("""fields *; where id = 5 & x = 10 & y = 12;""");
    }
    
    [Fact]
    public void OnlyOrThrowsException()
    {
        Action action = () => new Query().Or("id = 5");
        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void SingleOrWorks()
    {
        string query = new Query().Where("id = 5").Or("value = 42");
        query.Should().Be("fields *; where id = 5 | value = 42;");
    }

    [Fact]
    public void MultipleOrsWork()
    {
        string query = new Query().Where("id = 5").Or("value = 42").Or("name = {0}", "Mario");
        query.Should().Be("""fields *; where id = 5 | value = 42 | name = "Mario";""");
    }

    [Fact]
    public void SingleOrWithTwoParamsWorks()
    {
        string query = new Query().Where("id = 5").Or("x = {0} | y = {1}", 10, 12);
        query.Should().Be("""fields *; where id = 5 | x = 10 | y = 12;""");
    }

    [Fact]
    public void LimitClauseWorks()
    {
        string query = new Query().Limit(10);
        query.Should().Be("fields *; limit 10;");
    }
    
    [Fact]
    public void MultipleLimitClausesThrowsException()
    {
        var action = () => new Query().Limit(10).Limit(100);
        action.Should().ThrowExactly<InvalidOperationException>("because multiple limit clauses are not allowed");
    }
    
    [Fact]
    public void SearchClauseWorks()
    {
        string query = new Query().Search("Halo");
        query.Should().Be("fields *; search \"Halo\";");
    }
    
    [Fact]
    public void MultipleSearchClausesThrowsException()
    {
        var action = () => new Query().Search("Halo").Search("Mario");
        action.Should().ThrowExactly<InvalidOperationException>("because multiple search clauses are not allowed");
    }
    
    [Fact]
    public void OffsetClauseWorks()
    {
        string query = new Query().Offset(10);
        query.Should().Be("fields *; offset 10;");
    }
    
    [Fact]
    public void MultipleOffsetClausesThrowsException()
    {
        var action = () => new Query().Offset(10).Offset(100);
        action.Should().ThrowExactly<InvalidOperationException>("because multiple offset clauses are not allowed");
    }

    [Fact]
    public void SortDescendingWorks()
    {
        string query = new Query().SortBy("release_dates.date", false);
        query.Should().Be("fields *; sort release_dates.date desc;");
    }
    
    [Fact]
    public void SortAscendingWorks()
    {
        string query = new Query().SortBy("release_dates.date", true);
        query.Should().Be("fields *; sort release_dates.date asc;");
    }

    [Fact]
    public void MultipleSortsShouldThrowException()
    {
        Action action = () => new Query().SortBy("name", true).SortBy("first_release_date", false);
        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void IsAllOperatorWorks()
    {
        string term = "genre".IsAll(new [] { "a", "b", "c" });
        term.Should().Be("""genre = ["a", "b", "c"]""");
    }
    
    [Fact]
    public void IsNoneOfOperatorWorks()
    {
        string term = "genre".IsNoneOf(new [] { "a", "b", "c" });
        term.Should().Be("""genre = !["a", "b", "c"]""");
    }
    
    [Fact]
    public void IsAnyOperatorWorks()
    {
        string term = "genre".IsAny(new [] { "a", "b", "c" });
        term.Should().Be("""genre = ("a", "b", "c")""");
    }
    
    [Fact]
    public void IsMissingAnyOperatorWorks()
    {
        string term = "genre".IsMissingAny(new [] { "a", "b", "c" });
        term.Should().Be("""genre = !("a", "b", "c")""");
    }
    
    [Fact]
    public void ExactlyOperatorWorks()
    {
        string term = "genre".Exactly(new [] { "a", "b", "c" });
        term.Should().Be("""genre = {"a", "b", "c"}""");
    }
}