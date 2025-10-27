using FINALS_Budget_Tracker.Infrastructure.Entities;

namespace FINALS_Budget_Tracker.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(string userId);
        Task<IEnumerable<Budget>> GetActiveBudgetsAsync(string userId);
        Task<Budget?> GetBudgetByIdAsync(int id);
        Task<Budget> CreateBudgetAsync(Budget budget);
        Task<Budget> UpdateBudgetAsync(Budget budget);
        Task<bool> DeleteBudgetAsync(int id);
        Task<decimal> GetBudgetProgressAsync(int budgetId);
        Task<IEnumerable<Budget>> GetBudgetsByCategoryAsync(string userId, int categoryId);
    }
}
