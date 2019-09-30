using Microsoft.EntityFrameworkCore.Migrations;

namespace ErisHub.DiscordBot.Migrations
{
    public partial class AddWatching : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Watching",
                table: "WatcherSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Watching",
                table: "WatcherSettings");
        }
    }
}
