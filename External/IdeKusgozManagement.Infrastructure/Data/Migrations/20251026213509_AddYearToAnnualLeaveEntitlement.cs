using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddYearToAnnualLeaveEntitlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "IdtAnnualLeaveEntitlements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "IdtAnnualLeaveEntitlements");
        }
    }
}
