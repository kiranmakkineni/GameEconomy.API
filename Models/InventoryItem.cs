namespace GameEconomy.API.Models;

public class InventoryItem
{
    public Guid Id { get; set; }

    public string PlayerId { get; set; } = string.Empty;

    public string ItemId { get; set; } = string.Empty;

    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
}