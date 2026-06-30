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
}