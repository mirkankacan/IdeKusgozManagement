using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                table: "IdtNotifications",
                newName: "TargetUsers");

            migrationBuilder.RenameColumn(
                name: "TargetRole",
                table: "IdtNotifications",
                newName: "TargetRoles");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "IdtMessages",
                newName: "TargetUsers");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "IdtMessages",
                newName: "TargetRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetUsers",
                table: "IdtNotifications",
                newName: "TargetUserId");

            migrationBuilder.RenameColumn(
                name: "TargetRoles",
                table: "IdtNotifications",
                newName: "TargetRole");

            migrationBuilder.RenameColumn(
                name: "TargetUsers",
                table: "IdtMessages",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "TargetRoles",
                table: "IdtMessages",
                newName: "Role");
        }
    }
}
