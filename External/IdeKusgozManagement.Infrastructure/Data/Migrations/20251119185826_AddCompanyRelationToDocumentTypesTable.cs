using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRelationToDocumentTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "IdtDocumentTypes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtDocumentTypes_CompanyId",
                table: "IdtDocumentTypes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtDocumentTypes_IdtCompanies_CompanyId",
                table: "IdtDocumentTypes",
                column: "CompanyId",
                principalTable: "IdtCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtDocumentTypes_IdtCompanies_CompanyId",
                table: "IdtDocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_IdtDocumentTypes_CompanyId",
                table: "IdtDocumentTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "IdtDocumentTypes");
        }
    }
}
