using LinqToDB;

namespace Webinex.Chatify.DataAccess;

internal interface IChatifyDataConnectionFactory
{
    ChatifyDataConnection Create();
}

internal class ChatifyDataConnectionFactory : IChatifyDataConnectionFactory
{
    private readonly DataOptions<ChatifyDataConnection> _options;

    public ChatifyDataConnectionFactory(DataOptions<ChatifyDataConnection> options)
    {
        _options = options;
    }

    public ChatifyDataConnection Create()
    {
        return new ChatifyDataConnection(_options);
    }
}
