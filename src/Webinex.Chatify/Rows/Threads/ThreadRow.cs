namespace Webinex.Chatify.Rows.Threads;

internal class ThreadRow
{
    public string Id { get; protected init; } = null!;
    public string Name { get; protected init; } = null!;
    public string CreatedById { get; protected init; } = null!;
    public DateTimeOffset CreatedAt { get; protected init; }
    public bool Archived { get; protected init; }
    public string? LastMessageId { get; protected set; }

    protected ThreadRow(
        string id,
        string name,
        string createdById,
        DateTimeOffset createdAt,
        bool archived,
        string? lastMessageId)
    {
        Id = id;
        Name = name;
        CreatedById = createdById;
        CreatedAt = createdAt;
        Archived = archived;
        LastMessageId = lastMessageId;
    }

    protected ThreadRow()
    {
    }

    public static ThreadRow New(string id, string name, string createdById)
    {
        id = id ?? throw new ArgumentNullException(nameof(id));
        name = name ?? throw new ArgumentNullException(nameof(name));
        createdById = createdById ?? throw new ArgumentNullException(nameof(createdById));

        return new ThreadRow(id, name, createdById, DateTimeOffset.UtcNow, archived: false, lastMessageId: null);
    }
}
