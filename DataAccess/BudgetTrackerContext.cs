using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.DataAccess;

public class BudgetTrackerContext : DbContext
{
    public BudgetTrackerContext(
        DbContextOptions<BudgetTrackerContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;
}