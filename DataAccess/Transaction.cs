using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.DataAccess;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string TransactionType { get; set; } = string.Empty;

    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}