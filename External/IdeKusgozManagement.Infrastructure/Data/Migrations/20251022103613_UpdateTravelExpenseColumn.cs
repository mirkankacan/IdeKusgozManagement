using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTravelExpenseColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasTravel",
                table: "IdtWorkRecords");

            migrationBuilder.AddColumn<string>(
                name: "TravelExpenseAmount",
                table: "IdtWorkRecords",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TravelExpenseAmount",
                table: "IdtWorkRecords");

            migrationBuilder.AddColumn<bool>(
                name: "HasTravel",
                table: "IdtWorkRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
