namespace GameEconomy.API.DTOs;

public class PlayerStateResponse
{
    public long Balance { get; set; }

    public List<string> Inventory { get; set; } = new();

    public List<string> ClaimedRewards { get; set; } = new();
}