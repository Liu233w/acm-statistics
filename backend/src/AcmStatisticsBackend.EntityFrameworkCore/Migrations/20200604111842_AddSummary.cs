using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class AddSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSolvedList",
                table: "QueryWorkerHistories");

            migrationBuilder.DropColumn(
                name: "Solved",
                table: "QueryHistories");

            migrationBuilder.DropColumn(
                name: "Submission",
                table: "QueryHistories");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionsByCrawlerName",
                table: "QueryWorkerHistories",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SolvedList",
                table: "QueryWorkerHistories",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsReliableSource",
                table: "QueryHistories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "QuerySummaries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QueryHistoryId = table.Column<long>(nullable: false),
                    GenerateTime = table.Column<DateTime>(nullable: false),
                    SummaryWarnings = table.Column<string>(nullable: false),
                    Submission = table.Column<int>(nullable: false),
                    Solved = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuerySummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuerySummaries_QueryHistories_QueryHistoryId",
                        column: x => x.QueryHistoryId,
                        principalTable: "QueryHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryCrawlerSummaries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QuerySummaryId = table.Column<long>(nullable: false),
                    CrawlerName = table.Column<string>(nullable: false),
                    Submission = table.Column<int>(nullable: false),
                    Solved = table.Column<int>(nullable: false),
                    IsVirtualJudge = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryCrawlerSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryCrawlerSummaries_QuerySummaries_QuerySummaryId",
                        column: x => x.QuerySummaryId,
                        principalTable: "QuerySummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsernameInCrawler",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QueryCrawlerSummaryId = table.Column<long>(nullable: false),
                    FromCrawlerName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsernameInCrawler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsernameInCrawler_QueryCrawlerSummaries_QueryCrawlerSummaryId",
                        column: x => x.QueryCrawlerSummaryId,
                        principalTable: "QueryCrawlerSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueryCrawlerSummaries_QuerySummaryId",
                table: "QueryCrawlerSummaries",
                column: "QuerySummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuerySummaries_QueryHistoryId",
                table: "QuerySummaries",
                column: "QueryHistoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsernameInCrawler_QueryCrawlerSummaryId",
                table: "UsernameInCrawler",
                column: "QueryCrawlerSummaryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsernameInCrawler");

            migrationBuilder.DropTable(
                name: "QueryCrawlerSummaries");

            migrationBuilder.DropTable(
                name: "QuerySummaries");

            migrationBuilder.DropColumn(
                name: "IsReliableSource",
                table: "QueryHistories");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionsByCrawlerName",
                table: "QueryWorkerHistories",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SolvedList",
                table: "QueryWorkerHistories",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasSolvedList",
                table: "QueryWorkerHistories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Solved",
                table: "QueryHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Submission",
                table: "QueryHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
