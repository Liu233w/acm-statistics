using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class UseQueryHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcWorkerHistories");

            migrationBuilder.DropTable(
                name: "AcHistories");

            migrationBuilder.DropIndex(
                name: "IX_AbpOrganizationUnits_TenantId_Code",
                table: "AbpOrganizationUnits");

            migrationBuilder.CreateTable(
                name: "QueryHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    MainUsername = table.Column<string>(nullable: false),
                    Submission = table.Column<int>(nullable: false),
                    Solved = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryHistories_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueryWorkerHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QueryHistoryId = table.Column<long>(nullable: false),
                    CrawlerName = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Submission = table.Column<int>(nullable: false),
                    Solved = table.Column<int>(nullable: false),
                    HasSolvedList = table.Column<bool>(nullable: false),
                    SolvedList = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryWorkerHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueryWorkerHistories_QueryHistories_QueryHistoryId",
                        column: x => x.QueryHistoryId,
                        principalTable: "QueryHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_TenantId_Code",
                table: "AbpOrganizationUnits",
                columns: new[] { "TenantId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_QueryHistories_UserId",
                table: "QueryHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QueryWorkerHistories_QueryHistoryId",
                table: "QueryWorkerHistories",
                column: "QueryHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryWorkerHistories");

            migrationBuilder.DropTable(
                name: "QueryHistories");

            migrationBuilder.DropIndex(
                name: "IX_AbpOrganizationUnits_TenantId_Code",
                table: "AbpOrganizationUnits");

            migrationBuilder.CreateTable(
                name: "AcHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MainUsername = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Solved = table.Column<int>(type: "int", nullable: false),
                    Submission = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcHistories_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcWorkerHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AcHistoryId = table.Column<long>(type: "bigint", nullable: false),
                    CrawlerName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    ErrorMessage = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    HasSolvedList = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Solved = table.Column<int>(type: "int", nullable: false),
                    SolvedList = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Submission = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcWorkerHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcWorkerHistories_AcHistories_AcHistoryId",
                        column: x => x.AcHistoryId,
                        principalTable: "AcHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_TenantId_Code",
                table: "AbpOrganizationUnits",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcHistories_UserId",
                table: "AcHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AcWorkerHistories_AcHistoryId",
                table: "AcWorkerHistories",
                column: "AcHistoryId");
        }
    }
}
