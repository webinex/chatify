using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Messages;

internal class MessageMapper
{
    public static Message Map(MessageRow message, bool? read, Message.Props props = Message.Props.Default)
    {
        message = message ?? throw new ArgumentNullException(nameof(message));

        var readRequested = props.HasFlag(Message.Props.Read);
        var authorRequested = props.HasFlag(Message.Props.Author);
        
        if (authorRequested && message.Author == null)
            throw new InvalidOperationException("Author is requested but not loaded");
        
        if (readRequested && read == null)
            throw new InvalidOperationException("Read is requested but read not provided");

        return new Message(
            id: message.Id,
            chatId: message.ChatId,
            authorId: message.AuthorId,
            sentAt: message.SentAt,
            body: new MessageBody(message.Text, message.Files),
            read: readRequested ? Optional.Value(read!.Value) : Optional.NoValue<bool>(),
            author: authorRequested ? Optional.Value(message.Author!.ToAbstraction()) : Optional.NoValue<Account>());
    }
}
