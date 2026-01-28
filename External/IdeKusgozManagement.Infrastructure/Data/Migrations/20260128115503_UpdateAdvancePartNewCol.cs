using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdvancePartNewCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_CompletedById",
                table: "IdtAdvanceParts");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedById",
                table: "IdtAdvanceParts",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "IdtAdvanceParts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtAdvanceParts_ApprovedById",
                table: "IdtAdvanceParts",
                column: "ApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_ApprovedById",
                table: "IdtAdvanceParts",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_CompletedById",
                table: "IdtAdvanceParts",
                column: "CompletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_ApprovedById",
                table: "IdtAdvanceParts");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_CompletedById",
                table: "IdtAdvanceParts");

            migrationBuilder.DropIndex(
                name: "IX_IdtAdvanceParts_ApprovedById",
                table: "IdtAdvanceParts");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "IdtAdvanceParts");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "IdtAdvanceParts");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvanceParts_AspNetUsers_CompletedById",
                table: "IdtAdvanceParts",
                column: "CompletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
