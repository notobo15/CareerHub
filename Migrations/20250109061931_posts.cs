using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class posts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_districts_DistrictCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_provinces_ProvinceCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_districts_provinces_province_code",
                table: "districts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Companies_CompanyId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_wards_districts_district_code",
                table: "wards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_wards",
                table: "wards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_provinces",
                table: "provinces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_districts",
                table: "districts");

            migrationBuilder.RenameTable(
                name: "wards",
                newName: "Wards");

            migrationBuilder.RenameTable(
                name: "provinces",
                newName: "Provinces");

            migrationBuilder.RenameTable(
                name: "districts",
                newName: "Districts");

            migrationBuilder.RenameIndex(
                name: "IX_wards_district_code",
                table: "Wards",
                newName: "IX_Wards_district_code");

            migrationBuilder.RenameIndex(
                name: "IX_districts_province_code",
                table: "Districts",
                newName: "IX_Districts_province_code");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Posts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wards",
                table: "Wards",
                column: "code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provinces",
                table: "Provinces",
                column: "code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Districts",
                table: "Districts",
                column: "code");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Districts_DistrictCode",
                table: "Addresses",
                column: "DistrictCode",
                principalTable: "Districts",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Provinces_ProvinceCode",
                table: "Addresses",
                column: "ProvinceCode",
                principalTable: "Provinces",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Wards_WardCode",
                table: "Addresses",
                column: "WardCode",
                principalTable: "Wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Districts_Provinces_province_code",
                table: "Districts",
                column: "province_code",
                principalTable: "Provinces",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Companies_CompanyId",
                table: "Posts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Wards_Districts_district_code",
                table: "Wards",
                column: "district_code",
                principalTable: "Districts",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Districts_DistrictCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Provinces_ProvinceCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Wards_WardCode",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Districts_Provinces_province_code",
                table: "Districts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Companies_CompanyId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Wards_Districts_district_code",
                table: "Wards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wards",
                table: "Wards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provinces",
                table: "Provinces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Districts",
                table: "Districts");

            migrationBuilder.RenameTable(
                name: "Wards",
                newName: "wards");

            migrationBuilder.RenameTable(
                name: "Provinces",
                newName: "provinces");

            migrationBuilder.RenameTable(
                name: "Districts",
                newName: "districts");

            migrationBuilder.RenameIndex(
                name: "IX_Wards_district_code",
                table: "wards",
                newName: "IX_wards_district_code");

            migrationBuilder.RenameIndex(
                name: "IX_Districts_province_code",
                table: "districts",
                newName: "IX_districts_province_code");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_wards",
                table: "wards",
                column: "code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_provinces",
                table: "provinces",
                column: "code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_districts",
                table: "districts",
                column: "code");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_districts_DistrictCode",
                table: "Addresses",
                column: "DistrictCode",
                principalTable: "districts",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_provinces_ProvinceCode",
                table: "Addresses",
                column: "ProvinceCode",
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

            migrationBuilder.AddForeignKey(
                name: "FK_districts_provinces_province_code",
                table: "districts",
                column: "province_code",
                principalTable: "provinces",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Companies_CompanyId",
                table: "Posts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wards_districts_district_code",
                table: "wards",
                column: "district_code",
                principalTable: "districts",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
