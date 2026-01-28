using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "IdtCompanyPayments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyPayments_CreatedBy",
                table: "IdtCompanyPayments",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtCompanyPayments_AspNetUsers_CreatedBy",
                table: "IdtCompanyPayments",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtCompanyPayments_AspNetUsers_CreatedBy",
                table: "IdtCompanyPayments");

            migrationBuilder.DropIndex(
                name: "IX_IdtCompanyPayments_CreatedBy",
                table: "IdtCompanyPayments");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "IdtCompanyPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
