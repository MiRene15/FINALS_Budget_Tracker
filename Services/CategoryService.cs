using Microsoft.EntityFrameworkCore;
using FINALS_Budget_Tracker.Infrastructure;
using FINALS_Budget_Tracker.Infrastructure.Entities;
using FINALS_Budget_Tracker.Services.Interfaces;

namespace FINALS_Budget_Tracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly BudgetTrackerDbContext _context;

        public CategoryService(BudgetTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Check if category has transactions or budgets
            var hasTransactions = await _context.Transactions.AnyAsync(t => t.CategoryId == id);
            var hasBudgets = await _context.Budgets.AnyAsync(b => b.CategoryId == id);

            if (hasTransactions || hasBudgets)
                return false; // Cannot delete category with related data

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}
