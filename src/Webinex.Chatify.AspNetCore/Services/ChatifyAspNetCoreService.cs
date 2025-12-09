using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Chatify.AspNetCore.Threads;
using Webinex.Coded;
using Thread = Webinex.Chatify.Abstractions.Thread;

namespace Webinex.Chatify.AspNetCore;

public interface IChatifyAspNetCoreService
{
    Task<CodedResult<ChatListItemDto[]>> GetChatsAsync();
    Task<CodedResult<ChatDto>> GetChatAsync(Guid id);
    Task<CodedResult<Guid>> AddChatAsync(AddChatRequestDto request);
    Task<CodedResult> AddChatMemberAsync(Guid chatId, AddChatMemberRequestDto request);
    Task<CodedResult> DeleteChatMemberAsync(Guid chatId, RemoveChatMemberRequestDto request);
    Task<CodedResult> UpdateChatNameAsync(Guid chatId, UpdateChatNameRequest request);
    Task<CodedResult> ReadAsync(ReadChatMessageRequestDto request);
    Task<CodedResult<AccountDto[]>> GetAccountsAsync(IEnumerable<string>? ids = null);
    Task<CodedResult<AccountDto>> GetCurrentUserAccountAsync();
    Task<CodedResult<ChatMessageDto[]>> GetMessagesAsync(Guid chatId, PagingRule? pagingRule);
    Task<CodedResult> SendAsync(Guid chatId, SendChatMessageRequestDto request);
    Task<CodedResult<IReadOnlyCollection<ThreadListItemDto>>> GetWatchThreadsAsync(bool? archive);
    Task<CodedResult<ThreadDto?>> GetThreadAsync(string id);

    Task<CodedResult<IReadOnlyCollection<ThreadMessageDto>>> GetThreadMessageListAsync(
        string threadId,
        int skip,
        int take);

    Task<CodedResult> WatchThreadAsync(string threadId, WatchThreadRequestDto request);
    Task<CodedResult<string>> SendThreadMessageAsync(string threadId, SendThreadMessageRequestDto request);
    Task<CodedResult> ReadThreadMessageAsync(string messageId);
    Task<CodedResult> UpdateAccountAsync(string accountId, UpdateAccountRequestDto request);
}

internal class ChatifyAspNetCoreService : IChatifyAspNetCoreService
{
    private readonly IChatifyAspNetCoreContextProvider _contextProvider;
    private readonly IChatify _chatify;

    public ChatifyAspNetCoreService(IChatifyAspNetCoreContextProvider contextProvider, IChatify chatify)
    {
        _contextProvider = contextProvider;
        _chatify = chatify;
    }

    public async Task<CodedResult<ChatListItemDto[]>> GetChatsAsync()
    {
        var context = await _contextProvider.GetAsync();
        var result = await _chatify.QueryAsync(new ChatQuery(onBehalfOf: context,
            props: Chat.Props.TotalUnreadCount | Chat.Props.LastMessageAuthor | Chat.Props.LastMessageRead));

        var response = result.Select(chat => new ChatListItemDto(chat,
            new ChatMessageDto(chat.LastMessage.Value!),
            chat.TotalUnreadCount.Value)).ToArray();

        return CodedResults.Success(response);
    }

    public async Task<CodedResult<ChatDto>> GetChatAsync(Guid id)
    {
        var context = await _contextProvider.GetAsync();
        var chat = (await _chatify.QueryAsync(new ChatQuery(context, FilterRule.Eq("id", id)))).Single();
        var members = await _chatify.ChatMembersAsync(id, active: true);
        var accounts = await _chatify.AccountByIdAsync(ids: members.Select(x => x.AccountId), tryCache: true);
        var accountDtos = accounts.Values.Select(x => new AccountDto(x)).ToArray();
        var response = new ChatDto(chat, accountDtos);
        return CodedResults.Success(response);
    }

    public async Task<CodedResult<Guid>> AddChatAsync(AddChatRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        var members = request.Members.Concat(new[] { context.Id }).Distinct().ToArray();

        var chat = await _chatify.AddChatAsync(
            new AddChatArgs(
                request.Name,
                members,
                request.Message,
                await _contextProvider.GetAsync()));

        var response = chat.Id;
        return CodedResults.Success(response);
    }

    public async Task<CodedResult> AddChatMemberAsync(Guid chatId, AddChatMemberRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        await _chatify.AddChatMembersAsync(new[]
            { new AddChatMemberArgs(chatId, request.AccountId, context, request.WithHistory) });
        return CodedResults.Success();
    }

    public async Task<CodedResult> DeleteChatMemberAsync(Guid chatId, RemoveChatMemberRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        await _chatify.RemoveChatMembersAsync(new[]
            { new RemoveChatMemberArgs(chatId, request.AccountId, context, request.DeleteHistory) });
        return CodedResults.Success();
    }

    public async Task<CodedResult> UpdateChatNameAsync(Guid chatId, UpdateChatNameRequest request)
    {
        var context = await _contextProvider.GetAsync();
        await _chatify.UpdateChatNameAsync(new UpdateChatNameArgs(chatId, request.Name, context));
        return CodedResults.Success();
    }

    public async Task<CodedResult> ReadAsync(ReadChatMessageRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        var args = new ReadChatMessageArgs(request.Id, context);
        await _chatify.ReadAsync(args);
        return CodedResults.Success();
    }

    public async Task<CodedResult<AccountDto[]>> GetAccountsAsync(IEnumerable<string>? ids = null)
    {
        var context = await _contextProvider.GetAsync();
        var result = await _chatify.AccountsAsync(onBehalfOf: context, ids: ids);
        var response = result.Select(x => new AccountDto(x)).ToArray();
        return CodedResults.Success(response);
    }

    public async Task<CodedResult<AccountDto>> GetCurrentUserAccountAsync()
    {
        var context = await _contextProvider.GetAsync();
        var account = await _chatify.AccountByIdAsync(context.Id);
        return CodedResults.Success(new AccountDto(account));
    }

    public async Task<CodedResult<ChatMessageDto[]>> GetMessagesAsync(
        Guid chatId,
        PagingRule? pagingRule)
    {
        pagingRule ??= PagingRule.TakeFirst(20);
        var filterRule = FilterRule.Eq("chatId", chatId);
        var sortRule = SortRule.Desc("sentAt");
        var context = await _contextProvider.GetAsync();

        var query = new ChatMessageQuery(
            context,
            pagingRule: pagingRule,
            filterRule: filterRule,
            sortRule: [sortRule],
            props: ChatMessage.Props.Author);

        var result = await _chatify.QueryAsync(query);
        var mapped = result.Select(x => new ChatMessageDto(x)).ToArray();
        return CodedResults.Success(mapped);
    }

    public async Task<CodedResult> SendAsync(Guid chatId, SendChatMessageRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        var content = new MessageBody(request.Text, request.Files);
        await _chatify.SendMessagesAsync(new[]
            { new SendChatMessageArgs(chatId, content, context) });
        return CodedResults.Success();
    }

    public async Task<CodedResult<IReadOnlyCollection<ThreadListItemDto>>> GetWatchThreadsAsync(bool? archive)
    {
        var filterRule = archive.HasValue ? FilterRule.Eq("archive", archive.Value) : null;

        var threads = await _chatify.QueryAsync(new ThreadWatchQuery(
            onBehalfOf: await _contextProvider.GetAsync(),
            sortRule: [SortRule.Desc("name")],
            filterRule: filterRule,
            props: Thread.Props.LastMessage | Thread.Props.Watch | Thread.Props.LastReadMessageId));

        var accountById = await _chatify.AccountByIdAsync(ids: threads.Select(x => x.LastMessage.Value?.SentById)
                .Where(x => x != null).Cast<string>(),
            tryCache: true,
            required: true);

        var result = threads.Select(x =>
        {
            var lastMessage = x.LastMessage.Value != null
                ? new ThreadMessageDto(x.LastMessage.Value, new AccountDto(accountById[x.LastMessage.Value.SentById]))
                : null;
            return new ThreadListItemDto(x, lastMessage);
        }).ToArray();
        return CodedResults.Success<IReadOnlyCollection<ThreadListItemDto>>(result);
    }

    public async Task<CodedResult<ThreadDto?>> GetThreadAsync(string id)
    {
        var thread = await _chatify.ThreadByIdAsync(
            await _contextProvider.GetAsync(),
            id,
            props: Thread.Props.Watch | Thread.Props.LastReadMessageId);

        if (thread == null)
            return CodedResults.Success<ThreadDto?>(null);

        var accountIds = new[] { thread.CreatedById };
        var accountById = await _chatify.AccountByIdAsync(accountIds);
        var result = new ThreadDto(thread, new AccountDto(accountById[thread.CreatedById]));
        return CodedResults.Success<ThreadDto?>(result);
    }

    public async Task<CodedResult<IReadOnlyCollection<ThreadMessageDto>>> GetThreadMessageListAsync(
        string threadId,
        int skip,
        int take)
    {
        var messages = await _chatify.QueryAsync(new ThreadMessageQuery(
            onBehalfOf: await _contextProvider.GetAsync(),
            threadId,
            sentAtAsc: false,
            pagingRule: new PagingRule(skip, take)));

        var accountById = await _chatify.AccountByIdAsync(
            ids: messages.Select(x => x.SentById));

        var result = messages.Select(x => new ThreadMessageDto(x, new AccountDto(accountById[x.SentById]))).ToArray();
        return CodedResults.Success<IReadOnlyCollection<ThreadMessageDto>>(result);
    }

    public async Task<CodedResult> WatchThreadAsync(string threadId, WatchThreadRequestDto request)
    {
        var onBehalfOf = await _contextProvider.GetAsync();
        await _chatify.SetThreadWatchAsync(
            onBehalfOf,
            threadId,
            onBehalfOf.Id,
            request.Watch);
        return CodedResults.Success();
    }

    public async Task<CodedResult<string>> SendThreadMessageAsync(string threadId, SendThreadMessageRequestDto request)
    {
        var message = await _chatify.SendThreadMessageAsync(new SendThreadMessageArgs(threadId, request.Body,
            await _contextProvider.GetAsync()));
        return CodedResults.Success(message.Id);
    }

    public async Task<CodedResult> ReadThreadMessageAsync(string messageId)
    {
        await _chatify.ReadThreadMessageAsync(new ReadThreadMessageArgs(
            messageId,
            await _contextProvider.GetAsync()));
        return CodedResults.Success();
    }

    public async Task<CodedResult> UpdateAccountAsync(string accountId, UpdateAccountRequestDto request)
    {
        await _chatify.UpdateAccountsAsync([new UpdateAccountDataArgs(accountId, request.Name, request.Avatar, request.Active, request.AutoReply)]);
        return CodedResults.Success();
    }
}
