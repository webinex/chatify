namespace Webinex.Chatify.Abstractions;

[Flags]
public enum ChatQueryProp
{
    Unspecified = 0,
    LastMessage = 1,
    UnreadCount = 2,
}