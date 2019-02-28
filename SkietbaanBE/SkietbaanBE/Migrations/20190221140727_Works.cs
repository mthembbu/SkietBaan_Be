using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class Works : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Average",
                table: "UserCompStats");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "UserCompStats");

            migrationBuilder.AddColumn<double>(
                name: "Average",
                table: "UserCompetitionTotalScores",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Average",
                table: "UserCompetitionTotalScores");

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
    }
}
