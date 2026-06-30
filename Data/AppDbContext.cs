using GameEconomy.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameEconomy.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Wallet> Wallets { get; set; }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    public DbSet<RewardClaim> RewardClaims { get; set; }

    public DbSet<IdempotencyRequest> IdempotencyRequests { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wallet>()
            .HasIndex(w => w.PlayerId)
            .IsUnique();

        modelBuilder.Entity<IdempotencyRequest>()
            .HasIndex(i => i.IdempotencyKey)
            .IsUnique();
    }
}