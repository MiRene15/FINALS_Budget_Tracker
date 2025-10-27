using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FINALS_Budget_Tracker.Infrastructure.Entities;

namespace FINALS_Budget_Tracker.Infrastructure
{
    public class BudgetTrackerDbContext : IdentityDbContext<ApplicationUser>
    {
        public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Category entity
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(7);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure Transaction entity
            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Type).HasConversion<int>();

                // Foreign key relationships
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Transactions)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Budget entity
            builder.Entity<Budget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).HasMaxLength(500);

                // Foreign key relationships
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Budgets)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Budgets)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ApplicationUser entity
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            });

            // Seed initial data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed default categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food & Dining", Description = "Restaurants, groceries, and food expenses", Color = "#FF6B6B" },
                new Category { Id = 2, Name = "Transportation", Description = "Gas, public transport, car maintenance", Color = "#4ECDC4" },
                new Category { Id = 3, Name = "Entertainment", Description = "Movies, games, hobbies", Color = "#45B7D1" },
                new Category { Id = 4, Name = "Utilities", Description = "Electricity, water, internet bills", Color = "#96CEB4" },
                new Category { Id = 5, Name = "Healthcare", Description = "Medical expenses, insurance", Color = "#FFEAA7" },
                new Category { Id = 6, Name = "Shopping", Description = "Clothing, electronics, general shopping", Color = "#DDA0DD" },
                new Category { Id = 7, Name = "Income", Description = "Salary, freelance, investments", Color = "#98D8C8" },
                new Category { Id = 8, Name = "Savings", Description = "Emergency fund, investments", Color = "#F7DC6F" }
            );
        }
    }
}
