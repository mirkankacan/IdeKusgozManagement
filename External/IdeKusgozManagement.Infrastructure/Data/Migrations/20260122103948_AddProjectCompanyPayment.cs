using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectCompanyPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtCompanyPayments_AspNetUsers_ApproverId",
                table: "IdtCompanyPayments");

            migrationBuilder.RenameColumn(
                name: "ApproverId",
                table: "IdtCompanyPayments",
                newName: "SelectedApproverId");

            migrationBuilder.RenameIndex(
                name: "IX_IdtCompanyPayments_ApproverId",
                table: "IdtCompanyPayments",
                newName: "IX_IdtCompanyPayments_SelectedApproverId");

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentId",
                table: "IdtCompanyPayments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                table: "IdtCompanyPayments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "IdtCompanyPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyPayments_ProjectId",
                table: "IdtCompanyPayments",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtCompanyPayments_AspNetUsers_SelectedApproverId",
                table: "IdtCompanyPayments",
                column: "SelectedApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtCompanyPayments_IdtProjects_ProjectId",
                table: "IdtCompanyPayments",
                column: "ProjectId",
                principalTable: "IdtProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtCompanyPayments_AspNetUsers_SelectedApproverId",
                table: "IdtCompanyPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtCompanyPayments_IdtProjects_ProjectId",
                table: "IdtCompanyPayments");

            migrationBuilder.DropIndex(
                name: "IX_IdtCompanyPayments_ProjectId",
                table: "IdtCompanyPayments");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "IdtCompanyPayments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "IdtCompanyPayments");

            migrationBuilder.RenameColumn(
                name: "SelectedApproverId",
                table: "IdtCompanyPayments",
                newName: "ApproverId");

            migrationBuilder.RenameIndex(
                name: "IX_IdtCompanyPayments_SelectedApproverId",
                table: "IdtCompanyPayments",
                newName: "IX_IdtCompanyPayments_ApproverId");

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentId",
                table: "IdtCompanyPayments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtCompanyPayments_AspNetUsers_ApproverId",
                table: "IdtCompanyPayments",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
