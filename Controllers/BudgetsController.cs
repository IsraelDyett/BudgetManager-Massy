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
using OfficeOpenXml;
using System.ComponentModel;

namespace BudgetManager.Controllers
{
    [ServiceFilter(typeof(LogAttribute))]
    [Authorize]
    public class BudgetsController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BudgetsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;

            _configuration = configuration;

        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            int approvedCount = _context.Budget.Count(b => b.Accepted == true);
            int unApprovedCount = _context.Budget.Count(b => b.Accepted == null);
            int flaggedCount = _context.Budget.Count(b => b.Accepted == false);

            // Pass the counts to the view
            ViewBag.ApprovedBudgetCount = approvedCount;
            ViewBag.UnApprovedBudgetCount = unApprovedCount;
            ViewBag.FlaggedBudgetCount = flaggedCount;

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
        [Authorize(Roles = "SuperAdmin,Admin,Master")]
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
        public async Task<IActionResult> Create([Bind("BudgetID,FinYr,Yr,StoreNo,StoreName,DeptName,DeptNo,Month,MonthNo,OperationalDays,Week1,Week2,Week3,Week4,Week5,Week6,DailyGPP,TotalGPP_AC,TotalGPP_YRAC,TotalGPP_BC,TotalGPP_YRBC,DailyGP,DailySales,Week1GP,Week2GP,Week3GP,Week4GP,Week5GP,Week6GP,Week1Sales,Week2Sales,Week3Sales,Week4Sales,Week5Sales,Week6Sales,MonthGP,MonthSales,SuggestedChanges,Accepted")] Budget budget)
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
                    // budget.SuggestedChanges = "inital";
                    budget.Accepted = null;
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
                        budget.Month = record.Month;
                        budget.MonthNo = record.MonthNo;
                        budget.OperationalDays = record.OperationalDays;
                        budget.Week1 = record.Week1;
                        budget.Week2 = record.Week2;
                        budget.Week3 = record.Week3;
                        budget.Week4 = record.Week4;
                        budget.Week5 = record.Week5;
                        budget.Week6 = record.Week6;
                        budget.DailyGPP = record.DailyGPP;
                        budget.TotalGPP_AC = record.TotalGPP_AC;
                        budget.TotalGPP_YRAC = record.TotalGPP_YRAC;
                        budget.TotalGPP_BC = record.TotalGPP_BC;
                        budget.TotalGPP_YRBC = record.TotalGPP_YRBC;
                        budget.DailyGP = record.DailyGP;
                        budget.DailySales = record.DailySales;
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
                        budget.SuggestedChanges = null;
                        budget.Accepted = null;
                        budgetList.Add(budget);

                        // budget.BudgetID = 0; // You can also set it to null if it's configured as nullable in your model
                    }
                    //  _context.Budget.AddRange(budgetList);
                    //  _context.SaveChanges();
                    return View("PreviewEditable", budgetList);

                }
            }
            else
            {
                ViewData["File Upload Error"] = "Please Select A CSV File To Upload.";
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
                            existingBudget.FinYr = budget.FinYr;
                            existingBudget.Yr = budget.Yr;
                            existingBudget.StoreNo = budget.StoreNo;
                            existingBudget.StoreName = budget.StoreName;
                            existingBudget.DeptName = budget.DeptName;
                            existingBudget.DeptNo = budget.DeptNo;
                            existingBudget.Month = budget.Month;
                            existingBudget.MonthNo = budget.MonthNo;
                            existingBudget.OperationalDays = budget.OperationalDays;
                            existingBudget.Week1 = budget.Week1;
                            existingBudget.Week2 = budget.Week2;
                            existingBudget.Week3 = budget.Week3;
                            existingBudget.Week4 = budget.Week4;
                            existingBudget.Week5 = budget.Week5;
                            existingBudget.Week6 = budget.Week6;
                            existingBudget.DailyGPP = budget.DailyGPP;
                            existingBudget.TotalGPP_AC = budget.TotalGPP_AC;
                            existingBudget.TotalGPP_YRAC = budget.TotalGPP_YRAC;
                            existingBudget.TotalGPP_BC = budget.TotalGPP_BC;
                            existingBudget.TotalGPP_YRBC = budget.TotalGPP_YRBC;
                            existingBudget.DailyGP = budget.DailyGP;
                            existingBudget.DailySales = budget.DailySales;
                            existingBudget.Week1GP = budget.Week1GP;
                            existingBudget.Week2GP = budget.Week2GP;
                            existingBudget.Week3GP = budget.Week3GP;
                            existingBudget.Week4GP = budget.Week4GP;
                            existingBudget.Week5GP = budget.Week5GP;
                            existingBudget.Week6GP = budget.Week6GP;
                            existingBudget.Week1Sales = budget.Week1Sales;
                            existingBudget.Week2Sales = budget.Week2Sales;
                            existingBudget.Week3Sales = budget.Week3Sales;
                            existingBudget.Week4Sales = budget.Week4Sales;
                            existingBudget.Week5Sales = budget.Week5Sales;
                            existingBudget.Week6Sales = budget.Week6Sales;
                            existingBudget.MonthGP = budget.MonthGP;
                            existingBudget.MonthSales = budget.MonthSales;
                            existingBudget.SuggestedChanges = null;
                            existingBudget.Accepted = null;
                            
                            _context.Update(existingBudget);
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
                        
                        ViewData["Duplicate Error"] = "There Are Duplicates Budgets Present. Please Adjust The Values Accondingly Before Saving.";
                        
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





        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> SuggestBudgetChanges(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budget.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            return View(budget); // Pass the budget model to the view.
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuggestBudgetChanges(int id, Budget budget)
        {
            
                var existingBudget = _context.Budget.FirstOrDefault(b => b.BudgetID == id);

                if (existingBudget != null)
                {
                    existingBudget.SuggestedChanges = budget.SuggestedChanges;
                    existingBudget.Accepted = false;
                    _context.Update(existingBudget);
                }

                await _context.SaveChangesAsync();

            
            return await GetUnApproved();
        }



        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptBudget(int id)
        {
            var budget = await _context.Budget.FindAsync(id);

            if (budget != null)
            {
                budget.SuggestedChanges = "";
                budget.Accepted = true;
                _context.Update(budget);
                await _context.SaveChangesAsync();
            }

            return await GetUnApproved();
        }




        [HttpPost]
        public async Task<IActionResult> GetAcceptedBudgets()
        {
            int approvedCount = _context.Budget.Count(b => b.Accepted == true);
            int unApprovedCount = _context.Budget.Count(b => b.Accepted == null);
            int flaggedCount = _context.Budget.Count(b => b.Accepted == false);

            // Pass the counts to the view
            ViewBag.ApprovedBudgetCount = approvedCount;
            ViewBag.UnApprovedBudgetCount = unApprovedCount;
            ViewBag.FlaggedBudgetCount = flaggedCount;

            // Select the budgets from the Budget table where the BudgetID is in the list of flagged IDs
            var flaggedBudgets = _context.Budget.Where(b => b.Accepted == true).ToList();

            return View("Index", flaggedBudgets);
        }





        [HttpPost]
        public async Task<IActionResult> GetFlaggedBudgets()
        {

            int approvedCount = _context.Budget.Count(b => b.Accepted == true);
            int unApprovedCount = _context.Budget.Count(b => b.Accepted == null);
            int flaggedCount = _context.Budget.Count(b => b.Accepted == false);

            // Pass the counts to the view
            ViewBag.ApprovedBudgetCount = approvedCount;
            ViewBag.UnApprovedBudgetCount = unApprovedCount;
            ViewBag.FlaggedBudgetCount = flaggedCount;

            // Select the budgets from the Budget table where the BudgetID is in the list of flagged IDs
            var flaggedBudgets = _context.Budget.Where(b => b.Accepted == false).ToList();

            return View("Index", flaggedBudgets);
        }

        [HttpPost]
        public async Task<IActionResult> GetUnApproved()
        {
            var flaggedBudgets = new List<Budget>();
            if (_context.Budget != null)
            {
                int approvedCount = _context.Budget.Count(b => b.Accepted == true);
                int unApprovedCount = _context.Budget.Count(b => b.Accepted == null);
                int flaggedCount = _context.Budget.Count(b => b.Accepted == false); 

                ViewBag.ApprovedBudgetCount = approvedCount;
                ViewBag.UnApprovedBudgetCount = unApprovedCount;
                ViewBag.FlaggedBudgetCount = flaggedCount;

                flaggedBudgets = await _context.Budget.Where(b => b.Accepted != true && b.Accepted != false).ToListAsync();
            }
          
            return  View("Index", flaggedBudgets);  

        }

        public IActionResult ExportToExcel(string selectedBudgets)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
           if(selectedBudgets != null) { 
            // Parse the selected budget IDs from the comma-separated string
            var selectedBudgetIds = selectedBudgets.Split(',').Select(int.Parse).ToList();
            
            // Filter budgets based on selected IDs
            var budgets = _context.Budget.Where(b => selectedBudgetIds.Contains(b.BudgetID)).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Budgets");

                // Add column headers
                int col = 1;
                foreach (var property in typeof(Budget).GetProperties())
                {
                    if ((col != 1) && (col != 39) && (col != 38))
                    {
                        worksheet.Cells[1, col].Value = property.Name;
                    }
                    col++;
                }

                // Add data rows
                int row = 2;
                foreach (var budget in budgets)
                {
                    col = 1;
                    foreach (var property in typeof(Budget).GetProperties())
                    {
                        if ((col != 1) && (col != 39) && (col != 38))
                        {
                            worksheet.Cells[row, col].Value = property.GetValue(budget);
                        }
                        col++;
                    }
                    row++;
                }

                // Save the Excel package
                var content = package.GetAsByteArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "SelectedBudgetsExport.xlsx";

                return File(content, contentType, fileName);
            }
            
            }
            return RedirectToAction("Index", "Home");
        }




        [HttpPost]
        public IActionResult ApproveSelectedBudgets(string SelectedBudgets)
        {
            if(SelectedBudgets != null) {
                List<int> selectedBudgets = SelectedBudgets.Split(',').Select(int.Parse).ToList();
                // Retrieve the selected budgets
                var budgetsToApprove = _context.Budget.Where(b => selectedBudgets.Contains(b.BudgetID)).ToList();

                // Update the budgets
                foreach (var budget in budgetsToApprove)
                {
                    budget.Accepted = true;
                    budget.SuggestedChanges = null;
                }
                _context.Budget.UpdateRange(budgetsToApprove);
                // Save the changes
                _context.SaveChanges();
             }
                // Redirect to the Index view
            return RedirectToAction("Index");
        }


        public IActionResult FlagSelectedBudgets(string SelectedBudgets)
        {
            if (SelectedBudgets != null) { 
                List<int> selectedBudgets = SelectedBudgets.Split(',').Select(int.Parse).ToList();
                // Retrieve the selected budgets
                var budgetsToApprove = _context.Budget.Where(b => selectedBudgets.Contains(b.BudgetID)).ToList();

                // Update the budgets
                foreach (var budget in budgetsToApprove)
                {
                    budget.Accepted = false;
                    budget.SuggestedChanges = "Double check the values in this budget.";

                }
                _context.Budget.UpdateRange(budgetsToApprove);
                // Save the changes
                _context.SaveChanges();
                }
            // Redirect to the Index view
            return RedirectToAction("Index");
        }

        public IActionResult EditBudgets(string SelectedBudgets)
        {
            if (SelectedBudgets != null)
            {
                List<int> selectedBudgets = SelectedBudgets.Split(',').Select(int.Parse).ToList();
                // Retrieve the selected budgets
                var budgetList = _context.Budget.Where(b => selectedBudgets.Contains(b.BudgetID)).ToList();
                return View("PreviewEditable", budgetList);
                
            }
            // Redirect to the Index view
            return RedirectToAction("Index");
        }

    }
}
