using System.Security.Claims;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore;

namespace Webinex.Chatify.Example;

public class ChatifyAspNetCoreContext : IChatifyAspNetCoreContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatifyAspNetCoreContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<AccountContext> GetAsync()
    {
        var id = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        return Task.FromResult(new AccountContext(id));
    }
}