using System.Linq.Expressions;
using Webinex.Asky;

namespace Webinex.Chatify.Rows;

internal class DeliveryRowFieldMap : IAskyFieldMap<DeliveryRow>
{
    public Expression<Func<DeliveryRow, object>>? this[string fieldId] => fieldId switch
    {
        "chatId" => x => x.ChatId,
        "sentAt" => x => x.Message!.SentAt,
        _ => null,
    };
}
