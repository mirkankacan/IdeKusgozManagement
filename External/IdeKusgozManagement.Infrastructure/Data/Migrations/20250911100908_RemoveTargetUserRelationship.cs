using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTargetUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtNotifications_AspNetUsers_TargetUserId",
                table: "IdtNotifications");

            migrationBuilder.DropIndex(
                name: "IX_IdtNotifications_TargetUserId",
                table: "IdtNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "IdtNotifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "IdtNotifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtNotifications_TargetUserId",
                table: "IdtNotifications",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtNotifications_AspNetUsers_TargetUserId",
                table: "IdtNotifications",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
