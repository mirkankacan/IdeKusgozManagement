using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class AddColCompanyReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyDocumentRequirments_DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments",
                column: "DepartmentDutyId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtCompanyDocumentRequirments_IdtDepartmentDuties_DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments",
                column: "DepartmentDutyId",
                principalTable: "IdtDepartmentDuties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtCompanyDocumentRequirments_IdtDepartmentDuties_DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments");

            migrationBuilder.DropIndex(
                name: "IX_IdtCompanyDocumentRequirments_DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments");

            migrationBuilder.DropColumn(
                name: "DepartmentDutyId",
                table: "IdtCompanyDocumentRequirments");
        }
    }
}
