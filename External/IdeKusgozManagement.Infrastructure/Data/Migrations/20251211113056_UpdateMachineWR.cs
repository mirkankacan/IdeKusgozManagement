using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMachineWR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalTransport",
                table: "IdtMachineWorkRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "IdtMachineWorkRecords",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasInternalTransport",
                table: "IdtMachineWorkRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasInternalTransport",
                table: "IdtMachineWorkRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "IdtMachineWorkRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(350)",
                oldMaxLength: 350,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InternalTransport",
                table: "IdtMachineWorkRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
