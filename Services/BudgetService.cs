using Microsoft.EntityFrameworkCore;
using FINALS_Budget_Tracker.Infrastructure;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly BudgetTrackerDbContext _context;

        public BudgetService(BudgetTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Budget>> GetBudgetsByUserIdAsync(string userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Budget>> GetActiveBudgetsAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && b.StartDate <= now && b.EndDate >= now)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<Budget?> GetBudgetByIdAsync(int id)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Budget> CreateBudgetAsync(Budget budget)
        {
            budget.CreatedAt = DateTime.UtcNow;
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task<Budget> UpdateBudgetAsync(Budget budget)
        {
            budget.UpdatedAt = DateTime.UtcNow;
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task<bool> DeleteBudgetAsync(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return false;

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetBudgetProgressAsync(int budgetId)
        {
            var budget = await _context.Budgets.FindAsync(budgetId);
            if (budget == null) return 0;

            var spent = await _context.Transactions
                .Where(t => t.CategoryId == budget.CategoryId && 
                           t.UserId == budget.UserId && 
                           t.Type == TransactionType.Expense &&
                           t.Date >= budget.StartDate && 
                           t.Date <= budget.EndDate)
                .SumAsync(t => t.Amount);

            return spent;
        }

        public async Task<IEnumerable<Budget>> GetBudgetsByCategoryAsync(string userId, int categoryId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && b.CategoryId == categoryId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }
    }
}
