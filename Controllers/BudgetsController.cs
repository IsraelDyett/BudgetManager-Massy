using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetManager.Data;
using BudgetManager.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Data.SqlClient;
using BudgetManager.ViewModels;
using ExcelDataReader;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BudgetManager.Controllers
{
    [ServiceFilter(typeof(LogAttribute))]
    [Authorize]
    public class BudgetsController : Controller
    {
        private  ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BudgetsController(ApplicationDbContext context,   IConfiguration configuration)
        {
            _context = context;
           
            _configuration = configuration;
         
        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            return _context.Budget != null ?
                        View(await _context.Budget.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Budget'  is null.");
        }
        public async Task<IActionResult> ShowSearchForm()
        {
            return _context.Budget != null ?
                        View("ShowSeachForm") :
                        Problem("Entity set 'ApplicationDbContext.Budget'  is null.");
        }
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            if (_context.Budget != null)
            {
                var searchResults = await _context.Budget
                    .Where(b => b.StoreName == SearchPhrase || b.DeptName == SearchPhrase || b.Month == SearchPhrase)
                    .ToListAsync();

                return View("Index", searchResults);
            }
            else
            {
                return Problem("Entity set 'ApplicationDbContext.Budget' is null.");
            }
        }

        // GET: Budgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Budget == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget
                .FirstOrDefaultAsync(m => m.BudgetID == id);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }
        [Authorize (Roles = "SuperAdmin,Admin,Master")]
        // GET: Budgets/Create
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Admin,Master")]
        // POST: Budgets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BudgetID,FinYr,Yr,StoreNo,StoreName,DeptName,DeptNo,Month,MonthNo,OperationalDays,Week1,Week2,Week3,Week4,Week5,Week6,DailyGPP,TotalGPP_AC,TotalGPP_YRAC,TotalGPP_BC,TotalGPP_YRBC,DailyGP,DailySales,Week1GP,Week2GP,Week3GP,Week4GP,Week5GP,Week6GP,Week1Sales,Week2Sales,Week3Sales,Week4Sales,Week5Sales,Week6Sales,MonthGP,MonthSales")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                var existingBudget = _context.Budget.FirstOrDefault(b =>
                                                                 b.FinYr == budget.FinYr &&
                                                                 b.MonthNo == budget.MonthNo &&
                                                                 b.StoreNo == budget.StoreNo &&
                                                                 b.DeptNo == budget.DeptNo);

                if (existingBudget == null)
                {
                    // If no existing record is found, add the new budget
                    _context.Add(budget);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle the case when an existing record with the same composite key is found
                    ModelState.AddModelError(string.Empty, "This Budget already exists.");
                }
            }
            return View(budget);
        }
        [Authorize(Roles = "SuperAdmin,Admin,Master")]
        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Budget == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }
            return View(budget);
        }
        [Authorize(Roles = "SuperAdmin,Admin,Master")]
        // POST: Budgets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BudgetID,FinYr,Yr,StoreNo,StoreName,DeptName,DeptNo,Month,MonthNo,OperationalDays,Week1,Week2,Week3,Week4,Week5,Week6,DailyGPP,TotalGPP_AC,TotalGPP_YRAC,TotalGPP_BC,TotalGPP_YRBC,DailyGP,DailySales,Week1GP,Week2GP,Week3GP,Week4GP,Week5GP,Week6GP,Week1Sales,Week2Sales,Week3Sales,Week4Sales,Week5Sales,Week6Sales,MonthGP,MonthSales")] Budget budget)
        {
            if (id != budget.BudgetID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.BudgetID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(budget);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Budget == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget
                .FirstOrDefaultAsync(m => m.BudgetID == id);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Budget == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Budget'  is null.");
            }
            var budget = await _context.Budget.FindAsync(id);
            if (budget != null)
            {
                _context.Budget.Remove(budget);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin,Admin,Master")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    var records = csv.GetRecords<BudgetTemp>().ToList();
                    
                    List<Budget> budgetList = new List<Budget>();
                    foreach (var record in records)
                    {
                        Budget budget = new Budget();
                        budget.FinYr = record.FinYr;
                        budget.Yr = record.Yr;
                        budget.StoreNo = record.StoreNo;
                        budget.StoreName = record.StoreName;
                        budget.DeptName = record.DeptName;
                        budget.DeptNo = record.DeptNo;
                        budget.Month= record.Month;
                        budget.MonthNo = record.MonthNo;
                        budget.OperationalDays = record.OperationalDays;
                        budget.Week1 = record.Week1;
                        budget.Week2 = record.Week2;
                        budget.Week3 = record.Week3;
                        budget.Week4 = record.Week4;
                        budget.Week5 = record.Week5;
                        budget.Week6 = record.Week6;
                        budget.DailyGPP = record.DailyGPP;
                        budget.TotalGPP_AC= record.TotalGPP_AC;
                        budget.TotalGPP_YRAC = record.TotalGPP_YRAC;
                        budget.TotalGPP_BC = record.TotalGPP_BC;
                        budget.TotalGPP_YRBC=record.TotalGPP_YRBC;
                        budget.DailyGP= record.DailyGP;
                        budget.DailySales= record.DailySales;
                        budget.Week1GP = record.Week1GP;
                        budget.Week2GP = record.Week2GP; 
                        budget.Week3GP = record.Week3GP;
                        budget.Week4GP = record.Week4GP; 
                        budget.Week5GP = record.Week5GP;
                        budget.Week6GP = record.Week6GP;
                        budget.Week1Sales = record.Week1Sales;
                        budget.Week2Sales = record.Week2Sales;
                        budget.Week3Sales = record.Week3Sales;
                        budget.Week4Sales = record.Week4Sales;
                        budget.Week5Sales = record.Week5Sales;
                        budget.Week6Sales = record.Week6Sales;
                        budget.MonthGP = record.MonthGP;
                        budget.MonthSales = record.MonthSales;

                        budgetList.Add(budget);

                       // budget.BudgetID = 0; // You can also set it to null if it's configured as nullable in your model
                    }
                    //  _context.Budget.AddRange(budgetList);
                    //  _context.SaveChanges();
                    return View("PreviewEditable", budgetList);

                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveToDatabase(List<Budget> budgets)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var budget in budgets)
                    {

                        var existingBudget = _context.Budget.FirstOrDefault(b =>
                                                             b.FinYr == budget.FinYr &&
                                                             b.MonthNo == budget.MonthNo &&
                                                             b.StoreNo == budget.StoreNo &&
                                                             b.DeptNo == budget.DeptNo);

                        if (existingBudget != null)
                        {

                            continue;
                        }
                        else
                        {
                            _context.Add(budget);
                        }


                    }

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        return View("CSVFileError");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle exceptions as needed
                }
                return RedirectToAction(nameof(Index));
            }

            return View(budgets);
        }

        [Authorize(Roles = "SuperAdmin,Admin,Master")]
        public IActionResult PreviewEditable(List<Budget> budgets)
        {
            // Display the uploaded budgets in the editable view
            return View(budgets);
        }

        private bool BudgetExists(int id)
        {
          return (_context.Budget?.Any(e => e.BudgetID == id)).GetValueOrDefault();
        }



        public async Task AddFlaggedBudget(int budgetID)
        {
            // Check if the budget is already flagged
            if (!_context.Flags.Any(fb => fb.BudgetID == budgetID))
            {
                var flaggedBudget = new Flag
                {
                    BudgetID = budgetID
                };

                _context.Flags.Add(flaggedBudget);
                await _context.SaveChangesAsync();
            }
        }


        [HttpPost]
        public async Task<IActionResult> FlagBudget(int budgetID)
        {
            try
            {
                await AddFlaggedBudget(budgetID);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle the error, perhaps by displaying an error message
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetFlaggedBudgets()
        {
            
            var flaggedBudgetIDs = _context.Flags.Select(fb => fb.BudgetID).ToList();

            // Select the budgets from the Budget table where the BudgetID is in the list of flagged IDs
            var flaggedBudgets = _context.Budget.Where(b => flaggedBudgetIDs.Contains(b.BudgetID)).ToList();

            return View("Index", flaggedBudgets);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteFlaggedBudget(int flaggedID)
        {
            // Find the flagged budget by flaggedID
            var flaggedBudget = await _context.Flags.FindAsync(flaggedID);

            if (flaggedBudget == null)
            {
                // Handle not found
                return RedirectToAction("Index");
            }

            // Remove the flagged budget from the context and save changes to the database
            _context.Flags.Remove(flaggedBudget);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Redirect back to the list of flagged budgets
        }
    }
}
