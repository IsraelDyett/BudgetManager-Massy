using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class EDITINGBUDGETDATABASE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "Budget",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuggestedChanges",
                table: "Budget",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "SuggestedChanges",
                table: "Budget");
        }
    }
}
