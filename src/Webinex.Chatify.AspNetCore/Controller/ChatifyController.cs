using Microsoft.AspNetCore.Mvc;
using Webinex.Asky;
using Webinex.Coded.AspNetCore;

namespace Webinex.Chatify.AspNetCore;

[Route("/api/chatify")]
public class ChatifyController : ControllerBase
{
    private readonly IChatifyAspNetCoreService _chatifyAspNetCoreService;

    public ChatifyController(IChatifyAspNetCoreService chatifyAspNetCoreService)
    {
        _chatifyAspNetCoreService = chatifyAspNetCoreService;
    }

    [HttpGet("chat")]
    public async Task<IActionResult> GetChatsAsync()
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.GetChatsAsync());
    }

    [HttpGet("chat/{id:guid}")]
    public async Task<IActionResult> GetChatMembersAsync(Guid id)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.GetChatAsync(id));
    }

    [HttpPost("chat")]
    public async Task<IActionResult> AddChatAsync([FromBody] AddChatRequestDto request)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.AddChatAsync(request));
    }

    [HttpPost("chat/{chatId:guid}/member")]
    public async Task<IActionResult> AddChatMemberAsync(Guid chatId, [FromBody] AddMemberRequestDto request)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.AddChatMemberAsync(chatId, request));
    }

    [HttpPut("chat/{chatId:guid}/name")]
    public async Task<IActionResult> UpdateChatNameAsync(Guid chatId, [FromBody] UpdateChatNameRequest request)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.UpdateChatNameAsync(chatId, request));
    }

    [HttpDelete("chat/{chatId:guid}/member")]
    public async Task<IActionResult> DeleteChatMemberAsync(Guid chatId, [FromBody] RemoveChatMemberRequestDto request)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.DeleteChatMemberAsync(chatId, request));
    }

    [HttpPut("chat/message/read")]
    public async Task<IActionResult> ReadAsync([FromBody] ReadRequestDto request)
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.ReadAsync(request));
    }

    [HttpGet("account")]
    public async Task<IActionResult> GetAccountsAsync()
    {
        return new CodedActionResult(await _chatifyAspNetCoreService.GetAccountsAsync());
    }

    [HttpGet("chat/{chatId:guid}/message")]
    public async Task<IActionResult> GetMessagesAsync(
        Guid chatId,
        [FromQuery(Name = "pagingRule")] string? pagingRuleValue = null)
    {
        var pagingRule = pagingRuleValue != null ? PagingRule.FromJson(pagingRuleValue) : default;
        var result = await _chatifyAspNetCoreService.GetMessagesAsync(chatId, pagingRule);
        return new CodedActionResult(result);
    }

    [HttpPost("chat/{chatId:guid}/message")]
    public async Task<IActionResult> SendAsync(Guid chatId, [FromBody] SendMessageRequestDto request)
    {
        var result = await _chatifyAspNetCoreService.SendAsync(chatId, request);
        return new CodedActionResult(result);
    }
}
