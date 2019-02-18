using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SkietbaanBE.Migrations
{
    public partial class AddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemberExpiry",
                table: "Users",
                newName: "MemberExpiryDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "MemberStartDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "UserCompStats",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberExpiryDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "UserCompStats");

            migrationBuilder.RenameColumn(
                name: "MemberStartDate",
                table: "Users",
                newName: "MemberExpiry");
        }
    }
}
