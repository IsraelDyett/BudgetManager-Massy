namespace BudgetManager.ViewModels
{
    public class BudgetTemp
    {
        public float FinYr { get; set; }
        public float Yr { get; set; }
        public float StoreNo { get; set; }
        public string StoreName { get; set; }
        public string DeptName { get; set; }
        public float DeptNo { get; set; }

        public string Month { get; set; }
        public float MonthNo { get; set; }

        public float OperationalDays { get; set; }
        public float Week1 { get; set; }
        public float Week2 { get; set; }
        public float Week3 { get; set; }
        public float Week4 { get; set; }
        public float Week5 { get; set; }
        public float Week6 { get; set; }

        public float DailyGPP { get; set; }

        public float TotalGPP_AC { get; set; }
        public float? TotalGPP_YRAC { get; set; }
        public string? TotalGPP_BC { get; set; }
        public string TotalGPP_YRBC { get; set; }
        public decimal DailyGP { get; set; }
        public decimal DailySales { get; set; }
        public decimal Week1GP { get; set; }
        public decimal Week2GP { get; set; }
        public decimal Week3GP { get; set; }
        public decimal Week4GP { get; set; }
        public decimal Week5GP { get; set; }
        public decimal Week6GP { get; set; }
        public decimal Week1Sales { get; set; }
        public decimal Week2Sales { get; set; }
        public decimal Week3Sales { get; set; }
        public decimal Week4Sales { get; set; }
        public decimal Week5Sales { get; set; }
        public decimal Week6Sales { get; set; }
        public decimal MonthGP { get; set; }
        public decimal MonthSales { get; set; }
    }

}
