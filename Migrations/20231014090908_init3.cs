using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GgMap",
                table: "Addresses",
                newName: "Nation");

            migrationBuilder.RenameColumn(
                name: "Detail",
                table: "Addresses",
                newName: "GgMapSrc");

            migrationBuilder.AddColumn<string>(
                name: "DetailPosition",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailPosition",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "Nation",
                table: "Addresses",
                newName: "GgMap");

            migrationBuilder.RenameColumn(
                name: "GgMapSrc",
                table: "Addresses",
                newName: "Detail");
        }
    }
}
