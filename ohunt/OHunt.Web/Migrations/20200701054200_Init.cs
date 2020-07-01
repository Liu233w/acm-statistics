using Microsoft.EntityFrameworkCore.Migrations;

namespace OHunt.Web.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "submissions",
                columns: table => new
                {
                    SubmissionId = table.Column<long>(nullable: false),
                    OnlineJudgeId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissions", x => new { x.SubmissionId, x.OnlineJudgeId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "submissions");
        }
    }
}
