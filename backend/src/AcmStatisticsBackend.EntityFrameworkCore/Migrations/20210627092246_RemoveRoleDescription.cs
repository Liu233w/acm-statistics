using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmStatisticsBackend.Migrations
{
    public partial class RemoveRoleDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AbpRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AbpRoles",
                type: "longtext CHARACTER SET utf8mb4",
                maxLength: 5000,
                nullable: true);
        }
    }
}
