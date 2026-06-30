namespace GameEconomy.API.DTOs
{
    public class WalletResponse
    {
        public string PlayerId { get; set; } = string.Empty;

        public long Balance { get; set; }
    }
}
