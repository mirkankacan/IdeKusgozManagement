using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFileNullableIdtWorkRecordExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecordExpenses_IdtFiles_FileId",
                table: "IdtWorkRecordExpenses");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtWorkRecordExpenses",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecordExpenses_IdtFiles_FileId",
                table: "IdtWorkRecordExpenses",
                column: "FileId",
                principalTable: "IdtFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecordExpenses_IdtFiles_FileId",
                table: "IdtWorkRecordExpenses");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtWorkRecordExpenses",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecordExpenses_IdtFiles_FileId",
                table: "IdtWorkRecordExpenses",
                column: "FileId",
                principalTable: "IdtFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
