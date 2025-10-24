using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkRecordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcuseReason",
                table: "IdtWorkRecords");

            migrationBuilder.AddColumn<string>(
                name: "DailyStatus",
                table: "IdtWorkRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyStatus",
                table: "IdtWorkRecords");

            migrationBuilder.AddColumn<string>(
                name: "ExcuseReason",
                table: "IdtWorkRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
