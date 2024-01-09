using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Services;

internal interface IAuthorizationPolicy
{
    Task AuthorizeSendAsync(SendMessageArgs[] args);
    Task AuthorizeAddMemberAsync(AddMemberArgs[] args);
    Task AuthorizeRemoveMemberAsync(RemoveMemberArgs[] args);
    Task AuthorizeGetChatAsync(AccountContext onBehalfOf, IEnumerable<Guid> chatIds);
}

internal class AuthorizationPolicy : IAuthorizationPolicy
{
    private readonly IMemberService _memberService;

    public AuthorizationPolicy(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public async Task AuthorizeSendAsync(SendMessageArgs[] args)
    {
        var chatIds = args.Select(x => x.ChatId).Distinct().ToArray();
        var chatMembers = await _memberService.IdByChatIdAsync(chatIds);

        var forbidden = args.Where(x =>
                !x.OnBehalfOf.IsSystem() && !chatMembers[x.ChatId].Contains(x.OnBehalfOf.Id))
            .ToArray();

        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to send message by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.ChatId}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeAddMemberAsync(AddMemberArgs[] args)
    {
        var chatIds = args.Where(x => !x.OnBehalfOf.IsSystem()).Select(x => x.ChatId).Distinct().ToArray();
        var memberByChatId = await _memberService.IdByChatIdAsync(chatIds);

        var forbidden = args.Where(x => !x.OnBehalfOf.IsSystem() && !memberByChatId[x.ChatId].Contains(x.OnBehalfOf.Id))
            .ToArray();
        
        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to add member to chat by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.ChatId}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeRemoveMemberAsync(RemoveMemberArgs[] args)
    {
        var chatIds = args.Where(x => !x.OnBehalfOf.IsSystem()).Select(x => x.ChatId).Distinct().ToArray();
        var memberByChatId = await _memberService.IdByChatIdAsync(chatIds);

        var forbidden = args.Where(x => !x.OnBehalfOf.IsSystem() && !memberByChatId[x.ChatId].Contains(x.OnBehalfOf.Id))
            .ToArray();
        
        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to remove member from chat by not a member of chat. {string.Join(", ", forbidden.Select(x => $"[{x.ChatId}, {x.OnBehalfOf.Id}]"))}");
    }

    public async Task AuthorizeGetChatAsync(AccountContext onBehalfOf, IEnumerable<Guid> chatIds)
    {
        if (onBehalfOf.IsSystem())
            return;
        
        chatIds = chatIds.Distinct().ToArray();
        var memberByChatId = await _memberService.IdByChatIdAsync(chatIds);

        var forbidden = chatIds.Where(id => !memberByChatId[id].Contains(onBehalfOf.Id))
            .ToArray();
        
        if (forbidden.Any())
            throw new UnauthorizedAccessException(
                $"Attempt to get chat by not a member of chat {string.Join(", ", forbidden.Select(id => $"[{id}, {onBehalfOf.Id}]"))}");
    }
}
