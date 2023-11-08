using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class initialsetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    BudgetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinYr = table.Column<float>(type: "real", nullable: false),
                    Yr = table.Column<float>(type: "real", nullable: false),
                    StoreNo = table.Column<float>(type: "real", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeptName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeptNo = table.Column<float>(type: "real", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthNo = table.Column<float>(type: "real", nullable: false),
                    OperationalDays = table.Column<float>(type: "real", nullable: false),
                    Week1 = table.Column<float>(type: "real", nullable: false),
                    Week2 = table.Column<float>(type: "real", nullable: false),
                    Week3 = table.Column<float>(type: "real", nullable: false),
                    Week4 = table.Column<float>(type: "real", nullable: false),
                    Week5 = table.Column<float>(type: "real", nullable: false),
                    Week6 = table.Column<float>(type: "real", nullable: false),
                    DailyGPP = table.Column<float>(type: "real", nullable: false),
                    TotalGPP_AC = table.Column<float>(type: "real", nullable: false),
                    TotalGPP_YRAC = table.Column<float>(type: "real", nullable: false),
                    TotalGPP_BC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalGPP_YRBC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DailyGP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailySales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week1GP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week2GP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week3GP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week4GP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week5GP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week6GP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week1Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week2Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week3Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week4Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week5Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Week6Sales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthGP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget");
        }
    }
}
