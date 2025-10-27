using Microsoft.EntityFrameworkCore;
using FINALS_Budget_Tracker.Infrastructure;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Services
{
    public class ReportService : IReportService
    {
        private readonly BudgetTrackerDbContext _context;

        public ReportService(BudgetTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<MonthlyReport> GenerateMonthlyReportAsync(string userId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();

            var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var totalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var netIncome = totalIncome - totalExpenses;

            var categorySummaries = await GetCategorySummariesAsync(userId, startDate, endDate);
            var topExpenses = await GetTopExpensesAsync(userId, 10);

            return new MonthlyReport
            {
                Year = year,
                Month = month,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetIncome = netIncome,
                CategorySummaries = categorySummaries,
                TopExpenses = topExpenses.Where(t => t.Date >= startDate && t.Date <= endDate).Take(10)
            };
        }

        public async Task<IEnumerable<CategorySummary>> GetCategorySummariesAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var categoryData = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && 
                           t.Type == TransactionType.Expense &&
                           t.Date >= startDate && 
                           t.Date <= endDate)
                .GroupBy(t => new { t.CategoryId, t.Category.Name, t.Category.Color })
                .Select(g => new CategorySummary
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    CategoryColor = g.Key.Color,
                    TotalAmount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToListAsync();

            var totalExpenses = categoryData.Sum(c => c.TotalAmount);

            foreach (var category in categoryData)
            {
                category.Percentage = totalExpenses > 0 ? (category.TotalAmount / totalExpenses) * 100 : 0;
            }

            return categoryData;
        }

        public async Task<decimal> GetTotalByCategoryAsync(string userId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && 
                           t.CategoryId == categoryId &&
                           t.Date >= startDate && 
                           t.Date <= endDate)
                .SumAsync(t => t.Amount);
        }

        public async Task<IEnumerable<Transaction>> GetTopExpensesAsync(string userId, int count = 10)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
                .OrderByDescending(t => t.Amount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageMonthlyExpenseAsync(string userId)
        {
            var monthlyExpenses = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => g.Sum(t => t.Amount))
                .ToListAsync();

            return monthlyExpenses.Any() ? monthlyExpenses.Average() : 0;
        }
    }
}
