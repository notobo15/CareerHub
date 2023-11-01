using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardCODE",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_WardCODE",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "WardCODE",
                table: "Addresses",
                newName: "WardCode");

            migrationBuilder.AlterColumn<string>(
                name: "WardCode",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardCode1",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_WardCode1",
                table: "Addresses",
                column: "WardCode1");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_wards_WardCode1",
                table: "Addresses",
                column: "WardCode1",
                principalTable: "wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardCode1",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_WardCode1",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "WardCode1",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "WardCode",
                table: "Addresses",
                newName: "WardCODE");

            migrationBuilder.AlterColumn<string>(
                name: "WardCODE",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_WardCODE",
                table: "Addresses",
                column: "WardCODE");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_wards_WardCODE",
                table: "Addresses",
                column: "WardCODE",
                principalTable: "wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
