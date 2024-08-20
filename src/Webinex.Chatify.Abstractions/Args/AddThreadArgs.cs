namespace Webinex.Chatify.Abstractions;

public record AddThreadArgs(string Id, string Name, AccountContext OnBehalfOf, IEnumerable<string>? Watchers);
