using Microsoft.AspNetCore.Mvc;
using Webinex.Asky;
using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore.Controller;

[Route("/api/chatify")]
public class ChatifyController : ControllerBase
{
    private readonly IChatify _chatify;
    private readonly IChatifyAspNetCoreContext _chatifyAspNetCoreContext;

    public ChatifyController(IChatify chatify, IChatifyAspNetCoreContext chatifyAspNetCoreContext)
    {
        _chatify = chatify;
        _chatifyAspNetCoreContext = chatifyAspNetCoreContext;
    }

    [HttpGet("chat")]
    public async Task<ChatListItemDto[]> GetChatsAsync()
    {
        var context = await _chatifyAspNetCoreContext.GetAsync();
        var result = await _chatify.QueryAsync(new ChatQuery(onBehalfOf: context,
            prop: ChatQueryProp.LastMessage | ChatQueryProp.UnreadCount));
        return result.Chats.Select(x =>
        {
            var lastMessage = result.LastMessages![x];
            return new ChatListItemDto(x,
                new MessageDto(lastMessage.Message, lastMessage.Delivery, lastMessage.Author),
                result.UnreadCount![x]);
        }).ToArray();
    }

    [HttpGet("chat/{id:guid}")]
    public async Task<ChatDto> GetChatMembersAsync(Guid id)
    {
        var context = await _chatifyAspNetCoreContext.GetAsync();
        var chat = await _chatify.ChatAsync(id);
        var members = await _chatify.MembersAsync(id);
        var accounts = await _chatify.AccountByIdAsync(ids: members.Select(x => x.AccountId));
        var accountDtos = accounts.Select(x => new AccountDto(x)).ToArray();
        return new ChatDto(chat.Id, chat.Name, accountDtos);
    }

    [HttpPost("chat")]
    public async Task<Guid> AddChatAsync([FromBody] AddChatRequestDto request)
    {
        var chat = await _chatify.AddChatAsync(
            new AddChatCommand(
                request.Name,
                request.Members,
                request.Message,
                await _chatifyAspNetCoreContext.GetAsync(),
                request.RequestId));

        return chat.Id;
    }

    [HttpPost("chat/{chatId:guid}/member")]
    public async Task AddChatMemberAsync(Guid chatId, [FromBody] AddMemberRequestDto request)
    {
        var context = await _chatifyAspNetCoreContext.GetAsync();
        await _chatify.AddMembersAsync(new[] { new AddMemberCommand(chatId, request.AccountId, context, request.WithHistory) });
    }

    [HttpDelete("chat/{chatId:guid}/member")]
    public async Task DeleteChatMemberAsync(Guid chatId, [FromBody] RemoveChatMemberRequestDto request)
    {
        var context = await _chatifyAspNetCoreContext.GetAsync();
        await _chatify.RemoveMembersAsync(new[] { new RemoveMemberCommand(chatId, request.AccountId, context, request.DeleteHistory) });
    }

    [HttpPut("chat/message/read")]
    public async Task ReadAsync([FromBody] ReadRequestDto request)
    {
        var context = await _chatifyAspNetCoreContext.GetAsync();
        var args = new ReadCommand(request.Ids, context);
        await _chatify.ReadAsync(args);
    }

    [HttpGet("account")]
    public async Task<AccountDto[]> GetAccountsAsync()
    {
        var result = await _chatify.AccountByIdAsync();
        return result.Select(x => new AccountDto(x)).ToArray();
    }

    [HttpGet("chat/{chatId:guid}/message")]
    public async Task<MessageDto[]> GetMessagesAsync(
        Guid chatId,
        [FromQuery(Name = "pagingRule")] string? pagingRuleValue = null)
    {
        var pagingRule = pagingRuleValue != null ? PagingRule.FromJson(pagingRuleValue) : PagingRule.TakeFirst(20);
        var filterRule = FilterRule.Eq("chatId", chatId);
        var sortRule = SortRule.Desc("sentAt");
        var context = await _chatifyAspNetCoreContext.GetAsync();
        var result = await _chatify.QueryAsync(new MessageQuery(context, pagingRule: pagingRule, filterRule: filterRule,
            sortRule: new[] { sortRule }));
        return result.Entries.Select(x => new MessageDto(x.Message, x.Delivery, x.Author)).ToArray();
    }

    [HttpPost("chat/{chatId:guid}/message")]
    public async Task SendAsync(Guid chatId, [FromBody] SendMessageRequestDto request)
    {
        var context = await _chatifyAspNetCoreContext.GetAsync();
        var content = new MessageContent(request.Text, request.Files);
        await _chatify.AddMessagesAsync(new[] { new AddMessageCommand(chatId, content, context, request.RequestId) });
    }
}