using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdeKusgozManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdtAdvance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecords_IdtProject_ProjectId",
                table: "IdtWorkRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdtProject",
                table: "IdtProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdtAdvance",
                table: "IdtAdvance");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "IdtAdvance");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "IdtAdvance");

            migrationBuilder.RenameTable(
                name: "IdtProject",
                newName: "IdtProjects");

            migrationBuilder.RenameTable(
                name: "IdtAdvance",
                newName: "IdtAdvances");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChiefProcessedAt",
                table: "IdtAdvances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "IdtAdvances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedByChiefId",
                table: "IdtAdvances",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedByUnitManagerId",
                table: "IdtAdvances",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "IdtAdvances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnitManagerProcessedAt",
                table: "IdtAdvances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdtProjects",
                table: "IdtProjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdtAdvances",
                table: "IdtAdvances",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecords_IdtProjects_ProjectId",
                table: "IdtWorkRecords",
                column: "ProjectId",
                principalTable: "IdtProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdtWorkRecords_IdtProjects_ProjectId",
                table: "IdtWorkRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdtProjects",
                table: "IdtProjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdtAdvances",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "ChiefProcessedAt",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "ProcessedByChiefId",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "ProcessedByUnitManagerId",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "IdtAdvances");

            migrationBuilder.DropColumn(
                name: "UnitManagerProcessedAt",
                table: "IdtAdvances");

            migrationBuilder.RenameTable(
                name: "IdtProjects",
                newName: "IdtProject");

            migrationBuilder.RenameTable(
                name: "IdtAdvances",
                newName: "IdtAdvance");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "IdtAdvance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "IdtAdvance",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdtProject",
                table: "IdtProject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdtAdvance",
                table: "IdtAdvance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdtWorkRecords_IdtProject_ProjectId",
                table: "IdtWorkRecords",
                column: "ProjectId",
                principalTable: "IdtProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
