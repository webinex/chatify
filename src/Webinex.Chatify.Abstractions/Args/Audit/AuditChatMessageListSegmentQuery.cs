using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public record AuditChatMessageListSegmentQuery(Guid ChatId, PagingRule PagingRule, bool IncludeTotal = false);
