using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.Services.Members;

internal interface IMemberService
{
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> ByChatsAsync(IEnumerable<Guid> chatIds, bool? active = null);
    Task RemoveRangeAsync(IEnumerable<RemoveMemberArgs> args);
    Task AddRangeAsync(IEnumerable<AddMemberArgs> args);
    Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds);
}

internal class MemberService : IMemberService
{
    private readonly IGetMemberService _getMemberService;
    private readonly IAddMemberService _addMemberService;
    private readonly IRemoveMemberService _removeMemberService;

    public MemberService(
        IGetMemberService getMemberService,
        IAddMemberService addMemberService,
        IRemoveMemberService removeMemberService)
    {
        _getMemberService = getMemberService;
        _addMemberService = addMemberService;
        _removeMemberService = removeMemberService;
    }

    public Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> ByChatsAsync(IEnumerable<Guid> chatIds, bool? active = null)
    {
        return _getMemberService.ByChatsAsync(chatIds, active);
    }

    public Task RemoveRangeAsync(IEnumerable<RemoveMemberArgs> args)
    {
        return _removeMemberService.RemoveRangeAsync(args);
    }

    public Task AddRangeAsync(IEnumerable<AddMemberArgs> args)
    {
        return _addMemberService.AddRangeAsync(args);
    }

    public Task<IReadOnlyDictionary<Guid, string[]>> ActiveIdByChatIdAsync(IEnumerable<Guid> chatIds)
    {
        return _getMemberService.ActiveIdByChatIdAsync(chatIds);
    }
}
