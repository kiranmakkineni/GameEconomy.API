namespace GameEconomy.API.Services.Interfaces;

public interface IIdempotencyService
{
    Task<string?> GetResponseAsync(string idempotencyKey);

    Task SaveResponseAsync(
        string idempotencyKey,
        string playerId,
        string endpoint,
        string response);
}