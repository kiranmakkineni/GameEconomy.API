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

    public async Task<string?> GetResponseAsync(string idempotencyKey)
    {
        var request = await _context.IdempotencyRequests
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);

        return request?.ResponseJson;
    }

    public async Task SaveResponseAsync(
    string idempotencyKey,
    string playerId,
    string endpoint,
    string response)
    {
        var existing = await _context.IdempotencyRequests
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey);

        if (existing != null)
            return;

        var request = new IdempotencyRequest
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            PlayerId = playerId,
            Endpoint = endpoint,
            ResponseJson = response,
            CreatedAt = DateTime.UtcNow
        };

        _context.IdempotencyRequests.Add(request);
        await _context.SaveChangesAsync();
    }
}