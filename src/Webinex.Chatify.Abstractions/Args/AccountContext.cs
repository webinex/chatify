namespace Webinex.Chatify.Abstractions;

public class AccountContext : Equatable
{
    public string Id { get; }
    public string WorkspaceId { get; }

    public static AccountContext System => new("chatify::system", "chatify::system");
    
    public AccountContext(string id, string workspaceId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return WorkspaceId;
    }

    public bool IsSystem() => Id == "chatify::system";
}