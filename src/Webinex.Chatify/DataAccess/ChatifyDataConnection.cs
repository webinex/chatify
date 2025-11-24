using System.Text.Json;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using Webinex.Chatify.Rows;
using Webinex.Chatify.Rows.Chats;
using Webinex.Chatify.Rows.Threads;
using File = Webinex.Chatify.Abstractions.File;

namespace Webinex.Chatify.DataAccess;

internal class ChatifyDataConnection : DataConnection
{
    public ITable<AccountRow> AccountRows => this.GetTable<AccountRow>();
    public ITable<ChatMessageRow> MessageRows => this.GetTable<ChatMessageRow>();
    public ITable<ChatMemberRow> MemberRows => this.GetTable<ChatMemberRow>();
    public ITable<ChatMetaRow> ChatMetaRows => this.GetTable<ChatMetaRow>();
    public ITable<ChatRow> ChatRows => this.GetTable<ChatRow>();
    public ITable<ChatActivityRow> ChatActivityRows => this.GetTable<ChatActivityRow>();
    public ITable<ThreadRow> ThreadRows => this.GetTable<ThreadRow>();
    public ITable<ThreadMetaRow> ThreadMetaRows => this.GetTable<ThreadMetaRow>();
    public ITable<ThreadWatchRow> ThreadWatchRows => this.GetTable<ThreadWatchRow>();
    public ITable<ThreadMessageRow> ThreadMessageRows => this.GetTable<ThreadMessageRow>();

    public async Task<ChatMessageRow> SendMessageAsync(ChatMessageRow chatMessage, IEnumerable<string>? except = null, string? readForId = null)
    {
        await this.InsertAsync(chatMessage);
        await NotifyMembersAsync(chatMessage.Id, chatMessage.AuthorId, except, readForId);

        return chatMessage;
    }

    public async Task NotifyMembersAsync(string messageId, string authorId, IEnumerable<string>? except = null, string? readForId = null)
    {
        var id = ChatMessageId.Parse(messageId);
        
        var activityQueryable = ChatActivityRows
            .Where(x => x.ChatId == id.ChatId && x.Active);

        if (except != null)
            activityQueryable = activityQueryable.Where(x => !except.Contains(x.AccountId));

        var updatable = activityQueryable
            .Set(x => x.LastMessageId, id.ToString())
            .Set(x => x.LastMessageFromId, authorId);

        if (readForId != null)
            updatable = updatable
                .Set(x => x.LastReadMessageId,
                    x => x.AccountId == readForId ? messageId : x.LastReadMessageId);

        await updatable.UpdateAsync();
    }

    public async Task DeleteChatActivityAsync(Guid chatId, string accountId)
    {
        await ChatActivityRows.Where(x => x.ChatId == chatId && x.AccountId == accountId)
            .DeleteAsync();
    }

    public async Task DeleteMembershipAsync(Guid chatId, string accountId)
    {
        await MemberRows.Where(x => x.ChatId == chatId && x.AccountId == accountId)
            .DeleteAsync();
    }

    public async Task<ChatMetaRow> GetMetaWithUpdLockAsync(Guid chatId)
    {
        return await ChatMetaRows.AsSqlServer().WithUpdLock().FirstAsync(x => x.ChatId == chatId);
    }

    public async Task DeactivateChatActivityAsync(Guid chatId, string accountId)
    {
        var count = await ChatActivityRows.Where(x => x.ChatId == chatId && x.AccountId == accountId && x.Active)
            .Set(x => x.Active, false)
            .UpdateAsync();

        if (count == 0)
            throw new InvalidOperationException("Chat activity not found");
    }

    public async Task DeactivateMembershipAsync(Guid chatId, string accountId, string messageId)
    {
        var id = ChatMessageId.Parse(messageId);

        var count = await MemberRows.Where(x => x.ChatId == chatId && x.AccountId == accountId && x.LastMessageId == null)
            .Set(x => x.LastMessageId, id.ToString())
            .UpdateAsync();

        if (count == 0)
            throw new InvalidOperationException("Chat member not found");
    }

    public async Task RenameChatAsync(Guid id, string newName)
    {
        var count = await ChatRows.Where(x => x.Id == id)
            .Set(x => x.Name, newName)
            .UpdateAsync();

        if (count != 1)
            throw new InvalidOperationException();
    }
    
    public ChatifyDataConnection(DataOptions<ChatifyDataConnection> options) : base(options.Options)
    {
    }

    public static MappingSchema Schema()
    {
        var schema = new MappingSchema();
        var model = new FluentMappingBuilder(schema);

        model.Entity<AccountRow>()
            .HasSchemaName("chatify")
            .HasTableName("Accounts")
            .HasPrimaryKey(x => x.Id);

        model.Entity<ChatRow>()
            .HasSchemaName("chatify")
            .HasTableName("Chats")
            .HasPrimaryKey(x => x.Id);

        model.Entity<ChatMemberRow>()
            .HasSchemaName("chatify")
            .HasTableName("ChatMembers")
            .HasPrimaryKey(x => x.Id);

        model.Entity<ChatMessageRow>()
            .HasSchemaName("chatify")
            .HasTableName("ChatMessages")
            .HasPrimaryKey(x => x.Id)
            .Property(x => x.Files)
            .HasConversion(files => files.Count > 0 ? JsonSerializer.Serialize(files, JsonSerializerOptions.Default) : null,
                json => json == null ? Array.Empty<File>() : JsonSerializer.Deserialize<IReadOnlyCollection<File>>(json, JsonSerializerOptions.Default)!,
                handlesNulls: true)
            .Association(x => x.Author, x => x.AuthorId, x => x.Id, canBeNull: false);

        model.Entity<ChatMetaRow>()
            .HasSchemaName("chatify")
            .HasTableName("ChatMeta")
            .HasPrimaryKey(x => x.ChatId);

        model.Entity<ChatActivityRow>()
            .HasSchemaName("chatify")
            .HasTableName("ChatActivities")
            .HasPrimaryKey(x => new { x.ChatId, x.AccountId })
            .Association(x => x.LastChatMessage, x => x.LastMessageId, x => x.Id, canBeNull: true)
            .Association(x => x.Chat, x => x.ChatId, chat => chat!.Id, canBeNull: false);

        model.Entity<ThreadRow>()
            .HasSchemaName("chatify")
            .HasTableName("Threads")
            .HasPrimaryKey(x => new { x.Id });

        model.Entity<ThreadMessageRow>()
            .HasSchemaName("chatify")
            .HasTableName("ThreadMessages")
            .HasPrimaryKey(x => x.Id)
            .Property(x => x.Files)
            .HasConversion(files => files.Count > 0 ? JsonSerializer.Serialize(files, JsonSerializerOptions.Default) : null,
                json => json == null ? Array.Empty<File>() : JsonSerializer.Deserialize<IReadOnlyCollection<File>>(json, JsonSerializerOptions.Default)!,
                handlesNulls: true);
        
        model.Entity<ThreadWatchRow>()
            .HasSchemaName("chatify")
            .HasTableName("ThreadWatches")
            .HasPrimaryKey(x => new { x.ThreadId, x.AccountId });
        
        model.Entity<ThreadMetaRow>()
            .HasSchemaName("chatify")
            .HasTableName("ThreadMeta")
            .HasPrimaryKey(x => x.ThreadId);

        model.Build();
        return schema;
    }
}
