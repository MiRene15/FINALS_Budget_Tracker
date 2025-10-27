using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ITransactionService _transactionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(
            IReportService reportService,
            ITransactionService transactionService,
            UserManager<ApplicationUser> userManager)
        {
            _reportService = reportService;
            _transactionService = transactionService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var currentDate = DateTime.Now;
            
            // Get current month's report
            var monthlyReport = await _reportService.GenerateMonthlyReportAsync(
                userId!, 
                currentDate.Year, 
                currentDate.Month);

            ViewBag.CurrentYear = currentDate.Year;
            ViewBag.CurrentMonth = currentDate.Month;
            
            return View(monthlyReport);
        }

        [HttpPost]
        public async Task<IActionResult> MonthlyReport(int year, int month)
        {
            var userId = _userManager.GetUserId(User);
            var monthlyReport = await _reportService.GenerateMonthlyReportAsync(userId!, year, month);
            
            ViewBag.CurrentYear = year;
            ViewBag.CurrentMonth = month;
            
            return View("Index", monthlyReport);
        }

        public async Task<IActionResult> CategorySummary(DateTime? startDate, DateTime? endDate)
        {
            var userId = _userManager.GetUserId(User);
            
            var start = startDate ?? DateTime.Now.AddMonths(-1);
            var end = endDate ?? DateTime.Now;
            
            var categorySummaries = await _reportService.GetCategorySummariesAsync(userId!, start, end);
            
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            
            return View(categorySummaries);
        }

        public async Task<IActionResult> TopExpenses()
        {
            var userId = _userManager.GetUserId(User);
            var topExpenses = await _reportService.GetTopExpensesAsync(userId!);
            
            return View(topExpenses);
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var currentDate = DateTime.Now;
            
            // Get current month's data
            var monthlyReport = await _reportService.GenerateMonthlyReportAsync(
                userId!, 
                currentDate.Year, 
                currentDate.Month);

            // Get average monthly expense
            var averageExpense = await _reportService.GetAverageMonthlyExpenseAsync(userId!);
            
            // Get recent transactions
            var recentTransactions = await _transactionService.GetTransactionsByUserIdAndDateRangeAsync(
                userId!, 
                currentDate.AddDays(-30), 
                currentDate);

            ViewBag.AverageExpense = averageExpense;
            ViewBag.RecentTransactions = recentTransactions.Take(5);
            
            return View(monthlyReport);
        }
    }
}
