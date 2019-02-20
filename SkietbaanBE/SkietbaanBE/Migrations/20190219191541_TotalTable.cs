using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class TotalTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "UserCompStats");

            migrationBuilder.CreateTable(
                name: "UserCompetitionTotalScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompetitionId = table.Column<int>(nullable: true),
                    Total = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompetitionTotalScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompetitionTotalScores_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCompetitionTotalScores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCompetitionTotalScores_CompetitionId",
                table: "UserCompetitionTotalScores",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompetitionTotalScores_UserId",
                table: "UserCompetitionTotalScores",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCompetitionTotalScores");

            migrationBuilder.AddColumn<int>(
                name: "Total",
                table: "UserCompStats",
                nullable: false,
                defaultValue: 0);
        }
    }
}
