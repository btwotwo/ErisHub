using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ErisHub.Migrations
{
    public partial class InitialCreate : Migration
    {
        //we're applying this to existing database, so we don't need to execute anything
        protected override void Up(MigrationBuilder migrationBuilder)
        {           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
