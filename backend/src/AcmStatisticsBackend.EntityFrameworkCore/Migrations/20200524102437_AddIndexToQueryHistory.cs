using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class AddIndexToQueryHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_QuerySummaryId",
                table: "QueryHistories",
                column: "QuerySummaryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QueryHistories_QuerySummaryId",
                table: "QueryHistories");
        }
    }
}
