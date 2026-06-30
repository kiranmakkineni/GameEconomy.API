namespace GameEconomy.API.Models;

public class WalletTransaction
{
    public Guid Id { get; set; }

    public string PlayerId { get; set; } = string.Empty;

    public long Amount { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}