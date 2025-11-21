using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class CompanyBasedRequirments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtDocumentTypes_IdtCompanies_CompanyId",
                table: "IdtDocumentTypes");

            migrationBuilder.DropTable(
                name: "IdtDepartmentRequiredDocuments");

            migrationBuilder.DropIndex(
                name: "IX_IdtDocumentTypes_CompanyId",
                table: "IdtDocumentTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "IdtDocumentTypes");

            migrationBuilder.CreateTable(
                name: "IdtCompanyDocumentRequirments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_IdtCompanyDocumentRequirments_IdtDocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "IdtDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdtDepartmentDocumentRequirments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DepartmentDutyId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtDepartmentDocumentRequirments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentDocumentRequirments_IdtDepartmentDuties_DepartmentDutyId",
                        column: x => x.DepartmentDutyId,
                        principalTable: "IdtDepartmentDuties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentDocumentRequirments_IdtDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "IdtDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentDocumentRequirments_IdtDocumentTypes_DocumentTypeId",
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
                name: "IX_IdtCompanyDocumentRequirments_DocumentTypeId",
                table: "IdtCompanyDocumentRequirments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentDocumentRequirments_DepartmentDutyId",
                table: "IdtDepartmentDocumentRequirments",
                column: "DepartmentDutyId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentDocumentRequirments_DepartmentId",
                table: "IdtDepartmentDocumentRequirments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentDocumentRequirments_DocumentTypeId",
                table: "IdtDepartmentDocumentRequirments",
                column: "DocumentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdtCompanyDocumentRequirments");

            migrationBuilder.DropTable(
                name: "IdtDepartmentDocumentRequirments");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "IdtDocumentTypes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdtDepartmentRequiredDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentDutyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtDepartmentRequiredDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentRequiredDocuments_IdtDepartmentDuties_DepartmentDutyId",
                        column: x => x.DepartmentDutyId,
                        principalTable: "IdtDepartmentDuties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentRequiredDocuments_IdtDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "IdtDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentRequiredDocuments_IdtDocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "IdtDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdtDocumentTypes_CompanyId",
                table: "IdtDocumentTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentRequiredDocuments_DepartmentDutyId",
                table: "IdtDepartmentRequiredDocuments",
                column: "DepartmentDutyId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentRequiredDocuments_DepartmentId",
                table: "IdtDepartmentRequiredDocuments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentRequiredDocuments_DocumentTypeId",
                table: "IdtDepartmentRequiredDocuments",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtDocumentTypes_IdtCompanies_CompanyId",
                table: "IdtDocumentTypes",
                column: "CompanyId",
                principalTable: "IdtCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
