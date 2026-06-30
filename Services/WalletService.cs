using GameEconomy.API.Data;
using GameEconomy.API.DTOs;
using GameEconomy.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameEconomy.API.Services.Interfaces;

public class WalletService : IWalletService
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly AppDbContext _context;

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
            throw new ArgumentException("Amount must be greater than zero.");

        // 1. Check idempotency
        var cachedResponse = await _idempotencyService.GetResponseAsync(idempotencyKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            return System.Text.Json.JsonSerializer.Deserialize<WalletResponse>(cachedResponse)!;
        }

        // 2. Get or create wallet
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

        // 3. Update balance
        wallet.Balance += request.Amount;

        await _context.SaveChangesAsync();

        // 4. Prepare response
        var response = new WalletResponse
        {
            PlayerId = wallet.PlayerId,
            Balance = wallet.Balance
        };

        var responseJson = System.Text.Json.JsonSerializer.Serialize(response);

        // 5. Save idempotency record
        await _idempotencyService.SaveResponseAsync(
            idempotencyKey,
            playerId,
            "credit",
            responseJson
        );

        return response;
    }

}