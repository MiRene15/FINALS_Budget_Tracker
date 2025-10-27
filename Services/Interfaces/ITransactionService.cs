using FINALS_Budget_Tracker.Infrastructure.Entities;

namespace FINALS_Budget_Tracker.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(string userId);
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Transaction>> GetTransactionsByCategoryAsync(string userId, int categoryId);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> UpdateTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<decimal> GetTotalIncomeAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalExpensesAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetNetIncomeAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
