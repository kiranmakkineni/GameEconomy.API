using GameEconomy.API.DTOs;
using GameEconomy.API.Models;

namespace GameEconomy.API.Services.Interfaces;

public interface IWalletService
{
    Task<Wallet> CreateWalletAsync(string playerId);

    Task<Wallet?> GetWalletAsync(string playerId);

    Task<WalletResponse> CreditAsync(
        string playerId,
        CreditWalletRequest request,
        string idempotencyKey);
   

    Task<WalletResponse> PurchaseAsync(
    string playerId,
    PurchaseRequest request,
    string idempotencyKey);

    Task ClaimRewardAsync(
    string rewardId,
    ClaimRewardRequest request);

    Task<PlayerStateResponse?> GetPlayerStateAsync(string playerId);

}