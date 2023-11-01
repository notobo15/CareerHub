using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_districts_DistrictCODE",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_provinces_ProvinceCODE",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardCode1",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ProvinceCODE",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ProvinceCODE",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "DistrictCODE",
                table: "Addresses",
                newName: "DistrictCode");

            migrationBuilder.RenameColumn(
                name: "WardCode1",
                table: "Addresses",
                newName: "CityCode");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_DistrictCODE",
                table: "Addresses",
                newName: "IX_Addresses_DistrictCode");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_WardCode1",
                table: "Addresses",
                newName: "IX_Addresses_CityCode");

            migrationBuilder.AlterColumn<string>(
                name: "WardCode",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_WardCode",
                table: "Addresses",
                column: "WardCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_districts_DistrictCode",
                table: "Addresses",
                column: "DistrictCode",
                principalTable: "districts",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_provinces_CityCode",
                table: "Addresses",
                column: "CityCode",
                principalTable: "provinces",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_wards_WardCode",
                table: "Addresses",
                column: "WardCode",
                principalTable: "wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_districts_DistrictCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_provinces_CityCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardCode",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_WardCode",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "DistrictCode",
                table: "Addresses",
                newName: "DistrictCODE");

            migrationBuilder.RenameColumn(
                name: "CityCode",
                table: "Addresses",
                newName: "WardCode1");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_DistrictCode",
                table: "Addresses",
                newName: "IX_Addresses_DistrictCODE");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CityCode",
                table: "Addresses",
                newName: "IX_Addresses_WardCode1");

            migrationBuilder.AlterColumn<string>(
                name: "WardCode",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceCODE",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProvinceCODE",
                table: "Addresses",
                column: "ProvinceCODE");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_districts_DistrictCODE",
                table: "Addresses",
                column: "DistrictCODE",
                principalTable: "districts",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_provinces_ProvinceCODE",
                table: "Addresses",
                column: "ProvinceCODE",
                principalTable: "provinces",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_wards_WardCode1",
                table: "Addresses",
                column: "WardCode1",
                principalTable: "wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
