using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class AddSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVirtualJudge",
                table: "QueryWorkerHistories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionsByCrawlerName",
                table: "QueryWorkerHistories",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "UserSettingAttributes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    LastTimeZoneChangedTime = table.Column<DateTime>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettingAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettingAttributes_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSettingAttributes_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSettingAttributes_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettingAttributes_CreatorUserId",
                table: "UserSettingAttributes",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettingAttributes_LastModifierUserId",
                table: "UserSettingAttributes",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettingAttributes_UserId",
                table: "UserSettingAttributes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettingAttributes");

            migrationBuilder.DropColumn(
                name: "IsVirtualJudge",
                table: "QueryWorkerHistories");

            migrationBuilder.DropColumn(
                name: "SubmissionsByCrawlerName",
                table: "QueryWorkerHistories");
        }
    }
}
