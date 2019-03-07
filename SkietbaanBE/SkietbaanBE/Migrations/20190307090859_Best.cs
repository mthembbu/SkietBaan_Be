using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class Best : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Best",
                table: "UserCompStats",
                newName: "MonthBestScore");

            migrationBuilder.AddColumn<int>(
                name: "Best",
                table: "UserCompetitionTotalScores",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Best",
                table: "UserCompetitionTotalScores");

            migrationBuilder.RenameColumn(
                name: "MonthBestScore",
                table: "UserCompStats",
                newName: "Best");
        }
    }
}
