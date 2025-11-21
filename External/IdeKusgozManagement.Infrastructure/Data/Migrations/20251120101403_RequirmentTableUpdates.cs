using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class RequirmentTableUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdtCompanyDocumentRequirments");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "IdtDepartments");

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "IdtDocumentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "IdtDepartmentDocumentRequirments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentDocumentRequirments_CompanyId",
                table: "IdtDepartmentDocumentRequirments",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtDepartmentDocumentRequirments_IdtCompanies_CompanyId",
                table: "IdtDepartmentDocumentRequirments",
                column: "CompanyId",
                principalTable: "IdtCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtDepartmentDocumentRequirments_IdtCompanies_CompanyId",
                table: "IdtDepartmentDocumentRequirments");

            migrationBuilder.DropIndex(
                name: "IX_IdtDepartmentDocumentRequirments_CompanyId",
                table: "IdtDepartmentDocumentRequirments");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "IdtDocumentTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "IdtDepartmentDocumentRequirments");

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "IdtDepartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IdtCompanyDocumentRequirments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DepartmentDutyId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtCompanyDocumentRequirments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtCompanyDocumentRequirments_IdtCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "IdtCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtCompanyDocumentRequirments_IdtDepartmentDuties_DepartmentDutyId",
                        column: x => x.DepartmentDutyId,
                        principalTable: "IdtDepartmentDuties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtCompanyDocumentRequirments_IdtDocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "IdtDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyDocumentRequirments_CompanyId",
                table: "IdtCompanyDocumentRequirments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyDocumentRequirments_DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments",
                column: "DepartmentDutyId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyDocumentRequirments_DocumentTypeId",
                table: "IdtCompanyDocumentRequirments",
                column: "DocumentTypeId");
        }
    }
}
