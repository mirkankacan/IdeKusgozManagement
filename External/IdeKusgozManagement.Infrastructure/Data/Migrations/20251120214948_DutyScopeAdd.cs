using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class DutyScopeAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "IdtDocumentTypes");

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "IdtDepartmentDuties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "IdtDepartmentDuties");

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "IdtDocumentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
