using Microsoft.EntityFrameworkCore.Migrations;

namespace FactStore.Api.Migrations
{
    public partial class ChangedExternalFactEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "ExternalFactConfigs",
                newName: "FactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalFactConfigs_FactTypeId",
                table: "ExternalFactConfigs",
                column: "FactTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalFactConfigs_FactTypes_FactTypeId",
                table: "ExternalFactConfigs",
                column: "FactTypeId",
                principalTable: "FactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalFactConfigs_FactTypes_FactTypeId",
                table: "ExternalFactConfigs");

            migrationBuilder.DropIndex(
                name: "IX_ExternalFactConfigs_FactTypeId",
                table: "ExternalFactConfigs");

            migrationBuilder.RenameColumn(
                name: "FactTypeId",
                table: "ExternalFactConfigs",
                newName: "TypeId");
        }
    }
}
