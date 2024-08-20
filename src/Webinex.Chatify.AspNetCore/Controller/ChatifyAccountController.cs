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
    public async Task<IActionResult> GetAccountsAsync()
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.GetAccountsAsync());
    }
}
