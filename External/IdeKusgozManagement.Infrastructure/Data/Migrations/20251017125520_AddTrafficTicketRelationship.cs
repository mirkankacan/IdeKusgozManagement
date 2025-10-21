using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrafficTicketRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProjectId",
                table: "IdtTrafficTickets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtTrafficTickets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentId",
                table: "IdtTrafficTickets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_IdtTrafficTickets_EquipmentId",
                table: "IdtTrafficTickets",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtTrafficTickets_FileId",
                table: "IdtTrafficTickets",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtTrafficTickets_ProjectId",
                table: "IdtTrafficTickets",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtTrafficTickets_IdtEquipments_EquipmentId",
                table: "IdtTrafficTickets",
                column: "EquipmentId",
                principalTable: "IdtEquipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtTrafficTickets_IdtFiles_FileId",
                table: "IdtTrafficTickets",
                column: "FileId",
                principalTable: "IdtFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_IdtTrafficTickets_IdtProjects_ProjectId",
                table: "IdtTrafficTickets",
                column: "ProjectId",
                principalTable: "IdtProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtTrafficTickets_IdtEquipments_EquipmentId",
                table: "IdtTrafficTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtTrafficTickets_IdtFiles_FileId",
                table: "IdtTrafficTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_IdtTrafficTickets_IdtProjects_ProjectId",
                table: "IdtTrafficTickets");

            migrationBuilder.DropIndex(
                name: "IX_IdtTrafficTickets_EquipmentId",
                table: "IdtTrafficTickets");

            migrationBuilder.DropIndex(
                name: "IX_IdtTrafficTickets_FileId",
                table: "IdtTrafficTickets");

            migrationBuilder.DropIndex(
                name: "IX_IdtTrafficTickets_ProjectId",
                table: "IdtTrafficTickets");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectId",
                table: "IdtTrafficTickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FileId",
                table: "IdtTrafficTickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EquipmentId",
                table: "IdtTrafficTickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
