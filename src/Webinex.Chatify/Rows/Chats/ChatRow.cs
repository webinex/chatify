namespace Webinex.Chatify.Rows.Chats;

internal class ChatRow
{
    public Guid Id { get; protected init; }
    public string WorkspaceId { get; protected init; } = null!;
    public string Name { get; protected set; } = null!;
    public DateTimeOffset CreatedAt { get; protected init; }
    public string CreatedById { get; protected set; } = null!;

    protected ChatRow()
    {
    }

    public ChatRow(Guid id, string workspaceId, string name, DateTimeOffset createdAt, string createdById)
    {
        Id = id;
        WorkspaceId = workspaceId;
        Name = name;
        CreatedAt = createdAt;
        CreatedById = createdById;
    }

    public static ChatRow New(string workspaceId, string name, string createdById)
    {
        return new ChatRow(Guid.NewGuid(), workspaceId, name, DateTimeOffset.UtcNow, createdById);
    }
}
