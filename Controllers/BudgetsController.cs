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
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.UserId = _userManager.GetUserId(User)!;
                await _budgetService.CreateBudgetAsync(budget);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(budget);
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

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
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

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
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
