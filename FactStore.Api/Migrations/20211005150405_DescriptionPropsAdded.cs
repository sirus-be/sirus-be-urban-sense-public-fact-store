using Microsoft.EntityFrameworkCore.Migrations;

namespace FactStore.Api.Migrations
{
    public partial class DescriptionPropsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FactTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Facts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExternalFactConfigs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FactTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Facts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExternalFactConfigs");
        }
    }
}
