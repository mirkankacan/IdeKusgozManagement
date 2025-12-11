using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMachineWorkRecordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdtMachineWorkRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EquipmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InternalTransport = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdtMachineWorkRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdtMachineWorkRecords_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtMachineWorkRecords_AspNetUsers_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtMachineWorkRecords_IdtEquipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "IdtEquipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IdtMachineWorkRecords_IdtProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "IdtProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdtMachineWorkRecords_CreatedBy",
                table: "IdtMachineWorkRecords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_IdtMachineWorkRecords_Date",
                table: "IdtMachineWorkRecords",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_IdtMachineWorkRecords_EquipmentId",
                table: "IdtMachineWorkRecords",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtMachineWorkRecords_ProjectId",
                table: "IdtMachineWorkRecords",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IdtMachineWorkRecords_UpdatedBy",
                table: "IdtMachineWorkRecords",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdtMachineWorkRecords");
        }
    }
}
