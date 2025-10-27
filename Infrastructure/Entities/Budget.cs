using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINALS_Budget_Tracker.Infrastructure.Entities
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
