using System.ComponentModel.DataAnnotations;

namespace GameEconomy.API.Models;

public class IdempotencyRequest
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string IdempotencyKey { get; set; } = string.Empty;

    public string PlayerId { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string ResponseJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}