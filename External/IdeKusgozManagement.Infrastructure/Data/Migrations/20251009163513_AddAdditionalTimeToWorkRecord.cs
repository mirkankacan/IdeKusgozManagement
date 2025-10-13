using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalTimeToWorkRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "AdditionalEndTime",
                table: "IdtWorkRecords",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AdditionalStartTime",
                table: "IdtWorkRecords",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalEndTime",
                table: "IdtWorkRecords");

            migrationBuilder.DropColumn(
                name: "AdditionalStartTime",
                table: "IdtWorkRecords");
        }
    }
}
