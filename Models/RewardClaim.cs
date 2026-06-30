namespace GameEconomy.API.Models;

public class RewardClaim
{
    public Guid Id { get; set; }

    public string PlayerId { get; set; } = string.Empty;

    public string RewardId { get; set; } = string.Empty;

    public DateTime ClaimedAt { get; set; } = DateTime.UtcNow;
}