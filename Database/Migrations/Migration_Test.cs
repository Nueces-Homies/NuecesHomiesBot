using FluentMigrator;

namespace Database.Migrations;

[Migration(1)]
public class Migration_Test : Migration
{
    public override void Up()
    {
        Create.Table("Games")
            .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("ReleaseTime").AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Games");
    }
}