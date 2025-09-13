using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "IdtWorkRecords",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "IdtWorkRecords",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_IdtWorkRecords_CreatedBy",
                table: "IdtWorkRecords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IdtWorkRecords_UpdatedBy",
                table: "IdtWorkRecords",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecords_AspNetUsers_CreatedBy",
                table: "IdtWorkRecords",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecords_AspNetUsers_UpdatedBy",
                table: "IdtWorkRecords",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecords_AspNetUsers_CreatedBy",
                table: "IdtWorkRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecords_AspNetUsers_UpdatedBy",
                table: "IdtWorkRecords");

            migrationBuilder.DropIndex(
                name: "IX_IdtWorkRecords_CreatedBy",
                table: "IdtWorkRecords");

            migrationBuilder.DropIndex(
                name: "IX_IdtWorkRecords_UpdatedBy",
                table: "IdtWorkRecords");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "IdtWorkRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "IdtWorkRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
