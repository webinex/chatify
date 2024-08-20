using Microsoft.AspNetCore.Mvc;
using Webinex.Chatify.AspNetCore.Threads;
using Webinex.Coded.AspNetCore;

namespace Webinex.Chatify.AspNetCore;

[Route("/api/chatify")]
internal class ChatifyThreadController : ControllerBase
{
    private readonly IChatifyAspNetCoreService _chatify;

    public ChatifyThreadController(IChatifyAspNetCoreService chatify)
    {
        _chatify = chatify;
    }

    [HttpGet("thread/watch")]
    public async Task<IActionResult> GetWatchThreadsAsync([FromQuery] bool? archive)
    {
        return new CodedActionResult(await _chatify.GetWatchThreadsAsync(archive));
    }

    [HttpGet("thread/{id}")]
    public async Task<IActionResult> GetThreadAsync(string id)
    {
        return new CodedActionResult(await _chatify.GetThreadAsync(id));
    }

    [HttpGet("thread/{threadId}/message")]
    public async Task<IActionResult> GetThreadMessageListAsync(
        string threadId,
        [FromQuery] int skip,
        [FromQuery] int take)
    {
        return new CodedActionResult(await _chatify.GetThreadMessageListAsync(threadId, skip, take));
    }

    [HttpPut("thread/{threadId}/watch")]
    public async Task<IActionResult> WatchThreadAsync(string threadId, [FromBody] WatchThreadRequestDto request)
    {
        return new CodedActionResult(await _chatify.WatchThreadAsync(threadId, request));
    }

    [HttpPost("thread/{threadId}/message")]
    public async Task<IActionResult> SendThreadMessageAsync(string threadId, [FromBody] SendThreadMessageRequestDto request)
    {
        return new CodedActionResult(await _chatify.SendThreadMessageAsync(threadId, request));
    }

    [HttpPut("thread/message/{messageId}/read")]
    public async Task<IActionResult> ReadThreadMessageAsync(string messageId)
    {
        return new CodedActionResult(await _chatify.ReadThreadMessageAsync(messageId));
    }
}
