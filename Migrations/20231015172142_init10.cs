using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_wards_WardId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "WardId",
                table: "Addresses",
                newName: "WardCode");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_WardId",
                table: "Addresses",
                newName: "IX_Addresses_WardCode");

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
                name: "FK_Addresses_wards_WardCode",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "WardCode",
                table: "Addresses",
                newName: "WardId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_WardCode",
                table: "Addresses",
                newName: "IX_Addresses_WardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_wards_WardId",
                table: "Addresses",
                column: "WardId",
                principalTable: "wards",
                principalColumn: "code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
