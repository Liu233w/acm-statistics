using Microsoft.EntityFrameworkCore.Migrations;

namespace OHunt.Web.Migrations
{
    public partial class AddProblemLabelMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "problem_label_mappings",
                columns: table => new
                {
                    ProblemId = table.Column<long>(nullable: false),
                    OnlineJudgeId = table.Column<int>(nullable: false),
                    ProblemLabel = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_problem_label_mappings", x => new { x.ProblemId, x.OnlineJudgeId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "problem_label_mappings");
        }
    }
}
