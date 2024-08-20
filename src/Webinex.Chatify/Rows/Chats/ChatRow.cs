namespace Webinex.Chatify.Rows.Chats;

internal class ChatRow
{
    public Guid Id { get; protected init; }
    public string Name { get; protected set; } = null!;
    public DateTimeOffset CreatedAt { get; protected init; }
    public string CreatedById { get; protected set; } = null!;

    protected ChatRow()
    {
    }

    public ChatRow(Guid id, string name, DateTimeOffset createdAt, string createdById)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        CreatedById = createdById;
    }

    public static ChatRow New(string name, string createdById)
    {
        return new ChatRow(Guid.NewGuid(), name, DateTimeOffset.UtcNow, createdById);
    }
}
