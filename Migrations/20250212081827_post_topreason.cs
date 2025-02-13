using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class post_topreason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TopReason",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopReason",
                table: "Posts");
        }
    }
}
