using Microsoft.EntityFrameworkCore.Migrations;

namespace ErisHub.DiscordBot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewcomerConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NewcomerRoleId = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewcomerConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewcomerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(nullable: false),
                    MessageId = table.Column<ulong>(nullable: false),
                    RoleId = table.Column<ulong>(nullable: false),
                    EmoteId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewcomerSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NewcomerConfigs",
                columns: new[] { "Id", "NewcomerRoleId" },
                values: new object[] { 1, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewcomerConfigs");

            migrationBuilder.DropTable(
                name: "NewcomerSettings");
        }
    }
}
