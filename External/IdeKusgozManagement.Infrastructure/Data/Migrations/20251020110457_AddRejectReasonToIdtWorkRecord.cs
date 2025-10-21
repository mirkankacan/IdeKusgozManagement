using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectReasonToIdtWorkRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "IdtWorkRecords",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "IdtWorkRecords");
        }
    }
}
