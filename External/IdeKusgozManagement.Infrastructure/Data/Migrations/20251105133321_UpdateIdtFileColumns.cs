using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdtFileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "IdtFiles");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "IdtFiles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentTypeId",
                table: "IdtFiles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "IdtFiles");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "IdtFiles");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "IdtFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
