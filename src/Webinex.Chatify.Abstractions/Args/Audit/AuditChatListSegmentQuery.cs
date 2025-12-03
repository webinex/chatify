using Webinex.Asky;

namespace Webinex.Chatify.Abstractions;

public record AuditChatListSegmentQuery(
    IReadOnlyCollection<string>? ContainsOneOfMembers = null,
    string? SearchString = null,
    string? WorkspaceId = null,
    PagingRule? PagingRule = null,
    bool IncludeTotal = false);
