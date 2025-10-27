using FINALS_Budget_Tracker.Infrastructure.Entities;

namespace FINALS_Budget_Tracker.Services.Interfaces
{
    public interface IReportService
    {
        Task<MonthlyReport> GenerateMonthlyReportAsync(string userId, int year, int month);
        Task<IEnumerable<CategorySummary>> GetCategorySummariesAsync(string userId, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalByCategoryAsync(string userId, int categoryId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Transaction>> GetTopExpensesAsync(string userId, int count = 10);
        Task<decimal> GetAverageMonthlyExpenseAsync(string userId);
    }

    public class MonthlyReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
        public IEnumerable<CategorySummary> CategorySummaries { get; set; } = new List<CategorySummary>();
        public IEnumerable<Transaction> TopExpenses { get; set; } = new List<Transaction>();
    }

    public class CategorySummary
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
