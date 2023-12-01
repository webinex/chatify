using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public interface IChatifyAspNetCoreContext
{
    Task<AccountContext> GetAsync();
}