using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DocumentTablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_IdtDepartments_DepartmentId",
                table: "IdtFiles");

            migrationBuilder.DropTable(
                name: "IdtDepartmentDocumentTypes");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "IdtFiles",
                newName: "TargetDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_IdtFiles_DepartmentId",
                table: "IdtFiles",
                newName: "IX_IdtFiles_TargetDepartmentId");

            migrationBuilder.AddColumn<string>(
                name: "TargetCompanyId",
                table: "IdtFiles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetDepartmentDutyId",
                table: "IdtFiles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IdtDocumentTypes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentDutyId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "IdtCompanies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdtDepartmentDuties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtDepartmentDuties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdtDepartmentRequiredDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentDutyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_IdtFiles_TargetCompanyId",
                table: "IdtFiles",
                column: "TargetCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtFiles_TargetDepartmentDutyId",
                table: "IdtFiles",
                column: "TargetDepartmentDutyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentDutyId",
                table: "AspNetUsers",
                column: "DepartmentDutyId");

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
                name: "FK_AspNetUsers_IdtDepartmentDuties_DepartmentDutyId",
                table: "AspNetUsers",
                column: "DepartmentDutyId",
                principalTable: "IdtDepartmentDuties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_IdtCompanies_TargetCompanyId",
                table: "IdtFiles",
                column: "TargetCompanyId",
                principalTable: "IdtCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_IdtDepartmentDuties_TargetDepartmentDutyId",
                table: "IdtFiles",
                column: "TargetDepartmentDutyId",
                principalTable: "IdtDepartmentDuties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_IdtDepartments_TargetDepartmentId",
                table: "IdtFiles",
                column: "TargetDepartmentId",
                principalTable: "IdtDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_IdtDepartmentDuties_DepartmentDutyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_IdtCompanies_TargetCompanyId",
                table: "IdtFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_IdtDepartmentDuties_TargetDepartmentDutyId",
                table: "IdtFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_IdtDepartments_TargetDepartmentId",
                table: "IdtFiles");

            migrationBuilder.DropTable(
                name: "IdtCompanies");

            migrationBuilder.DropTable(
                name: "IdtDepartmentRequiredDocuments");

            migrationBuilder.DropTable(
                name: "IdtDepartmentDuties");

            migrationBuilder.DropIndex(
                name: "IX_IdtFiles_TargetCompanyId",
                table: "IdtFiles");

            migrationBuilder.DropIndex(
                name: "IX_IdtFiles_TargetDepartmentDutyId",
                table: "IdtFiles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmentDutyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TargetCompanyId",
                table: "IdtFiles");

            migrationBuilder.DropColumn(
                name: "TargetDepartmentDutyId",
                table: "IdtFiles");

            migrationBuilder.DropColumn(
                name: "DepartmentDutyId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TargetDepartmentId",
                table: "IdtFiles",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_IdtFiles_TargetDepartmentId",
                table: "IdtFiles",
                newName: "IX_IdtFiles_DepartmentId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IdtDocumentTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "IdtDepartmentDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtDepartmentDocumentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentDocumentTypes_IdtDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "IdtDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdtDepartmentDocumentTypes_IdtDocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "IdtDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentDocumentTypes_DepartmentId",
                table: "IdtDepartmentDocumentTypes",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtDepartmentDocumentTypes_DocumentTypeId",
                table: "IdtDepartmentDocumentTypes",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_IdtDepartments_DepartmentId",
                table: "IdtFiles",
                column: "DepartmentId",
                principalTable: "IdtDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
