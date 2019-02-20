using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class RemoveCompScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompScore",
                table: "UserCompStats");

            migrationBuilder.AddColumn<int>(
                name: "BestScoresNumber",
                table: "Competitions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestScoresNumber",
                table: "Competitions");

            migrationBuilder.AddColumn<int>(
                name: "CompScore",
                table: "UserCompStats",
                nullable: false,
                defaultValue: 0);
        }
    }
}
