using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Services;
using Webinex.Chatify.Services.Chats;
using Webinex.Chatify.Services.Chats.Members;
using Webinex.Chatify.Services.Chats.Messages;
using Webinex.Chatify.Services.Threads;
using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify;

internal class Chatify : IChatify
{
    private readonly IAccountService _accountService;
    private readonly IChatMessageService _chatMessageService;
    private readonly IChatService _chatService;
    private readonly IChatMemberService _chatMemberService;
    private readonly IAuthorizationPolicy _authorizationPolicy;
    private readonly IThreadService _threadService;

    public Chatify(
        IAccountService accountService,
        IChatMessageService chatMessageService,
        IChatService chatService,
        IChatMemberService chatMemberService,
        IAuthorizationPolicy authorizationPolicy,
        IThreadService threadService)
    {
        _accountService = accountService;
        _chatMessageService = chatMessageService;
        _chatService = chatService;
        _chatMemberService = chatMemberService;
        _authorizationPolicy = authorizationPolicy;
        _threadService = threadService;
    }

    public async Task<Chat> AddChatAsync(AddChatArgs args)
    {
        return await _chatService.AddAsync(args);
    }

    public async Task UpdateChatNameAsync(UpdateChatNameArgs args)
    {
        await _authorizationPolicy.AuthorizeUpdateChatNameAsync([args]);
        await _chatService.UpdateNameAsync(args);
    }

    public async Task<ChatMessage[]> SendMessagesAsync(IEnumerable<SendChatMessageArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return Array.Empty<ChatMessage>();

        await _authorizationPolicy.AuthorizeSendAsync(commands.ToArray());
        return await _chatMessageService.SendRangeAsync(commands);
    }

    public async Task AddChatMembersAsync(IEnumerable<AddChatMemberArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return;

        await _authorizationPolicy.AuthorizeAddChatMemberAsync(commands.ToArray());
        await _chatMemberService.AddRangeAsync(commands);
    }

    public async Task RemoveChatMembersAsync(IEnumerable<RemoveChatMemberArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return;

        await _authorizationPolicy.AuthorizeRemoveChatMemberAsync(commands.ToArray());
        await _chatMemberService.RemoveRangeAsync(commands);
    }

    public async Task<Chat[]> QueryAsync(ChatQuery query)
    {
        return await _chatService.QueryAsync(query);
    }

    public Task<IReadOnlyCollection<Chat>> GetAllChatsAsync(
        FilterRule? filterRule = null,
        SortRule? sortRule = null,
        PagingRule? pagingRule = null)
    {
        return _chatService.GetAllAsync(filterRule, sortRule, pagingRule);
    }

    public async Task<ChatMessage[]> QueryAsync(ChatMessageQuery query)
    {
        return await _chatMessageService.QueryAsync(query);
    }

    public async Task<IReadOnlyCollection<Chat>> ChatByIdAsync(
        IEnumerable<Guid> chatIds,
        AccountContext? onBehalfOf = null,
        bool required = true)
    {
        chatIds = chatIds.Distinct().ToArray();

        if (onBehalfOf != null)
            await _authorizationPolicy.AuthorizeGetChatAsync(onBehalfOf, chatIds);

        return await _chatService.ByIdAsync(chatIds, required);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<ChatMember>>> ChatMembersAsync(IEnumerable<Guid> chatIds, bool? active = null)
    {
        return await _chatMemberService.ByChatsAsync(chatIds, active);
    }

    public Task<IReadOnlyDictionary<Guid, string[]>> ActiveChatMemberIdByChatIdAsync(IEnumerable<Guid> chatIds)
    {
        return _chatMemberService.ActiveIdByChatIdAsync(chatIds);
    }

    public Task<IReadOnlyDictionary<string, Account>> AccountByIdAsync(
        IEnumerable<string> ids,
        bool tryCache = false,
        bool required = true)
    {
        return _accountService.ByIdAsync(ids, tryCache, required);
    }

    public Task<IReadOnlyCollection<Account>> AccountsAsync(AccountContext? onBehalfOf = null)
    {
        return _accountService.GetAllAsync(onBehalfOf);
    }

    public async Task ReadAsync(ReadChatMessageArgs chatMessageArgs)
    {
        if (!chatMessageArgs.Id.Any())
            return;

        await _chatMessageService.ReadAsync(chatMessageArgs);
    }

    public async Task<Account[]> AddAccountsAsync(IEnumerable<AddAccountArgs> commands)
    {
        return await _accountService.AddAsync(commands);
    }

    public async Task<Account[]> UpdateAccountsAsync(IEnumerable<UpdateAccountArgs> commands)
    {
        return await _accountService.UpdateAsync(commands);
    }

    public Task<Thread> AddThreadAsync(AddThreadArgs args)
    {
        return _threadService.AddThreadAsync(args);
    }

    public Task RemoveThreadAsync(string threadId)
    {
        return _threadService.RemoveThreadAsync(threadId);
    }

    public Task ArchiveThreadAsync(string threadId)
    {
        return _threadService.ArchiveThreadAsync(threadId);
    }

    public Task AddThreadWatcherAsync(string threadId, string accountId)
    {
        return _threadService.AddThreadWatcherAsync(threadId, accountId);
    }

    public Task RemoveThreadWatcherAsync(string threadId, string accountId)
    {
        return _threadService.RemoveThreadWatcherAsync(threadId, accountId);
    }

    public Task SetThreadWatchAsync(AccountContext onBehalfOf, string threadId, string accountId, bool watch)
    {
        return _threadService.SetThreadWatchAsync(onBehalfOf, threadId, accountId, watch);
    }

    public Task<ThreadMessage> SendThreadMessageAsync(SendThreadMessageArgs args)
    {
        return _threadService.SendThreadMessageAsync(args);
    }

    public Task<int> ReadThreadMessageAsync(ReadThreadMessageArgs args)
    {
        return _threadService.ReadThreadMessageAsync(args);
    }

    public Task<IReadOnlyDictionary<string, bool>> ThreadExistsAsync(IEnumerable<string> ids)
    {
        return _threadService.ThreadExistsAsync(ids);
    }

    public Task<Thread?> ThreadByIdAsync(AccountContext onBehalfOf, string id, Thread.Props props = Thread.Props.Default)
    {
        return _threadService.ThreadByIdAsync(onBehalfOf, id, props);
    }

    public Task<IReadOnlyDictionary<string, Thread?>> ThreadByIdAsync(IEnumerable<string> ids)
    {
        return _threadService.ThreadByIdAsync(ids);
    }

    public Task<IReadOnlyCollection<Thread>> QueryAsync(ThreadWatchQuery query)
    {
        return _threadService.QueryAsync(query);
    }

    public Task<IReadOnlyCollection<ThreadMessage>> QueryAsync(ThreadMessageQuery query)
    {
        return _threadService.QueryAsync(query);
    }

    public Task<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> WatchersByThreadIdAsync(IEnumerable<string> threadIds)
    {
        return _threadService.WatchersByThreadIdAsync(threadIds);
    }
}
