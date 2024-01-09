using System.Security.Claims;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore;

namespace Webinex.Chatify.Example;

public class ChatifyAspNetCoreContextProvider : IChatifyAspNetCoreContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IChatify _chatify;

    public ChatifyAspNetCoreContextProvider(IHttpContextAccessor httpContextAccessor, IChatify chatify)
    {
        _httpContextAccessor = httpContextAccessor;
        _chatify = chatify;
    }

    public async Task<AccountContext> GetAsync()
    {
        var id = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var account = await _chatify.AccountById(id, tryCache: true);
        return new AccountContext(id, account.WorkspaceId);
    }
}