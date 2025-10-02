using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdtFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "IdtFiles",
                newName: "Path");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "IdtFiles",
                newName: "OriginalName");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "IdtFiles",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Path",
                table: "IdtFiles",
                newName: "OriginalFileName");

            migrationBuilder.RenameColumn(
                name: "OriginalName",
                table: "IdtFiles",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "IdtFiles",
                newName: "FileName");
        }
    }
}
