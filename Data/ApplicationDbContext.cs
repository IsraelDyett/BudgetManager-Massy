using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BudgetManager.Models;

namespace BudgetManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BudgetManager.Models.Budget>? Budget { get; set; }
        public DbSet<BudgetManager.Models.ActionLog>? ActionLogs { get; set; }

        public DbSet<BudgetManager.Models.Flag>? Flags { get; set; }


    }
}