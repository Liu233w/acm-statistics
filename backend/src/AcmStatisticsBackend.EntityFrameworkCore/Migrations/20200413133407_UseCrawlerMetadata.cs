using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class UseCrawlerMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcWorkerHistories_OjCrawlers_OjCrawlerId",
                table: "AcWorkerHistories");

            migrationBuilder.DropTable(
                name: "OjCrawlers");

            migrationBuilder.DropIndex(
                name: "IX_AcWorkerHistories_OjCrawlerId",
                table: "AcWorkerHistories");

            migrationBuilder.DropColumn(
                name: "OjCrawlerId",
                table: "AcWorkerHistories");

            migrationBuilder.AddColumn<string>(
                name: "CrawlerName",
                table: "AcWorkerHistories",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrawlerName",
                table: "AcWorkerHistories");

            migrationBuilder.AddColumn<int>(
                name: "OjCrawlerId",
                table: "AcWorkerHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OjCrawlers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CrawlerName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Title = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OjCrawlers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcWorkerHistories_OjCrawlerId",
                table: "AcWorkerHistories",
                column: "OjCrawlerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcWorkerHistories_OjCrawlers_OjCrawlerId",
                table: "AcWorkerHistories",
                column: "OjCrawlerId",
                principalTable: "OjCrawlers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
