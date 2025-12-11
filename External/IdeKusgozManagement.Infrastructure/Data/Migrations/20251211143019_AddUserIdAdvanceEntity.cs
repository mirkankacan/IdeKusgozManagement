using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class AddUserIdAdvanceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "IdtAdvances",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_IdtAdvances_UserId",
                table: "IdtAdvances",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_UserId",
                table: "IdtAdvances",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_UserId",
                table: "IdtAdvances");

            migrationBuilder.DropIndex(
                name: "IX_IdtAdvances_UserId",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IdtAdvances");
        }
    }
}
