using BudgetManager.Models;
using BudgetManager.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace BudgetManager.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IWebHostEnvironment _webHostEnvironment;
        private readonly ICustomer _customer;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, ICustomer customer)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _customer = customer;
        }
       
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpPost]
        public IActionResult Index(IFormFile formFile)
        {
            string path = _customer.DocumentUpload(formFile);
            DataTable dt = _customer.CustomerTable(path);
            _customer.ImportCustomer(dt);
            return View(); 
        }
     
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}