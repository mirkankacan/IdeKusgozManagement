using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdvanceNewCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByChiefId",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByUnitManagerId",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_UpdatedBy",
                table: "IdtAdvances");

            migrationBuilder.AddColumn<string>(
                name: "CompletedById",
                table: "IdtAdvances",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "IdtAdvances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdtAdvances_CompletedById",
                table: "IdtAdvances",
                column: "CompletedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_CompletedById",
                table: "IdtAdvances",
                column: "CompletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByChiefId",
                table: "IdtAdvances",
                column: "ProcessedByChiefId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByUnitManagerId",
                table: "IdtAdvances",
                column: "ProcessedByUnitManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_UpdatedBy",
                table: "IdtAdvances",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_CompletedById",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByChiefId",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByUnitManagerId",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_UpdatedBy",
                table: "IdtAdvances");

            migrationBuilder.DropIndex(
                name: "IX_IdtAdvances_CompletedById",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "CompletedById",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "IdtAdvances");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByChiefId",
                table: "IdtAdvances",
                column: "ProcessedByChiefId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByUnitManagerId",
                table: "IdtAdvances",
                column: "ProcessedByUnitManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_UpdatedBy",
                table: "IdtAdvances",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
