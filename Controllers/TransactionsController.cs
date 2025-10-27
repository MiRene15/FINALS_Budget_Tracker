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
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.UserId = _userManager.GetUserId(User)!;
                await _transactionService.CreateTransactionAsync(transaction);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(transaction);
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

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
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

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
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
