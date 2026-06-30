namespace GameEconomy.API.DTOs
{
    public class CreditWalletRequest
    {
        public long Amount { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
