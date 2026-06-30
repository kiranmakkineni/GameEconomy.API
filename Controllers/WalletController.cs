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
        var playerState = await _walletService.GetPlayerStateAsync(playerId);

        if (playerState == null)
            return NotFound();

        return Ok(playerState);
    }
    // POST /v1/wallets/{playerId}/purchase
    [HttpPost("{playerId}/purchase")]
    public async Task<IActionResult> Purchase(
        string playerId,
        [FromBody] PurchaseRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _walletService.PurchaseAsync(
            playerId,
            request,
            idempotencyKey);

        return Ok(result);
    }

    // POST /v1/rewards/{rewardId}/claim
    [HttpPost("{rewardId}/claim")]
    public async Task<IActionResult> ClaimReward(
        string rewardId,
        [FromBody] ClaimRewardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _walletService.ClaimRewardAsync(rewardId, request);

        return Ok(new
        {
            Message = "Reward claimed successfully."
        });
    }

    
}