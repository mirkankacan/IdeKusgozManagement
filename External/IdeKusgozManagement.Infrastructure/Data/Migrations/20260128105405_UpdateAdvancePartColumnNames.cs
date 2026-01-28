using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdvancePartColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_ApprovedById",
                table: "IdtAdvanceParts");

            migrationBuilder.RenameColumn(
                name: "ApprovedDate",
                table: "IdtAdvanceParts",
                newName: "CompletedDate");

            migrationBuilder.RenameColumn(
                name: "ApprovedById",
                table: "IdtAdvanceParts",
                newName: "CompletedById");

            migrationBuilder.RenameIndex(
                name: "IX_IdtAdvanceParts_ApprovedById",
                table: "IdtAdvanceParts",
                newName: "IX_IdtAdvanceParts_CompletedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_CompletedById",
                table: "IdtAdvanceParts",
                column: "CompletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_CompletedById",
                table: "IdtAdvanceParts");

            migrationBuilder.RenameColumn(
                name: "CompletedDate",
                table: "IdtAdvanceParts",
                newName: "ApprovedDate");

            migrationBuilder.RenameColumn(
                name: "CompletedById",
                table: "IdtAdvanceParts",
                newName: "ApprovedById");

            migrationBuilder.RenameIndex(
                name: "IX_IdtAdvanceParts_CompletedById",
                table: "IdtAdvanceParts",
                newName: "IX_IdtAdvanceParts_ApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_ApprovedById",
                table: "IdtAdvanceParts",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
