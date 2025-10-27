using Microsoft.EntityFrameworkCore;
using FINALS_Budget_Tracker.Infrastructure;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BudgetTrackerDbContext _context;

        public TransactionService(BudgetTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(string userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAndDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByCategoryAsync(string userId, int categoryId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.CategoryId == categoryId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalIncomeAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            return await query.SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpensesAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            return await query.SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetNetIncomeAsync(string userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var income = await GetTotalIncomeAsync(userId, startDate, endDate);
            var expenses = await GetTotalExpensesAsync(userId, startDate, endDate);
            return income - expenses;
        }
    }
}
