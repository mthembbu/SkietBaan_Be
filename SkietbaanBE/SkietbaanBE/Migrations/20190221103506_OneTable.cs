using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class OneTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BestScore",
                table: "UserCompStats",
                newName: "Best");

            migrationBuilder.AddColumn<int>(
                name: "Average",
                table: "UserCompStats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Total",
                table: "UserCompStats",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Average",
                table: "UserCompStats");

            migrationBuilder.DropColumn(
                name: "Best",
                table: "UserCompStats");

            migrationBuilder.RenameColumn(
                name: "Best",
                table: "UserCompStats",
                newName: "BestScore");
        }
    }
}
