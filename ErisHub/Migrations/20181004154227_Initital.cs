using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ErisHub.Migrations
{
    public partial class Initital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ckey = table.Column<string>(type: "varchar(255)", nullable: false),
                    registered = table.Column<DateTime>(type: "date", nullable: true),
                    first_seen = table.Column<DateTime>(type: "datetime", nullable: false),
                    last_seen = table.Column<DateTime>(type: "datetime", nullable: false),
                    ip = table.Column<string>(type: "varchar(255)", nullable: false),
                    cid = table.Column<string>(type: "varchar(255)", nullable: false),
                    rank = table.Column<string>(type: "varchar(255)", nullable: false, defaultValueSql: "'player'"),
                    flags = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    byond_version = table.Column<string>(type: "varchar(255)", nullable: false),
                    country = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "polls",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(type: "varchar(16)", nullable: false),
                    start = table.Column<DateTime>(type: "datetime", nullable: false),
                    end = table.Column<DateTime>(type: "datetime", nullable: false),
                    question = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_polls", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "populations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    player_count = table.Column<int>(type: "int(11)", nullable: false),
                    admin_count = table.Column<int>(type: "int(11)", nullable: false),
                    time = table.Column<DateTime>(type: "datetime", nullable: false),
                    server = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_populations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    server = table.Column<string>(type: "varchar(255)", nullable: false),
                    type = table.Column<string>(type: "varchar(255)", nullable: false),
                    ip = table.Column<string>(type: "varchar(255)", nullable: true),
                    cid = table.Column<string>(type: "varchar(255)", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: false),
                    job = table.Column<string>(type: "varchar(255)", nullable: true),
                    duration = table.Column<int>(type: "int(11)", nullable: false),
                    time = table.Column<DateTime>(type: "datetime", nullable: false),
                    target_id = table.Column<int>(type: "int(11)", nullable: false),
                    banned_by_id = table.Column<int>(type: "int(11)", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    unbanned = table.Column<bool?>(type: "tinyint(1)", nullable: true),
                    unbanned_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    unbanned_by_id = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bans", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_20d480679b",
                        column: x => x.banned_by_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rails_62ac37e1e1",
                        column: x => x.target_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rails_a305c9e562",
                        column: x => x.unbanned_by_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    author = table.Column<string>(type: "varchar(255)", nullable: true),
                    title = table.Column<string>(type: "varchar(255)", nullable: true),
                    content = table.Column<string>(type: "varchar(255)", nullable: true),
                    category = table.Column<string>(type: "varchar(255)", nullable: true),
                    author_id = table.Column<int>(type: "int(11)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_53d51ce16a",
                        column: x => x.author_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "poll_options",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    poll_id = table.Column<int>(type: "int(11)", nullable: false),
                    text = table.Column<string>(type: "varchar(255)", nullable: false),
                    min_value = table.Column<int>(type: "int(11)", nullable: true),
                    max_value = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_aa85becb42",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "poll_text_replies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime", nullable: true),
                    poll_id = table.Column<int>(type: "int(11)", nullable: true),
                    player_id = table.Column<int>(type: "int(11)", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll_text_replies", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_ffc8df499f",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rails_0833f4df0b",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "poll_votes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    time = table.Column<DateTime>(type: "datetime", nullable: false),
                    poll_id = table.Column<int>(type: "int(11)", nullable: false),
                    player_id = table.Column<int>(type: "int(11)", nullable: false),
                    option_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_poll_votes", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_826ebfbbb3",
                        column: x => x.option_id,
                        principalTable: "poll_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rails_a3e5a3aede",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rails_a6e6974b7e",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "index_bans_on_banned_by_id",
                table: "bans",
                column: "banned_by_id");

            migrationBuilder.CreateIndex(
                name: "index_bans_on_target_id",
                table: "bans",
                column: "target_id");

            migrationBuilder.CreateIndex(
                name: "index_bans_on_unbanned_by_id",
                table: "bans",
                column: "unbanned_by_id");

            migrationBuilder.CreateIndex(
                name: "index_books_on_author_id",
                table: "books",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_options_on_poll_id",
                table: "poll_options",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_text_replies_on_player_id",
                table: "poll_text_replies",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_text_replies_on_poll_id",
                table: "poll_text_replies",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_votes_on_option_id",
                table: "poll_votes",
                column: "option_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_votes_on_player_id",
                table: "poll_votes",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_votes_on_poll_id",
                table: "poll_votes",
                column: "poll_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bans");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "poll_text_replies");

            migrationBuilder.DropTable(
                name: "poll_votes");

            migrationBuilder.DropTable(
                name: "populations");

            migrationBuilder.DropTable(
                name: "poll_options");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "polls");
        }
    }
}
