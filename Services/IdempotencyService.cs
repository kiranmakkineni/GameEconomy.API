using GameEconomy.API.Data;
using GameEconomy.API.Models;
using GameEconomy.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameEconomy.API.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly AppDbContext _context;

    public IdempotencyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetResponseAsync(string idempotencyKey, string playerId)
    {
        var record = await _context.IdempotencyRequests
            .FirstOrDefaultAsync(x =>
                x.IdempotencyKey == idempotencyKey &&
                x.PlayerId == playerId);

        return record?.ResponseJson;
    }

    public async Task SaveResponseAsync(
    string idempotencyKey,
    string playerId,
    string endpoint,
    string response)
    {
        // 🔥 STEP 1: check first (prevents duplicate insert)
        var exists = await _context.IdempotencyRequests
            .AnyAsync(x =>
                x.IdempotencyKey == idempotencyKey &&
                x.PlayerId == playerId);

        if (exists)
            return;

        // 🔥 STEP 2: insert only if not exists
        _context.IdempotencyRequests.Add(new IdempotencyRequest
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            PlayerId = playerId,
            Endpoint = endpoint,
            ResponseJson = response,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}