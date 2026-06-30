using GameEconomy.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameEconomy.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<RewardClaim> RewardClaims { get; set; }
    public DbSet<IdempotencyRequest> IdempotencyRequests { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Wallet must be unique per player
        modelBuilder.Entity<Wallet>()
            .HasIndex(x => x.PlayerId)
            .IsUnique();

        //  FIXED: Idempotency MUST be scoped per player + key
        modelBuilder.Entity<IdempotencyRequest>()
            .HasIndex(x => new { x.PlayerId, x.IdempotencyKey })
            .IsUnique();

        //  Prevent duplicate inventory items
        modelBuilder.Entity<InventoryItem>()
            .HasIndex(x => new { x.PlayerId, x.ItemId })
            .IsUnique();

        // Prevent duplicate reward claims
        modelBuilder.Entity<RewardClaim>()
            .HasIndex(x => new { x.PlayerId, x.RewardId })
            .IsUnique();
    }
}