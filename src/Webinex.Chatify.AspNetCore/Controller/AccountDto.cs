using Webinex.Chatify.Types;

namespace Webinex.Chatify.AspNetCore.Controller;

public class AccountDto
{
    public string Id { get;  } 
    public string? Avatar { get;  }
    public string Name { get;  } 
    public AccountType Type { get;  }
    public bool Active { get;  }

    public AccountDto(Account account)
    {
        Id = account.Id;
        Avatar = account.Avatar;
        Name = account.Name;
        Type = account.Type;
        Active = account.Active;
    }
}