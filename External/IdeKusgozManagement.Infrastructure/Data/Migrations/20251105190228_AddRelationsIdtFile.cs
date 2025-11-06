using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class AddRelationsIdtFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtFiles_DepartmentId",
                table: "IdtFiles",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtFiles_DocumentTypeId",
                table: "IdtFiles",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtFiles_TargetUserId",
                table: "IdtFiles",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_AspNetUsers_TargetUserId",
                table: "IdtFiles",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_IdtDepartments_DepartmentId",
                table: "IdtFiles",
                column: "DepartmentId",
                principalTable: "IdtDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtFiles_IdtDocumentTypes_DocumentTypeId",
                table: "IdtFiles",
                column: "DocumentTypeId",
                principalTable: "IdtDocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_AspNetUsers_TargetUserId",
                table: "IdtFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_IdtDepartments_DepartmentId",
                table: "IdtFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtFiles_IdtDocumentTypes_DocumentTypeId",
                table: "IdtFiles");

            migrationBuilder.DropIndex(
                name: "IX_IdtFiles_DepartmentId",
                table: "IdtFiles");

            migrationBuilder.DropIndex(
                name: "IX_IdtFiles_DocumentTypeId",
                table: "IdtFiles");

            migrationBuilder.DropIndex(
                name: "IX_IdtFiles_TargetUserId",
                table: "IdtFiles");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
