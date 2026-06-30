using GameEconomy.API.Data;
using GameEconomy.API.DTOs;
using GameEconomy.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameEconomy.API.Services;

public class WalletService : IWalletService
{
    private readonly AppDbContext _context;

    public WalletService(AppDbContext context)
    {
        _context = context;
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

    public async Task<WalletResponse> CreditAsync(string playerId, CreditWalletRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

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

        await _context.SaveChangesAsync();

        return new WalletResponse
        {
            PlayerId = wallet.PlayerId,
            Balance = wallet.Balance
        };
    }


}