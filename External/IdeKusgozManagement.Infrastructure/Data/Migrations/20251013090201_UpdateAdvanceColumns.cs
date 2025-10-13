using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdvanceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitManagerProcessedAt",
                table: "IdtAdvances",
                newName: "UnitManagerProcessedDate");

            migrationBuilder.RenameColumn(
                name: "ChiefProcessedAt",
                table: "IdtAdvances",
                newName: "ChiefProcessedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitManagerProcessedDate",
                table: "IdtAdvances",
                newName: "UnitManagerProcessedAt");

            migrationBuilder.RenameColumn(
                name: "ChiefProcessedDate",
                table: "IdtAdvances",
                newName: "ChiefProcessedAt");
        }
    }
}
