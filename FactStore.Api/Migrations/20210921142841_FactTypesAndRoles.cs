using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FactStore.Api.Migrations
{
    public partial class FactTypesAndRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FactTypeId",
                table: "Facts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FactTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactTypeRoles",
                columns: table => new
                {
                    FactTypesId = table.Column<int>(type: "integer", nullable: false),
                    RolesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactTypeRoles", x => new { x.FactTypesId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_FactTypeRoles_FactTypes_FactTypesId",
                        column: x => x.FactTypesId,
                        principalTable: "FactTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactTypeRoles_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Fact_Key",
                table: "Facts",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Facts_FactTypeId",
                table: "Facts",
                column: "FactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FactTypeRoles_RolesId",
                table: "FactTypeRoles",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IDX_FactType_Name",
                table: "FactTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IDX_Role_Name",
                table: "Roles",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Facts_FactTypes_FactTypeId",
                table: "Facts",
                column: "FactTypeId",
                principalTable: "FactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facts_FactTypes_FactTypeId",
                table: "Facts");

            migrationBuilder.DropTable(
                name: "FactTypeRoles");

            migrationBuilder.DropTable(
                name: "FactTypes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IDX_Fact_Key",
                table: "Facts");

            migrationBuilder.DropIndex(
                name: "IX_Facts_FactTypeId",
                table: "Facts");

            migrationBuilder.DropColumn(
                name: "FactTypeId",
                table: "Facts");
        }
    }
}
