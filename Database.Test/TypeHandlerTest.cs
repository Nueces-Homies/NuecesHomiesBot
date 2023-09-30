
using FluentAssertions;

namespace Database.Test;

using Dapper;
using Microsoft.Data.Sqlite;

using TypeHandlers;
using Entities;


public class TypeHandlerTest
{
    private SqliteConnection DbConnection { get; }
    
    public TypeHandlerTest()
    {
        SqlMapper.AddTypeHandler(new HumanTimeTypeHandler());
        SqlMapper.AddTypeHandler(new CrystalTypeHandler());
        
        DbConnection = new SqliteConnection("Data Source=:memory:");
        DbConnection.Open();
        
        MigratorBuilder
            .GetMigrator(DbConnection, includeTestScripts: false)
            .Build()
            .PerformUpgrade();
    }
    
    [Fact]
    public void WriteWithDateTime()
    {
        var expected = new Game
        {
            Id = Crystal.New(),
            ReleaseTime = HumanTime.HumanTime.DateTime(DateTimeOffset.UtcNow),
        };

        var insertSql = "INSERT INTO Games (Id, ReleaseTime) VALUES (@Id, @ReleaseTime)";
        this.DbConnection.Execute(insertSql, expected);

        var querySql = "SELECT * FROM Games WHERE Id = @Id";
        var actual = this.DbConnection.QuerySingle<Game>(querySql, expected);
        
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void WriteWithDate()
    {
        var expected = new Game
        {
            Id = Crystal.New(),
            ReleaseTime = HumanTime.HumanTime.Date(new DateOnly(2024,5,1))
        };

        var insertSql = "INSERT INTO Games (Id, ReleaseTime) VALUES (@Id, @ReleaseTime)";
        this.DbConnection.Execute(insertSql, expected);

        var querySql = "SELECT * FROM Games WHERE Id = @Id";
        var actual = this.DbConnection.QuerySingle<Game>(querySql, expected);
        
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WriteWithWindow()
    {
        var expected = new Game
        {
            Id = Crystal.New(),
            ReleaseTime = HumanTime.HumanTime.Month(2024, 5)
        };

        var insertSql = "INSERT INTO Games (Id, ReleaseTime) VALUES (@Id, @ReleaseTime)";
        this.DbConnection.Execute(insertSql, expected);

        var querySql = "SELECT * FROM Games WHERE Id = @Id";
        var actual = this.DbConnection.QuerySingle<Game>(querySql, expected);
        
        actual.Should().BeEquivalentTo(expected);
    }
}