using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats.Messages;

internal static class ChatMessageMapper
{
    public static ChatMessage Map(ChatMessageRow chatMessage, bool? read, ChatMessage.Props props = ChatMessage.Props.Default)
    {
        chatMessage = chatMessage ?? throw new ArgumentNullException(nameof(chatMessage));

        var readRequested = props.HasFlag(ChatMessage.Props.Read);
        var authorRequested = props.HasFlag(ChatMessage.Props.Author);
        
        if (authorRequested && chatMessage.Author == null)
            throw new InvalidOperationException("Author is requested but not loaded");
        
        if (readRequested && read == null)
            throw new InvalidOperationException("Read is requested but read not provided");

        return new ChatMessage(
            id: chatMessage.Id,
            chatId: chatMessage.ChatId,
            authorId: chatMessage.AuthorId,
            sentAt: chatMessage.SentAt,
            body: new MessageBody(chatMessage.Text, chatMessage.Files),
            read: readRequested ? Optional.Value(read!.Value) : Optional.NoValue<bool>(),
            author: authorRequested ? Optional.Value(chatMessage.Author.ToAbstraction()) : Optional.NoValue<Account>());
    }
}
