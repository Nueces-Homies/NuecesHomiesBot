
using FluentAssertions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

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
        string connectionString = $"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared";
        
        SqlMapper.AddTypeHandler(new HumanTimeTypeHandler());
        SqlMapper.AddTypeHandler(new CrystalTypeHandler());
        DbConnection = new SqliteConnection(connectionString);

        // Run migration to create database and initial tables
        new ServiceCollection()
            .AddMigratorDependencies(connectionString, profile:"Test")
            .BuildServiceProvider(false)
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
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