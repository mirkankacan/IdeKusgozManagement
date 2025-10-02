using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptImageUrl",
                table: "IdtWorkRecordExpenses",
                newName: "FileId");

            migrationBuilder.RenameColumn(
                name: "DocumentUrl",
                table: "IdtLeaveRequests",
                newName: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "IdtWorkRecordExpenses",
                newName: "ReceiptImageUrl");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "IdtLeaveRequests",
                newName: "DocumentUrl");
        }
    }
}
