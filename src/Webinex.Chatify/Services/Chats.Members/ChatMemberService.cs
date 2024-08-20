using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Services.Chats.Members;

internal interface IChatMemberService
{
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<ChatMember>>> ByChatsAsync(IEnumerable<Guid> chatIds, bool? active = null);
    Task RemoveRangeAsync(IEnumerable<RemoveChatMemberArgs> args);
    Task AddRangeAsync(IEnumerable<AddChatMemberArgs> args);
    Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds);
}

internal class ChatMemberService : IChatMemberService
{
    private readonly IGetChatMemberService _getChatMemberService;
    private readonly IAddChatMemberService _addChatMemberService;
    private readonly IRemoveChatMemberService _removeChatMemberService;

    public ChatMemberService(
        IGetChatMemberService getChatMemberService,
        IAddChatMemberService addChatMemberService,
        IRemoveChatMemberService removeChatMemberService)
    {
        _getChatMemberService = getChatMemberService;
        _addChatMemberService = addChatMemberService;
        _removeChatMemberService = removeChatMemberService;
    }

    public Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<ChatMember>>> ByChatsAsync(IEnumerable<Guid> chatIds, bool? active = null)
    {
        return _getChatMemberService.ByChatsAsync(chatIds, active);
    }

    public Task RemoveRangeAsync(IEnumerable<RemoveChatMemberArgs> args)
    {
        return _removeChatMemberService.RemoveRangeAsync(args);
    }

    public Task AddRangeAsync(IEnumerable<AddChatMemberArgs> args)
    {
        return _addChatMemberService.AddRangeAsync(args);
    }

    public Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds)
    {
        return _getChatMemberService.ActiveIdByChatIdAsync(chatIds);
    }
}
