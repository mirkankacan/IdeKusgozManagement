using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyPaymentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenseType",
                table: "IdtExpenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IdtAuditLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdtCompanyPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpenseId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PersonnelNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChiefNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproverId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtCompanyPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtCompanyPayments_AspNetUsers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtCompanyPayments_IdtEquipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "IdtEquipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtCompanyPayments_IdtExpenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "IdtExpenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyPayments_ApproverId",
                table: "IdtCompanyPayments",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyPayments_EquipmentId",
                table: "IdtCompanyPayments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtCompanyPayments_ExpenseId",
                table: "IdtCompanyPayments",
                column: "ExpenseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdtAuditLogs");

            migrationBuilder.DropTable(
                name: "IdtCompanyPayments");

            migrationBuilder.DropColumn(
                name: "ExpenseType",
                table: "IdtExpenses");
        }
    }
}
