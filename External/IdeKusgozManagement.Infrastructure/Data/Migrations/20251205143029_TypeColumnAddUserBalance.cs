using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TypeColumnAddUserBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "IdtUserBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "IdtUserBalances");
        }
    }
}
