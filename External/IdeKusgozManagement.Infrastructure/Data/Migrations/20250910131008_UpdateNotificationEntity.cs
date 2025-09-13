using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RedirectUrl",
                table: "IdtNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetRole",
                table: "IdtNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetUserId",
                table: "IdtNotifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "IdtNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtNotifications_AspNetUsers_TargetUserId",
                table: "IdtNotifications");

            migrationBuilder.DropIndex(
                name: "IX_IdtNotifications_TargetUserId",
                table: "IdtNotifications");

            migrationBuilder.DropColumn(
                name: "RedirectUrl",
                table: "IdtNotifications");

            migrationBuilder.DropColumn(
                name: "TargetRole",
                table: "IdtNotifications");

            migrationBuilder.DropColumn(
                name: "TargetUserId",
                table: "IdtNotifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "IdtNotifications");
        }
    }
}
