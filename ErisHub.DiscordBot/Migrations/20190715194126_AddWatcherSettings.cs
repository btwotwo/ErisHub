using Microsoft.EntityFrameworkCore.Migrations;

namespace ErisHub.DiscordBot.Migrations
{
    public partial class AddWatcherSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatcherSettings",
                columns: table => new
                {
                    Server = table.Column<string>(nullable: false),
                    MentionRoleId = table.Column<ulong>(nullable: false),
                    ChannelId = table.Column<ulong>(nullable: false),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatcherSettings", x => x.Server);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatcherSettings");
        }
    }
}
