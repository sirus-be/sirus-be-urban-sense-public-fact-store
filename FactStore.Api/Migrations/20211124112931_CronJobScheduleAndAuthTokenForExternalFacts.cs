using Microsoft.EntityFrameworkCore.Migrations;

namespace FactStore.Api.Migrations
{
    public partial class CronJobScheduleAndAuthTokenForExternalFacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestRenewalMinutes",
                table: "ExternalFactConfigs");

            migrationBuilder.AddColumn<string>(
                name: "CronScheduleExpression",
                table: "ExternalFactConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenAuthorizationHeader",
                table: "ExternalFactConfigs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronScheduleExpression",
                table: "ExternalFactConfigs");

            migrationBuilder.DropColumn(
                name: "TokenAuthorizationHeader",
                table: "ExternalFactConfigs");

            migrationBuilder.AddColumn<int>(
                name: "RequestRenewalMinutes",
                table: "ExternalFactConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
