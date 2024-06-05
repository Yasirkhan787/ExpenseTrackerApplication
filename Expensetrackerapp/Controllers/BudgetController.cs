using Expensetrackerapp.Data;
using Expensetrackerapp.Models;
using Expensetrackerapp.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expensetrackerapp.Controllers
{
    [Authorize(Roles = SD.Role_User)]
    public class BudgetController : Controller
    {
            private readonly ApplicationDbContext _context;

            public BudgetController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: Transaction
            public async Task<IActionResult> Index()
            {
                var applicationDbContext = _context.Budgets.Include(t => t.Category);
                return View(await applicationDbContext.ToListAsync());
            }

            // GET: Transaction/AddOrEdit
            public IActionResult AddOrEdit(int id = 0)
            {
                PopulateCategories();
                if (id == 0)
                    return View(new Budget());
                else
                    return View(_context.Budgets.Find(id));
            }

            // POST: Transaction/AddOrEdit
             [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddOrEdit([Bind("BudgetId,CategoryId,Amount,Note,StartDate,EndDate")] Budget budgets)
            {
                if (ModelState.IsValid)
                {
                    if (budgets.BudgetId == 0)
                        _context.Add(budgets);
                    else
                        _context.Update(budgets);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                PopulateCategories();
                return View(budgets);
            }

            // POST: Transaction/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                if (_context.Budgets == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Budget'  is null.");
                }
                var budgets = await _context.Budgets.FindAsync(id);
                if (budgets != null)
                {
                    _context.Budgets.Remove(budgets);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            [NonAction]
            public void PopulateCategories()
            {
                var CategoryCollection = _context.Categories.ToList();
                Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
                CategoryCollection.Insert(0, DefaultCategory);
                ViewBag.Categories = CategoryCollection;
            }
        }
    }