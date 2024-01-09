using Microsoft.AspNetCore.Mvc;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore;

namespace Webinex.Chatify.Example;

[Route("/api")]
public class AccountController : ControllerBase
{
    private readonly IChatify _chatify;

    public AccountController(IChatify chatify)
    {
        _chatify = chatify;
    }

    [HttpGet("avatar/{id}")]
    public IActionResult Get(string id)
    {
        var assembly = typeof(AccountController).Assembly;
        var directory = Path.GetDirectoryName(assembly.Location)!;
        var path = Path.Combine(directory, "Resources", "avatars", $"avatar-{id}.jpeg");
        return File(System.IO.File.OpenRead(path), "image/jpeg");
    }

    [HttpGet("account")]
    public async Task<AccountDto[]> AccountsAsync()
    {
        var accounts = await _chatify.AccountsAsync();
        return accounts.Select(x => new AccountDto(x)).ToArray();
    }

    [HttpGet("account/{id}")]
    public async Task<AccountDto> AccountAsync(string id)
    {
        var account = await _chatify.AccountById(id, tryCache: true);
        return new AccountDto(account);
    }
}