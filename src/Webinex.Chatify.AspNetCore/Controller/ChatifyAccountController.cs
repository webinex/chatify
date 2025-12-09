using Microsoft.AspNetCore.Mvc;
using Webinex.Coded.AspNetCore;

namespace Webinex.Chatify.AspNetCore;

[Route("/api/chatify")]
internal class ChatifyAccountController : ControllerBase
{
    private readonly IChatifyAspNetCoreService _chatifyAspNetCoreService;

    public ChatifyAccountController(IChatifyAspNetCoreService chatifyAspNetCoreService)
    {
        _chatifyAspNetCoreService = chatifyAspNetCoreService;
    }

    [HttpGet("account")]
    public async Task<IActionResult> GetAccountsAsync([FromQuery] IEnumerable<string>? ids = null)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.GetAccountsAsync(ids: ids));
    }

    [HttpGet("account/me")]
    public async Task<IActionResult> GetCurrentUserAccountAsync()
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.GetCurrentUserAccountAsync());
    }

    [HttpPut("account/{accountId}")]
    public async Task<IActionResult> UpdateAccountAsync(string accountId, [FromBody] UpdateAccountRequestDto request)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.UpdateAccountAsync(accountId, request));
    }
}
