namespace Webinex.Chatify.Abstractions;

public record SendThreadMessageArgs(string ThreadId, MessageBody Body, AccountContext OnBehalfOf);
