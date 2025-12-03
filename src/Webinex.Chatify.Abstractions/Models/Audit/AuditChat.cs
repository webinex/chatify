namespace Webinex.Chatify.Abstractions.Audit;

public class AuditChat
{
    public AuditChat(
        Guid id,
        string workspaceId,
        string name,
        DateTimeOffset createdAt,
        string createdById,
        AuditChatMessage? lastMessage = null)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        CreatedById = createdById;
        WorkspaceId = workspaceId;
        LastMessage = lastMessage != null ? Optional.Value(lastMessage) : Optional.NoValue<AuditChatMessage>();
    }

    public Guid Id { get; }
    public string WorkspaceId { get; }
    public string Name { get; }
    public DateTimeOffset CreatedAt { get; }
    public string CreatedById { get; }
    public Optional<AuditChatMessage> LastMessage { get; }
}
