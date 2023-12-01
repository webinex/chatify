using Webinex.Chatify.Types;

namespace Webinex.Chatify.Abstractions;

public class MessageQueryResult
{
    public MessageQueryResult(MessageQuery query, Entry[] entries)
    {
        Entries = entries;
        Query = query;
    }

    public MessageQuery Query { get; }
    public Entry[] Entries { get; }

    public record Entry(Message Message, Delivery Delivery, Account Author);
}