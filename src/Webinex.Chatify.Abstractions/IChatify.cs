using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public interface IChatify
{
    Task<IReadOnlyCollection<Chat>> AddChatsAsync(IEnumerable<AddChatArgs> commands);
    Task<Message[]> SendMessagesAsync(IEnumerable<SendMessageArgs> commands);
    Task AddMembersAsync(IEnumerable<AddMemberArgs> commands);
    Task RemoveMembersAsync(IEnumerable<RemoveMemberArgs> commands);
    Task<Chat[]> QueryAsync(ChatQuery query);

    Task<IReadOnlyCollection<Chat>> GetAllChatsAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null);

    Task<Message[]> QueryAsync(MessageQuery query);
    Task<IReadOnlyCollection<Chat>> ChatByIdAsync(IEnumerable<Guid> chatIds, AccountContext? onBehalfOf = null, bool required = true);
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> MembersAsync(IEnumerable<Guid> chatIds);

    Task<IReadOnlyDictionary<string, Account>> AccountByIdAsync(IEnumerable<string> ids, bool tryCache = false,
        bool required = true);

    Task<IReadOnlyCollection<Account>> AccountsAsync(AccountContext? onBehalfOf = null);
    Task ReadAsync(ReadArgs args);
    Task<Account[]> AddAccountsAsync(IEnumerable<AddAccountArgs> commands);
    Task<Account[]> UpdateAccountsAsync(IEnumerable<UpdateAccountArgs> commands);
}

public static class ChatifyExtensions
{
    public static async Task<Chat> AddChatAsync(this IChatify chatify, AddChatArgs args)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        args = args ?? throw new ArgumentNullException(nameof(args));

        var result = await chatify.AddChatsAsync(new[] { args });
        return result.First();
    }

    public static async Task<IReadOnlyCollection<Member>> MembersAsync(this IChatify chatify, Guid chatId)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        var result = await chatify.MembersAsync(new[] { chatId });
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
}