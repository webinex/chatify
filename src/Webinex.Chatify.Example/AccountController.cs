using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webinex.Chatify.AspNetCore.Controller;
using Webinex.Chatify.DataAccess;
using Webinex.Chatify.Types;

namespace Webinex.Chatify.Example;

[Route("/api")]
public class AccountController : ControllerBase
{
    private readonly ChatifyDbContext _chatifyDbContext;

    public AccountController(ChatifyDbContext chatifyDbContext)
    {
        _chatifyDbContext = chatifyDbContext;
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
        var accounts = await _chatifyDbContext.Accounts.Where(x => x.Id != AccountId.SYSTEM).ToArrayAsync();
        return accounts.Select(x => new AccountDto(x)).ToArray();
    }

    [HttpGet("account/{id}")]
    public async Task<AccountDto> AccountAsync(string id)
    {
        var account = await _chatifyDbContext.Accounts.FindAsync(id);
        return new AccountDto(account!);
    }
}