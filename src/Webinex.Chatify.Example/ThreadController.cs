using Microsoft.AspNetCore.Mvc;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore;

namespace Webinex.Chatify.Example;

[Route("/api/thread")]
public class ThreadController : ControllerBase
{
    private readonly IChatify _chatify;
    private readonly IChatifyAspNetCoreContextProvider _chatifyContextProvider;

    public ThreadController(IChatify chatify, IChatifyAspNetCoreContextProvider chatifyContextProvider)
    {
        _chatify = chatify;
        _chatifyContextProvider = chatifyContextProvider;
    }

    [HttpPost]
    public async Task<string> CreateThreadAsync([FromBody] CreateThreadRequestDto request)
    {
        var context = await _chatifyContextProvider.GetAsync();
        var thread = await _chatify.AddThreadAsync(new AddThreadArgs(Guid.NewGuid().ToString(), request.Name,
            AccountContext.System,
            new[] { context.Id }));

        return thread.Id;
    }

    [HttpPut]
    public async Task UpdateThreadAsync([FromBody] UpdateThreadRequestDto request)
    {
        await _chatify.UpdateThreadAsync(new UpdateThreadArgs(request.Id, request.Name));
    }

    public record CreateThreadRequestDto(string Name);

    public record UpdateThreadRequestDto(string Id, string Name);
}