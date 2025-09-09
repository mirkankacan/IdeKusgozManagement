using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtLeaveRequests_AspNetUsers_CreatedBy",
                table: "IdtLeaveRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "IdtLeaveRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "IdtMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtMessages_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdtLeaveRequests_UpdatedBy",
                table: "IdtLeaveRequests",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IdtMessages_CreatedBy",
                table: "IdtMessages",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtLeaveRequests_AspNetUsers_CreatedBy",
                table: "IdtLeaveRequests",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtLeaveRequests_AspNetUsers_UpdatedBy",
                table: "IdtLeaveRequests",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtLeaveRequests_AspNetUsers_CreatedBy",
                table: "IdtLeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtLeaveRequests_AspNetUsers_UpdatedBy",
                table: "IdtLeaveRequests");

            migrationBuilder.DropTable(
                name: "IdtMessages");

            migrationBuilder.DropIndex(
                name: "IX_IdtLeaveRequests_UpdatedBy",
                table: "IdtLeaveRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "IdtLeaveRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtLeaveRequests_AspNetUsers_CreatedBy",
                table: "IdtLeaveRequests",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
