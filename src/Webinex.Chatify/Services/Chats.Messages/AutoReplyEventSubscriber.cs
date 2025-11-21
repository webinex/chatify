using Webinex.Chatify.Abstractions;
using Webinex.Chatify.Abstractions.Events;
using Webinex.Chatify.Services.Chats.Members;

namespace Webinex.Chatify.Services.Chats.Messages;

[EventSubscriberPriority(-400)]
internal class AutoReplyEventSubscriber : IEventSubscriber<IEnumerable<ChatMessageSentEvent>>
{
    private readonly ISendMessageService _sendMessageService;
    private readonly IChatMemberService _memberService;
    private readonly IAccountService _accountService;

    public AutoReplyEventSubscriber(
        ISendMessageService sendMessageService,
        IChatMemberService memberService,
        IAccountService accountService)
    {
        _sendMessageService = sendMessageService;
        _memberService = memberService;
        _accountService = accountService;
    }

    public async Task InvokeAsync(IEnumerable<ChatMessageSentEvent> events)
    {
        var eventArray = events.Where(x => !x.IsAutoReply).ToArray();

        var chatMembersByChatId = await _memberService.ActiveIdByChatIdAsync(events.Select(x => x.ChatId));
        var accountById = await _accountService.ByIdAsync(events.SelectMany(x => chatMembersByChatId[x.ChatId]));

        var autoReplyMessages = new List<SendChatMessageArgs>();

        foreach (var sentEvent in eventArray)
        {
            if (sentEvent.AuthorId == AccountContext.System.Id)
                continue;

            var recipientIds = chatMembersByChatId[sentEvent.ChatId];

            foreach (var recipientId in recipientIds)
            {
                if (recipientId == sentEvent.AuthorId)
                    continue;

                if (!accountById.TryGetValue(recipientId, out var recipientAccount))
                    continue;

                if (recipientAccount.AutoReply == null)
                    continue;

                var autoReply = recipientAccount.AutoReply;

                bool isInPeriod = autoReply.Period.Contains(sentEvent.SentAt);

                if (!isInPeriod)
                    continue;

                var replyContext = new AccountContext(recipientAccount.Id, recipientAccount.WorkspaceId);
                var replyBody = new MessageBody(autoReply.Text, []);

                autoReplyMessages.Add(new SendChatMessageArgs(
                        sentEvent.ChatId,
                        replyBody,
                        replyContext
                    ));
            }
        }

        if (autoReplyMessages.Count > 0)
            await _sendMessageService.SendRangeAsync(autoReplyMessages, isAutoReply: true);
    }
}
