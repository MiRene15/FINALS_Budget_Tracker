using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Controllers
{
    [Authorize]
    public class BudgetsController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BudgetsController(
            IBudgetService budgetService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager)
        {
            _budgetService = budgetService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var budgets = await _budgetService.GetBudgetsByUserIdAsync(userId!);
            return View(budgets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var budget = await _budgetService.GetBudgetByIdAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (budget.UserId != userId)
            {
                return Forbid();
            }

            var spent = await _budgetService.GetBudgetProgressAsync(id);
            ViewBag.Spent = spent;
            ViewBag.Remaining = budget.Amount - spent;
            ViewBag.Percentage = budget.Amount > 0 ? (spent / budget.Amount) * 100 : 0;

            return View(budget);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            // Remove server-populated fields from validation
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var error in ModelState)
                {
                    foreach (var errorMessage in error.Value.Errors)
                    {
                        // You can log this or return to debug
                    }
                }

                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return View(budget);
            }

            budget.UserId = _userManager.GetUserId(User)!;
            await _budgetService.CreateBudgetAsync(budget);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var budget = await _budgetService.GetBudgetByIdAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (budget.UserId != userId)
            {
                return Forbid();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                budget.UserId = userId!;
                
                await _budgetService.UpdateBudgetAsync(budget);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(budget);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var budget = await _budgetService.GetBudgetByIdAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (budget.UserId != userId)
            {
                return Forbid();
            }

            return View(budget);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _budgetService.DeleteBudgetAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
