namespace GameEconomy.API.DTOs;

public class PurchaseRequest
{
    public string ItemId { get; set; } = string.Empty;

    public long Price { get; set; }
}