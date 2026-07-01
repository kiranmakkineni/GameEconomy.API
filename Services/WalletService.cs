using GameEconomy.API.Data;
using GameEconomy.API.DTOs;
using GameEconomy.API.Models;
using GameEconomy.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GameEconomy.API.Services;

public class WalletService : IWalletService
{
    private readonly AppDbContext _context;
    private readonly IIdempotencyService _idempotencyService;

    public WalletService(AppDbContext context, IIdempotencyService idempotencyService)
    {
        _context = context;
        _idempotencyService = idempotencyService;
    }
    public async Task<Wallet> CreateWalletAsync(string playerId)
    {
        var existing = await _context.Wallets
            .FirstOrDefaultAsync(w => w.PlayerId == playerId);

        if (existing != null)
            return existing;

        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            Balance = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return wallet;
    }
    public async Task<Wallet?> GetWalletAsync(string playerId)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(w => w.PlayerId == playerId);
    }
    public async Task<WalletResponse> CreditAsync(
    string playerId,
    CreditWalletRequest request,
    string idempotencyKey)
    {
        if (request.Amount <= 0)
            throw new Exception("Invalid amount");

        // Check for previous successful request
        var cached = await _idempotencyService.GetResponseAsync(idempotencyKey, playerId);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<WalletResponse>(cached)!;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.PlayerId == playerId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    PlayerId = playerId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Wallets.Add(wallet);
            }

            wallet.Balance += request.Amount;

            var response = new WalletResponse
            {
                PlayerId = playerId,
                Balance = wallet.Balance
            };

            await _idempotencyService.SaveResponseAsync(
                idempotencyKey,
                playerId,
                "credit",
                JsonSerializer.Serialize(response));

            // Save BOTH wallet and idempotency record together
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return response;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();

            var cachedResponse = await _idempotencyService
                .GetResponseAsync(idempotencyKey, playerId);

            if (!string.IsNullOrEmpty(cachedResponse))
                return JsonSerializer.Deserialize<WalletResponse>(cachedResponse)!;

            throw;
        }
        
    }

    public async Task<WalletResponse> PurchaseAsync(
    string playerId,
    PurchaseRequest request,
    string idempotencyKey)
    {
        var cached = await _idempotencyService.GetResponseAsync(idempotencyKey, playerId);

        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<WalletResponse>(cached)!;

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE Wallets
                        SET Balance = Balance - {request.Price}
                        WHERE PlayerId = {playerId}
                        AND Balance >= {request.Price};
                        ");

            if (rows == 0)
                throw new InvalidOperationException("Insufficient balance.");

            _context.InventoryItems.Add(new InventoryItem
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                ItemId = request.ItemId
            });

            var wallet = await _context.Wallets
                .FirstAsync(w => w.PlayerId == playerId);

            var response = new WalletResponse
            {
                PlayerId = playerId,
                Balance = wallet.Balance
            };

            await _idempotencyService.SaveResponseAsync(
                idempotencyKey,
                playerId,
                "purchase",
                JsonSerializer.Serialize(response));

            // Save wallet update, inventory item, and idempotency record together
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return response;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();

            var cachedResponse = await _idempotencyService
                .GetResponseAsync(idempotencyKey, playerId);

            if (!string.IsNullOrEmpty(cachedResponse))
                return JsonSerializer.Deserialize<WalletResponse>(cachedResponse)!;

            throw;
        }
        
    }

    public async Task ClaimRewardAsync(
    string rewardId,
    ClaimRewardRequest request)
    {
        var alreadyClaimed = await _context.RewardClaims
            .AnyAsync(x =>
                x.PlayerId == request.PlayerId &&
                x.RewardId == rewardId);

        if (alreadyClaimed)
        {
            throw new InvalidOperationException("Reward already claimed.");
        }

        _context.RewardClaims.Add(new RewardClaim
        {
            Id = Guid.NewGuid(),
            PlayerId = request.PlayerId,
            RewardId = rewardId,
            ClaimedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task<PlayerStateResponse?> GetPlayerStateAsync(string playerId)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.PlayerId == playerId);
        if (wallet == null) return null;

        var inventory = await _context.InventoryItems
            .Where(x => x.PlayerId == playerId)
            .Select(x => x.ItemId)
            .ToListAsync();

        var rewards = await _context.RewardClaims
            .Where(x => x.PlayerId == playerId)
            .Select(x => x.RewardId)
            .ToListAsync();

        return new PlayerStateResponse
        {
            Balance = wallet.Balance,
            Inventory = inventory,
            ClaimedRewards = rewards
        };
    }
}