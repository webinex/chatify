using System.Linq.Expressions;
using Webinex.Asky;
using Webinex.Chatify.Rows;

namespace Webinex.Chatify.Services.Messages;

internal class MessageRowFieldMap : IAskyFieldMap<MessageRow>
{
    public Expression<Func<MessageRow, object>>? this[string fieldId] => fieldId switch
    {
        "chatId" => x => x.ChatId,
        "sentAt" => x => x.SentAt,
        _ => null,
    };
}
