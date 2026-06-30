using GameEconomy.API.DTOs;
using GameEconomy.API.Models;

namespace GameEconomy.API.Services;

public interface IWalletService
{
    Task<Wallet> CreateWalletAsync(string playerId);

    Task<Wallet?> GetWalletAsync(string playerId);

    Task<WalletResponse> CreditAsync(string playerId, CreditWalletRequest request);
}