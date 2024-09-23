using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class updatingActionLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ActionLogs",
                newName: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "ActionLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "ActionLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoneOn",
                table: "ActionLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field",
                table: "ActionLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "DoneOn",
                table: "ActionLogs");

            migrationBuilder.DropColumn(
                name: "Field",
                table: "ActionLogs");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "ActionLogs",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "ActionLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
