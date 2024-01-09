using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Services;
using Webinex.Chatify.Services.Chats;
using Webinex.Chatify.Services.Messages;

namespace Webinex.Chatify;

internal class Chatify : IChatify
{
    private readonly IAccountService _accountService;
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;
    private readonly IMemberService _memberService;
    private readonly IAuthorizationPolicy _authorizationPolicy;

    public Chatify(
        IAccountService accountService,
        IMessageService messageService,
        IChatService chatService,
        IMemberService memberService,
        IAuthorizationPolicy authorizationPolicy)
    {
        _accountService = accountService;
        _messageService = messageService;
        _chatService = chatService;
        _memberService = memberService;
        _authorizationPolicy = authorizationPolicy;
    }

    public async Task<IReadOnlyCollection<Chat>> AddChatsAsync(IEnumerable<AddChatArgs> commands)
    {
        return await _chatService.AddAsync(commands);
    }

    public async Task<Message[]> SendMessagesAsync(IEnumerable<SendMessageArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return Array.Empty<Message>();

        await _authorizationPolicy.AuthorizeSendAsync(commands.ToArray());
        return await _messageService.SendRangeAsync(commands);
    }

    public async Task AddMembersAsync(IEnumerable<AddMemberArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return;

        await _authorizationPolicy.AuthorizeAddMemberAsync(commands.ToArray());
        await _memberService.AddRangeAsync(commands);
    }

    public async Task RemoveMembersAsync(IEnumerable<RemoveMemberArgs> commands)
    {
        commands = commands.ToArray();
        if (!commands.Any()) return;

        await _authorizationPolicy.AuthorizeRemoveMemberAsync(commands.ToArray());
        await _memberService.RemoveRangeAsync(commands);
    }

    public async Task<Chat[]> QueryAsync(ChatQuery query)
    {
        return await _chatService.QueryAsync(query);
    }

    public async Task<Message[]> QueryAsync(MessageQuery query)
    {
        return await _messageService.QueryAsync(query);
    }

    public async Task<IReadOnlyCollection<Chat>> ChatByIdAsync(
        IEnumerable<Guid> chatIds,
        AccountContext? onBehalfOf = null)
    {
        chatIds = chatIds.Distinct().ToArray();

        if (onBehalfOf != null)
            await _authorizationPolicy.AuthorizeGetChatAsync(onBehalfOf, chatIds);
        
        return await _chatService.ByIdAsync(chatIds);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> MembersAsync(IEnumerable<Guid> chatIds)
    {
        return await _memberService.ByChatsAsync(chatIds);
    }

    public Task<IReadOnlyDictionary<string, Account>> AccountByIdAsync(
        IEnumerable<string> ids,
        bool tryCache = false)
    {
        return _accountService.ByIdAsync(ids, tryCache);
    }

    public Task<IReadOnlyCollection<Account>> AccountsAsync(AccountContext? onBehalfOf = null)
    {
        return _accountService.GetAllAsync(onBehalfOf);
    }

    public async Task ReadAsync(ReadArgs args)
    {
        if (!args.MessageIds.Any())
            return;

        await _messageService.ReadAsync(args);
    }

    public async Task<Account[]> AddAccountsAsync(IEnumerable<AddAccountArgs> commands)
    {
        return await _accountService.AddAsync(commands);
    }

    public async Task<Account[]> UpdateAccountsAsync(IEnumerable<UpdateAccountArgs> commands)
    {
        return await _accountService.UpdateAsync(commands);
    }
}
