using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class NewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_UserCompetitionTotalScores_Competitions_CompetitionId",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompetitionTotalScores_Users_UserId",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserCompStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCompetitionTotalScores",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropIndex(
                name: "IX_UserCompetitionTotalScores_CompetitionId",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserCompetitionTotalScores");*/

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Users",
                nullable: true);

           /* migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserGroups",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "UserGroups",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserCompetitionTotalScores",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Total",
                table: "UserCompetitionTotalScores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CompetitionId",
                table: "UserCompetitionTotalScores",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Best",
                table: "UserCompetitionTotalScores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "UserCompetitionTotalScores",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "UserScore",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "TimeOfArrival",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Competitions",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "Competitions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Awards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Awards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCompetitionTotalScores",
                table: "UserCompetitionTotalScores",
                columns: new[] { "CompetitionId", "UserId" });

            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    OTPExpiry = table.Column<DateTime>(nullable: false),
                    OneTimePassword = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_OTPs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Accuracy = table.Column<double>(nullable: false),
                    CompID = table.Column<int>(nullable: false),
                    CompetitionId = table.Column<int>(nullable: true),
                    Standard = table.Column<string>(nullable: true),
                    Total = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requirements_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSpents",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    HoursSpent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSpents", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_TimeSpents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_Name",
                table: "Competitions",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_CompetitionId",
                table: "Requirements",
                column: "CompetitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompetitionTotalScores_Competitions_CompetitionId",
                table: "UserCompetitionTotalScores",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompetitionTotalScores_Users_UserId",
                table: "UserCompetitionTotalScores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropForeignKey(
                name: "FK_UserCompetitionTotalScores_Competitions_CompetitionId",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompetitionTotalScores_Users_UserId",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups");

            migrationBuilder.DropTable(
                name: "OTPs");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropTable(
                name: "TimeSpents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCompetitionTotalScores",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Name",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_Name",
                table: "Competitions");*/

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Users");

            /*migrationBuilder.DropColumn(
                name: "Year",
                table: "UserCompetitionTotalScores");

            migrationBuilder.DropColumn(
                name: "TimeOfArrival",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Awards");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Awards");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserGroups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "UserGroups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserGroups",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Total",
                table: "UserCompetitionTotalScores",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "Best",
                table: "UserCompetitionTotalScores",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserCompetitionTotalScores",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CompetitionId",
                table: "UserCompetitionTotalScores",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserCompetitionTotalScores",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "UserScore",
                table: "Scores",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Competitions",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCompetitionTotalScores",
                table: "UserCompetitionTotalScores",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserCompStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompetitionId = table.Column<int>(nullable: true),
                    Month = table.Column<int>(nullable: false),
                    MonthBestScore = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompStats_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCompStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompetitionTotalScores_CompetitionId",
                table: "UserCompetitionTotalScores",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompStats_CompetitionId",
                table: "UserCompStats",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompStats_UserId",
                table: "UserCompStats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompetitionTotalScores_Competitions_CompetitionId",
                table: "UserCompetitionTotalScores",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompetitionTotalScores_Users_UserId",
                table: "UserCompetitionTotalScores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);*/
        }
    }
}
