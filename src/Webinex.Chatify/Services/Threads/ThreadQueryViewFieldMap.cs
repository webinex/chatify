using System.Linq.Expressions;
using Webinex.Asky;

namespace Webinex.Chatify.Services.Threads;

internal class ThreadQueryViewFieldMap : IAskyFieldMap<ThreadQueryView>
{
    public Expression<Func<ThreadQueryView, object>>? this[string fieldId] => fieldId switch
    {
        "id" => x => x.Thread.Id,
        "name" => x => x.Thread.Name,
        "createdAt" => x => x.Thread.CreatedAt,
        "createdById" => x => x.Thread.CreatedById,
        "archived" => x => x.Thread.Archived,
        "lastMessage.id" => x => x.Thread.LastMessageId!,
        "lastMessage.sentAt" => x => x.LastMessage!.SentAt,
        _ => null,
    };
}
