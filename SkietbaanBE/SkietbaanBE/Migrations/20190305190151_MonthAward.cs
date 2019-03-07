using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class MonthAward : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompetitionId",
                table: "Awards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Awards_CompetitionId",
                table: "Awards",
                column: "CompetitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Competitions_CompetitionId",
                table: "Awards",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Competitions_CompetitionId",
                table: "Awards");

            migrationBuilder.DropIndex(
                name: "IX_Awards_CompetitionId",
                table: "Awards");

            migrationBuilder.DropColumn(
                name: "CompetitionId",
                table: "Awards");
        }
    }
}
