using Microsoft.EntityFrameworkCore.Migrations;

namespace OHunt.Web.Migrations
{
    public partial class AddIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_submissions_Status",
                table: "submissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_UserName",
                table: "submissions",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_submissions_Status",
                table: "submissions");

            migrationBuilder.DropIndex(
                name: "IX_submissions_UserName",
                table: "submissions");
        }
    }
}
