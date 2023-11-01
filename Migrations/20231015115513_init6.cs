using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AddressId",
                table: "Posts",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Addresses_AddressId",
                table: "Posts",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Addresses_AddressId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AddressId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Posts");
        }
    }
}
