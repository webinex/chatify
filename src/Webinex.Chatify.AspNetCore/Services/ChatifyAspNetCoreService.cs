using Webinex.Asky;
using Webinex.Chatify.Abstractions;
using Webinex.Coded;

namespace Webinex.Chatify.AspNetCore;

public interface IChatifyAspNetCoreService
{
    Task<CodedResult<ChatListItemDto[]>> GetChatsAsync();
    Task<CodedResult<ChatDto>> GetChatAsync(Guid id);
    Task<CodedResult<Guid>> AddChatAsync(AddChatRequestDto request);
    Task<CodedResult> AddChatMemberAsync(Guid chatId, AddMemberRequestDto request);
    Task<CodedResult> DeleteChatMemberAsync(Guid chatId, RemoveChatMemberRequestDto request);
    Task<CodedResult> ReadAsync(ReadRequestDto request);
    Task<CodedResult<AccountDto[]>> GetAccountsAsync();
    Task<CodedResult<MessageDto[]>> GetMessagesAsync(Guid chatId, PagingRule? pagingRule);
    Task<CodedResult> SendAsync(Guid chatId, SendMessageRequestDto request);
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
            new MessageDto(chat.LastMessage.Value!),
            chat.TotalUnreadCount.Value)).ToArray();
        
        return CodedResults.Success(response);
    }

    public async Task<CodedResult<ChatDto>> GetChatAsync(Guid id)
    {
        var context = await _contextProvider.GetAsync();
        var chat = await _chatify.ChatAsync(id, onBehalfOf: context);
        var members = await _chatify.MembersAsync(id);
        var accounts = await _chatify.AccountByIdAsync(ids: members.Select(x => x.AccountId), tryCache: true);
        var accountDtos = accounts.Values.Select(x => new AccountDto(x)).ToArray();
        var response = new ChatDto(chat.Id, chat.Name, accountDtos);
        return CodedResults.Success(response);
    }

    public async Task<CodedResult<Guid>> AddChatAsync(AddChatRequestDto request)
    {
        var chat = await _chatify.AddChatAsync(
            new AddChatArgs(
                request.Name,
                request.Members,
                request.Message,
                await _contextProvider.GetAsync(),
                request.RequestId));

        var response = chat.Id;
        return CodedResults.Success(response);
    }

    public async Task<CodedResult> AddChatMemberAsync(Guid chatId, AddMemberRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        await _chatify.AddMembersAsync(new[]
            { new AddMemberArgs(chatId, request.AccountId, context) });
        return CodedResults.Success();
    }

    public async Task<CodedResult> DeleteChatMemberAsync(Guid chatId, RemoveChatMemberRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        await _chatify.RemoveMembersAsync(new[]
            { new RemoveMemberArgs(chatId, request.AccountId, context) });
        return CodedResults.Success();
    }

    public async Task<CodedResult> ReadAsync(ReadRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        var args = new ReadArgs(request.Ids, context);
        await _chatify.ReadAsync(args);
        return CodedResults.Success();
    }
    
    public async Task<CodedResult<AccountDto[]>> GetAccountsAsync()
    {
        var context = await _contextProvider.GetAsync();
        var result = await _chatify.AccountsAsync(onBehalfOf: context);
        var response = result.Select(x => new AccountDto(x)).ToArray();
        return CodedResults.Success(response);
    }
    
    public async Task<CodedResult<MessageDto[]>> GetMessagesAsync(
        Guid chatId,
        PagingRule? pagingRule)
    {
        pagingRule ??= PagingRule.TakeFirst(20);
        var filterRule = FilterRule.Eq("chatId", chatId);
        var sortRule = SortRule.Desc("sentAt");
        var context = await _contextProvider.GetAsync();

        var query = new MessageQuery(
            context,
            pagingRule: pagingRule,
            filterRule: filterRule,
            sortRule: [sortRule],
            props: Message.Props.Author | Message.Props.Read);

        var result = await _chatify.QueryAsync(query);
        var mapped = result.Select(x => new MessageDto(x)).ToArray();
        return CodedResults.Success(mapped);
    }
    
    public async Task<CodedResult> SendAsync(Guid chatId, SendMessageRequestDto request)
    {
        var context = await _contextProvider.GetAsync();
        var content = new MessageBody(request.Text, request.Files);
        await _chatify.SendMessagesAsync(new[] { new SendMessageArgs(chatId, content, context, request.RequestId) });
        return CodedResults.Success();
    }
}
