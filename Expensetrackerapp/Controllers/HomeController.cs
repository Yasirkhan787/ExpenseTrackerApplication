using Expensetrackerapp.Data;
using Expensetrackerapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace Expensetrackerapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }

        public async Task<ActionResult> Index()
        {
            // Last 30 Days Record 

            DateTime StartDate = DateTime.Today.AddDays(-30);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                 .Include(x => x.Category)
                 .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                 .ToListAsync();

             //Total Income
             int TotalIncome = SelectedTransactions
                 .Where(i => i.Category.Type == "Income")
                 .Sum(j => j.Amount);
             ViewBag.TotalIncome = TotalIncome.ToString("C0");

             //Total Expense
             int TotalExpense = SelectedTransactions
                 .Where(i => i.Category.Type == "Expense")
                 .Sum(j => j.Amount);
             ViewBag.TotalExpense = TotalExpense.ToString("C0");


             //Balance
             int Balance = TotalIncome - TotalExpense;
             CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
             culture.NumberFormat.CurrencyNegativePattern = 1;
             ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

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
