using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_provinces_CityCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardCode",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "WardCode",
                table: "Addresses",
                newName: "WardId");

            migrationBuilder.RenameColumn(
                name: "CityCode",
                table: "Addresses",
                newName: "ProvinceCode");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_WardCode",
                table: "Addresses",
                newName: "IX_Addresses_WardId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CityCode",
                table: "Addresses",
                newName: "IX_Addresses_ProvinceCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_provinces_ProvinceCode",
                table: "Addresses",
                column: "ProvinceCode",
                principalTable: "provinces",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_wards_WardId",
                table: "Addresses",
                column: "WardId",
                principalTable: "wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_provinces_ProvinceCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "WardId",
                table: "Addresses",
                newName: "WardCode");

            migrationBuilder.RenameColumn(
                name: "ProvinceCode",
                table: "Addresses",
                newName: "CityCode");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_WardId",
                table: "Addresses",
                newName: "IX_Addresses_WardCode");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_ProvinceCode",
                table: "Addresses",
                newName: "IX_Addresses_CityCode");

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
    }
}
