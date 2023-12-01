using Webinex.Chatify.Types;

namespace Webinex.Chatify.Abstractions;

public class AccountContext
{
    public string Id { get; }

    public AccountContext(string id)
    {
        Id = id;
    }

    public bool IsSystem()
    {
        return AccountId.SYSTEM == Id;
    }
}