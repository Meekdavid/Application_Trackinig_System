using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobPostId1",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobPostId1",
                table: "Applications",
                column: "JobPostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_JobPosts_JobPostId1",
                table: "Applications",
                column: "JobPostId1",
                principalTable: "JobPosts",
                principalColumn: "JobPostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_JobPosts_JobPostId1",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobPostId1",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "JobPostId1",
                table: "Applications");
        }
    }
}
