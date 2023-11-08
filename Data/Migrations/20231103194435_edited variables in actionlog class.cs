using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class editedvariablesinactionlogclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "ActionLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "duration",
                table: "ActionLogs",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "ActionLogs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "controller",
                table: "ActionLogs",
                newName: "Controller");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "ActionLogs",
                newName: "Action");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ActionLogs",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "ActionLogs",
                newName: "duration");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ActionLogs",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "Controller",
                table: "ActionLogs",
                newName: "controller");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "ActionLogs",
                newName: "action");
        }
    }
}
