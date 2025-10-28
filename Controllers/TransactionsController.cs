using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionsController(
            ITransactionService transactionService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId!);
            return View(transactions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (transaction.UserId != userId)
            {
                return Forbid();
            }

            return View(transaction);
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
        public async Task<IActionResult> Create(Transaction transaction)
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
                return View(transaction);
            }

            transaction.UserId = _userManager.GetUserId(User)!;
            await _transactionService.CreateTransactionAsync(transaction);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (transaction.UserId != userId)
            {
                return Forbid();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                transaction.UserId = userId!;
                
                await _transactionService.UpdateTransactionAsync(transaction);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(transaction);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (transaction.UserId != userId)
            {
                return Forbid();
            }

            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
