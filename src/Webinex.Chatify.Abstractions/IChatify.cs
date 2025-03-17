using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public interface IChatify
{
    Task<Chat> AddChatAsync(AddChatArgs args);
    Task UpdateChatNameAsync(UpdateChatNameArgs args);
    Task<ChatMessage[]> SendMessagesAsync(IEnumerable<SendChatMessageArgs> commands);
    Task AddChatMembersAsync(IEnumerable<AddChatMemberArgs> commands);
    Task RemoveChatMembersAsync(IEnumerable<RemoveChatMemberArgs> commands);
    Task<Chat[]> QueryAsync(ChatQuery query);

    Task<IReadOnlyCollection<Chat>> GetAllChatsAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null);

    Task<ChatMessage[]> QueryAsync(ChatMessageQuery query);
    Task<IReadOnlyCollection<Chat>> ChatByIdAsync(IEnumerable<Guid> chatIds, AccountContext? onBehalfOf = null, bool required = true);
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<ChatMember>>> ChatMembersAsync(IEnumerable<Guid> chatIds, bool? active = null);
    Task<IReadOnlyDictionary<Guid, string[]>> ActiveChatMemberIdByChatIdAsync(IEnumerable<Guid> chatIds);

    Task<IReadOnlyDictionary<string, Account>> AccountByIdAsync(IEnumerable<string> ids, bool tryCache = false,
        bool required = true);

    Task<IReadOnlyCollection<Account>> AccountsAsync(AccountContext? onBehalfOf = null);
    Task ReadAsync(ReadChatMessageArgs chatMessageArgs);
    Task<Account[]> AddAccountsAsync(IEnumerable<AddAccountArgs> commands);
    Task<Account[]> UpdateAccountsAsync(IEnumerable<UpdateAccountArgs> commands);
    
    
    Task<Thread> AddThreadAsync(AddThreadArgs args);
    Task UpdateThreadAsync(UpdateThreadArgs args);
    Task RemoveThreadAsync(string threadId);
    Task ArchiveThreadAsync(string threadId);

    Task AddThreadWatcherAsync(string threadId, string accountId);
    Task RemoveThreadWatcherAsync(string threadId, string accountId);
    Task SetThreadWatchAsync(AccountContext onBehalfOf, string threadId, string accountId, bool watch);

    Task<ThreadMessage> SendThreadMessageAsync(SendThreadMessageArgs args);
    Task<int> ReadThreadMessageAsync(ReadThreadMessageArgs args);

    Task<IReadOnlyDictionary<string, bool>> ThreadExistsAsync(IEnumerable<string> ids);
    Task<Thread?> ThreadByIdAsync(AccountContext onBehalfOf, string id, Thread.Props props = Thread.Props.Default);
    Task<IReadOnlyDictionary<string, Thread?>> ThreadByIdAsync(IEnumerable<string> ids);
    Task<IReadOnlyCollection<Thread>> QueryAsync(ThreadWatchQuery query);
    Task<IReadOnlyCollection<ThreadMessage>> QueryAsync(ThreadMessageQuery query);
    Task<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> WatchersByThreadIdAsync(IEnumerable<string> threadIds);
}

public static class ChatifyExtensions
{
    public static async Task<IReadOnlyCollection<ChatMember>> ChatMembersAsync(this IChatify chatify, Guid chatId, bool? active = null)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        var result = await chatify.ChatMembersAsync(new[] { chatId }, active);
        return result.Values.First();
    }

    public static async Task<Chat> ChatAsync(this IChatify chatify, Guid chatId, AccountContext? onBehalfOf = null)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        var result = await chatify.ChatByIdAsync(new[] { chatId }, onBehalfOf: onBehalfOf);
        return result.First();
    }

    public static async Task<Account?> AccountByIdAsync(this IChatify chatify, string id, bool tryCache, bool required)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        var result = await chatify.AccountByIdAsync(new[] { id }, tryCache, required);
        return required ? result.Values.First() : result.Values.FirstOrDefault();
    }

    public static async Task<Account> AccountByIdAsync(this IChatify chatify, string id, bool tryCache = false)
    {
        var result = await AccountByIdAsync(chatify, id, tryCache, required: true);
        return result!;
    }

    public static async Task<bool> ThreadExistsAsync(this IChatify chatify, string id)
    {
        var result = await chatify.ThreadExistsAsync(new[] { id });
        return result.Single().Value;
    }
}