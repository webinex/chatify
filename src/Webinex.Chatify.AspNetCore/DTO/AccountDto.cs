﻿using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore;

public class AccountDto
{
    public string Id { get; }
    public string? Avatar { get; }
    public string Name { get; }
    public AccountType Type { get; }
    public bool Active { get; }

    public AccountDto(string id, string? avatar, string name, AccountType type, bool active)
    {
        Id = id;
        Avatar = avatar;
        Name = name;
        Type = type;
        Active = active;
    }

    public AccountDto(Account account)
    {
        Id = account.Id;
        Avatar = account.Avatar;
        Name = account.Name;
        Type = account.Type;
        Active = account.Active;
    }

    public static AccountDto System() =>
        new AccountDto(AccountContext.System.Id, avatar: null, "System", AccountType.System, active: true);
}
