using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvanceRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "IdtAdvances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_IdtAdvances_CreatedBy",
                table: "IdtAdvances",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IdtAdvances_ProcessedByChiefId",
                table: "IdtAdvances",
                column: "ProcessedByChiefId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtAdvances_ProcessedByUnitManagerId",
                table: "IdtAdvances",
                column: "ProcessedByUnitManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_CreatedBy",
                table: "IdtAdvances",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_CreatedBy",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByChiefId",
                table: "IdtAdvances");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtAdvances_AspNetUsers_ProcessedByUnitManagerId",
                table: "IdtAdvances");

            migrationBuilder.DropIndex(
                name: "IX_IdtAdvances_CreatedBy",
                table: "IdtAdvances");

            migrationBuilder.DropIndex(
                name: "IX_IdtAdvances_ProcessedByChiefId",
                table: "IdtAdvances");

            migrationBuilder.DropIndex(
                name: "IX_IdtAdvances_ProcessedByUnitManagerId",
                table: "IdtAdvances");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "IdtAdvances",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
