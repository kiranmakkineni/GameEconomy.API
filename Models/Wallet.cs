using System.ComponentModel.DataAnnotations;

namespace GameEconomy.API.Models;

public class Wallet
{
    [Key]
    public Guid Id { get; set; }

    public string PlayerId { get; set; } = string.Empty;

    public long Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}