using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Services.Chats.Members;
using Webinex.Chatify.Services.Chats.Messages;

namespace Webinex.Chatify.Services;

internal interface IAuthorizationPolicy
{
    Task AuthorizeSendAsync(SendChatMessageArgs[] args);
    Task AuthorizeRemoveChatMessageAsync(RemoveChatMessageArgs[] args);
    Task AuthorizeAddChatMemberAsync(AddChatMemberArgs[] args);
    Task AuthorizeRemoveChatMemberAsync(RemoveChatMemberArgs[] args);
    Task AuthorizeUpdateChatNameAsync(UpdateChatNameArgs[] args);
    Task AuthorizeGetChatAsync(AccountContext onBehalfOf, IEnumerable<Guid> chatIds);
}

internal class AuthorizationPolicy : IAuthorizationPolicy
{
    private readonly IChatMemberService _chatMemberService;
    private readonly IChatMessageService _chatMessageService;

    public AuthorizationPolicy(IChatMemberService chatMemberService, IChatMessageService chatMessageService)
    {
        _chatMemberService = chatMemberService;
        _chatMessageService = chatMessageService;
    }

    public async Task AuthorizeSendAsync(SendChatMessageArgs[] args)
    {
        var chatIds = args.Select(x => x.ChatId).Distinct().ToArray();
        var chatMembers = await _chatMemberService.ActiveIdByChatIdAsync(chatIds);

        var forbidden = args.Where(x =>
                !x.OnBehalfOf.IsSystem() && !chatMembers[x.ChatId].Contains(x.OnBehalfOf.Id))
            .ToArray();

        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to send message by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.ChatId}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeRemoveChatMessageAsync(RemoveChatMessageArgs[] args)
    {
        var chatIds = args.Select(x => x.ChatId).Distinct().ToArray();
        var chatMembers = await _chatMemberService.ActiveIdByChatIdAsync(chatIds);

        var messageIds = args.Select(x => x.MessageId).Distinct().ToArray();
        var messageAuthors = await _chatMessageService.AuthorIdByMessageIdAsync(messageIds);

        var forbidden = args
            .Where(x =>
            {
                if (x.OnBehalfOf.IsSystem())
                    return false;

                var isMember = chatMembers[x.ChatId].Contains(x.OnBehalfOf.Id);
                var hasAuthor = messageAuthors.TryGetValue(x.MessageId, out var authorId);
                var isAuthor = hasAuthor && authorId == x.OnBehalfOf.Id;

                return !isMember || !isAuthor;
            })
            .ToArray();

        if (forbidden.Any())
        {
            throw new UnauthorizedAccessException(
                $"Attempt to delete message by not a member of chat or not an author of message. " +
                $"{string.Join(", ", forbidden.Select(x => $"[Chat:{x.ChatId}, Message:{x.MessageId}, User:{x.OnBehalfOf.Id}]"))}");
        }
    }

    public async Task AuthorizeAddChatMemberAsync(AddChatMemberArgs[] args)
    {
        var chatIds = args.Where(x => !x.OnBehalfOf.IsSystem()).Select(x => x.ChatId).Distinct().ToArray();
        var memberByChatId = await _chatMemberService.ActiveIdByChatIdAsync(chatIds);

        var forbidden = args.Where(x => !x.OnBehalfOf.IsSystem() && !memberByChatId[x.ChatId].Contains(x.OnBehalfOf.Id))
            .ToArray();

        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to add member to chat by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.ChatId}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeRemoveChatMemberAsync(RemoveChatMemberArgs[] args)
    {
        var chatIds = args.Where(x => !x.OnBehalfOf.IsSystem()).Select(x => x.ChatId).Distinct().ToArray();
        var memberByChatId = await _chatMemberService.ActiveIdByChatIdAsync(chatIds);

        var forbidden = args.Where(x => !x.OnBehalfOf.IsSystem() && !memberByChatId[x.ChatId].Contains(x.OnBehalfOf.Id))
            .ToArray();

        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to remove member from chat by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.ChatId}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeUpdateChatNameAsync(UpdateChatNameArgs[] args)
    {
        var chatIds = args.Where(x => !x.OnBehalfOf.IsSystem()).Select(x => x.Id).Distinct().ToArray();
        var memberByChatId = await _chatMemberService.ActiveIdByChatIdAsync(chatIds);

        var forbidden = args.Where(x => !x.OnBehalfOf.IsSystem() && !memberByChatId[x.Id].Contains(x.OnBehalfOf.Id))
            .ToArray();

        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to edit chat name by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.Id}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeGetChatAsync(AccountContext onBehalfOf, IEnumerable<Guid> chatIds)
    {
        if (onBehalfOf.IsSystem())
            return;

        chatIds = chatIds.Distinct().ToArray();
        var memberByChatId = await _chatMemberService.ActiveIdByChatIdAsync(chatIds);

        var forbidden = chatIds.Where(id => !memberByChatId[id].Contains(onBehalfOf.Id))
            .ToArray();

        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to get chat by not a member of chat {string.Join(", ", forbidden.Select(id => $"[{id}, {onBehalfOf.Id}]"))}");
    }
}
