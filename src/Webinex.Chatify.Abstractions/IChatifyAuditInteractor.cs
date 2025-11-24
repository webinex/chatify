using Webinex.Chatify.Abstractions.Audit;

namespace Webinex.Chatify.Abstractions;

public interface IChatifyAuditInteractor
{
    Task<ListSegment<AuditChat>> ChatListSegmentAsync(AuditChatListSegmentQuery query);
    Task<AuditChat?> ChatAsync(Guid id);
    Task<ListSegment<AuditChatMessage>> ChatMessageListSegmentAsync(AuditChatMessageListSegmentQuery query);
}
