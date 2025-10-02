using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtNotificationReads_AspNetUsers_CreatedBy",
                table: "IdtNotificationReads");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtWorkRecordExpenses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtLeaveRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtWorkRecordExpenses_FileId",
                table: "IdtWorkRecordExpenses",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtLeaveRequests_FileId",
                table: "IdtLeaveRequests",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtLeaveRequests_IdtFiles_FileId",
                table: "IdtLeaveRequests",
                column: "FileId",
                principalTable: "IdtFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtNotificationReads_AspNetUsers_CreatedBy",
                table: "IdtNotificationReads",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecordExpenses_IdtFiles_FileId",
                table: "IdtWorkRecordExpenses",
                column: "FileId",
                principalTable: "IdtFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtLeaveRequests_IdtFiles_FileId",
                table: "IdtLeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtNotificationReads_AspNetUsers_CreatedBy",
                table: "IdtNotificationReads");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecordExpenses_IdtFiles_FileId",
                table: "IdtWorkRecordExpenses");

            migrationBuilder.DropIndex(
                name: "IX_IdtWorkRecordExpenses_FileId",
                table: "IdtWorkRecordExpenses");

            migrationBuilder.DropIndex(
                name: "IX_IdtLeaveRequests_FileId",
                table: "IdtLeaveRequests");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtWorkRecordExpenses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtLeaveRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtNotificationReads_AspNetUsers_CreatedBy",
                table: "IdtNotificationReads",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}