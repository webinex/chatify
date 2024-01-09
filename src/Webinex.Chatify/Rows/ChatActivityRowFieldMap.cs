using System.Linq.Expressions;
using Webinex.Asky;

namespace Webinex.Chatify.Rows;

internal class ChatActivityRowFieldMap : IAskyFieldMap<ChatActivityRow>
{
    public Expression<Func<ChatActivityRow, object>>? this[string fieldId] => fieldId switch
    {
        "id" => x => x.Chat!.Id,
        "name" => x => x.Chat!.Name,
        "createdAt" => x => x.Chat!.CreatedAt,
        "createdById" => x => x.Chat!.CreatedById,
        "read" => x => x.Delivery!.Read,
        _ => null,
    };
}
