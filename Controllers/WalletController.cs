using GameEconomy.API.DTOs;
using GameEconomy.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameEconomy.API.Controllers;

[ApiController]
[Route("v1/wallets")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    // POST /v1/wallets/{playerId}/credit
    [HttpPost("{playerId}/credit")]
    public async Task<IActionResult> Credit(
    string playerId,
    [FromBody] CreditWalletRequest request,
    [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _walletService.CreditAsync(playerId, request, idempotencyKey);

        return Ok(result);
    }

    // GET /v1/wallets/{playerId}
    [HttpGet("{playerId}")]
    public async Task<IActionResult> GetWallet(string playerId)
    {
        var wallet = await _walletService.GetWalletAsync(playerId);

        if (wallet == null)
            return NotFound();

        return Ok(new WalletResponse
        {
            PlayerId = wallet.PlayerId,
            Balance = wallet.Balance
        });
    }
}