using Webinex.Chatify.Types;

namespace Webinex.Chatify.Abstractions;

public interface IChatify
{
    Task<IReadOnlyCollection<Chat>> AddChatsAsync(IEnumerable<AddChatCommand> commands);
    Task<Message[]> AddMessagesAsync(IEnumerable<AddMessageCommand> commands);
    Task AddMembersAsync(IEnumerable<AddMemberCommand> commands);
    Task RemoveMembersAsync(IEnumerable<RemoveMemberCommand> commands);
    Task<ChatQueryResult> QueryAsync(ChatQuery query);
    Task<MessageQueryResult> QueryAsync(MessageQuery query);
    Task<IReadOnlyCollection<Chat>> ChatByIdAsync(IEnumerable<Guid> chatIds);
    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<Member>>> MembersAsync(IEnumerable<Guid> chatIds);
    Task<IReadOnlyCollection<Account>> AccountByIdAsync(IEnumerable<string>? ids = null);
    Task ReadAsync(ReadCommand command);
    Task<Account[]> AddAccountsAsync(IEnumerable<AddAccountCommand> commands);
    Task<Account[]> UpdateAccountsAsync(IEnumerable<UpdateAccountCommand> commands);
}

public static class ChatifyExtensions
{
    public static async Task<Chat> AddChatAsync(this IChatify chatify, AddChatCommand command)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        command = command ?? throw new ArgumentNullException(nameof(command));

        var result = await chatify.AddChatsAsync(new[] { command });
        return result.First();
    }
    
    public static async Task<IReadOnlyCollection<Member>> MembersAsync(this IChatify chatify, Guid chatId)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        var result = await chatify.MembersAsync(new[] { chatId });
        return result.Values.First();
    }
    
    public static async Task<Chat> ChatAsync(this IChatify chatify, Guid chatId)
    {
        chatify = chatify ?? throw new ArgumentNullException(nameof(chatify));
        var result = await chatify.ChatByIdAsync(new[] { chatId });
        return result.First();
    }
} 