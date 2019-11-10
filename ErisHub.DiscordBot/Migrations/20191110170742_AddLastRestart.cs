using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ErisHub.DiscordBot.Migrations
{
    public partial class AddLastRestart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRestart",
                table: "WatcherSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRestart",
                table: "WatcherSettings");
        }
    }
}
