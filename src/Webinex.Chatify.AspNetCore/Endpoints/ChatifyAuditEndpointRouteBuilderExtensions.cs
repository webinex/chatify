using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Webinex.Chatify.Abstractions;

namespace Webinex.Chatify.AspNetCore.Endpoints;

public static class ChatifyAuditEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapChatifyAudit(
        this IEndpointRouteBuilder endpoints,
        string basePath = "/api/chatify/audit")
    {
        var auditApi = endpoints.MapGroup(basePath)
            .WithTags("Chatify Audit")
            .WithDisplayName("Chatify Audit");

        auditApi.MapGet("/chats", async (
            [FromServices] IChatifyAuditInteractor interactor,
            [FromServices] IOptions<JsonOptions> jsonOption,
            [FromQuery] string? query = null) =>
        {
            if (query == null)
                return await interactor.ChatListSegmentAsync(new AuditChatListSegmentQuery());

            var qObject = JsonSerializer.Deserialize<AuditChatListSegmentQuery>(query,
                jsonOption.Value.JsonSerializerOptions);
            return await interactor.ChatListSegmentAsync(qObject ?? new AuditChatListSegmentQuery());
        });

        auditApi.MapGet("/chats/{id}", async (
            [FromRoute] Guid id,
            [FromServices] IChatifyAuditInteractor interactor) => await interactor.ChatAsync(id));

        auditApi.MapGet("/messages", async (
            [FromServices] IChatifyAuditInteractor interactor,
            [FromServices] IOptions<JsonOptions> jsonOption,
            [FromQuery] string query) =>
        {
            if (string.IsNullOrWhiteSpace(query))
                return Results.BadRequest("Query parameter is required.");
            
            var qObject = JsonSerializer.Deserialize<AuditChatMessageListSegmentQuery>(query,
                jsonOption.Value.JsonSerializerOptions);
            qObject = qObject ?? throw new InvalidOperationException();
            var result = await interactor.ChatMessageListSegmentAsync(qObject);
            return Results.Ok(result);
        });

        return endpoints;
    }
}
