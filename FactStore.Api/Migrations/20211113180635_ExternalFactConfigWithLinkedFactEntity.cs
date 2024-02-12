using Microsoft.EntityFrameworkCore.Migrations;

namespace FactStore.Api.Migrations
{
    public partial class ExternalFactConfigWithLinkedFactEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalFactConfigs_FactTypes_FactTypeId",
                table: "ExternalFactConfigs");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "ExternalFactConfigs");

            migrationBuilder.RenameColumn(
                name: "FactTypeId",
                table: "ExternalFactConfigs",
                newName: "FactId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalFactConfigs_FactTypeId",
                table: "ExternalFactConfigs",
                newName: "IX_ExternalFactConfigs_FactId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalFactConfigs_Facts_FactId",
                table: "ExternalFactConfigs",
                column: "FactId",
                principalTable: "Facts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalFactConfigs_Facts_FactId",
                table: "ExternalFactConfigs");

            migrationBuilder.RenameColumn(
                name: "FactId",
                table: "ExternalFactConfigs",
                newName: "FactTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalFactConfigs_FactId",
                table: "ExternalFactConfigs",
                newName: "IX_ExternalFactConfigs_FactTypeId");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ExternalFactConfigs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalFactConfigs_FactTypes_FactTypeId",
                table: "ExternalFactConfigs",
                column: "FactTypeId",
                principalTable: "FactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
