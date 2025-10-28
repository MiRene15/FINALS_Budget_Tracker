using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FINALS_Budget_Tracker.Infrastructure.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        [Required]
        public int CategoryId { get; set; }

        [BindNever]
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        [ValidateNever]
        public virtual Category Category { get; set; } = null!;
        [ValidateNever]
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public enum TransactionType
    {
        Income = 1,
        Expense = 2
    }
}
