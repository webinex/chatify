using System.Linq.Expressions;
using Webinex.Asky;

namespace Webinex.Chatify.Rows;

internal class ChatRowFieldMap : IAskyFieldMap<ChatRow>
{
    public Expression<Func<ChatRow, object>>? this[string fieldId] => fieldId switch
    {
        "id" => x => x.Id,
        "name" => x => x.Name,
        "createdAt" => x => x.CreatedAt,
        "createdById" => x => x.CreatedById,
        _ => null,
    };
}
