using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentApp.Migrations
{
    public partial class init13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_Id",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Posts",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_Id",
                table: "Posts",
                newName: "IX_Posts_AppUserId");

            migrationBuilder.AddColumn<string>(
                name: "RecruiterId",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_AppUserId",
                table: "Posts",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_AppUserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RecruiterId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Posts",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_AppUserId",
                table: "Posts",
                newName: "IX_Posts_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_Id",
                table: "Posts",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
