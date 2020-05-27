using Microsoft.EntityFrameworkCore.Migrations;

namespace OHunt.Web.Migrations
{
    public partial class AddOjName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OjName",
                table: "submissions",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_OjName",
                table: "submissions",
                column: "OjName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_submissions_OjName",
                table: "submissions");

            migrationBuilder.DropColumn(
                name: "OjName",
                table: "submissions");
        }
    }
}
