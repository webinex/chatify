using System.Linq.Expressions;
using Webinex.Asky;
using Webinex.Chatify.Rows.Chats;

namespace Webinex.Chatify.Services.Chats.Messages;

internal class ChatMessageRowFieldMap : IAskyFieldMap<ChatMessageRow>
{
    public Expression<Func<ChatMessageRow, object>>? this[string fieldId] => fieldId switch
    {
        "chatId" => x => x.ChatId,
        "sentAt" => x => x.SentAt,
        _ => null,
    };
}
