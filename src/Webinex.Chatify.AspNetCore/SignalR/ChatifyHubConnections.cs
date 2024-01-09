namespace Webinex.Chatify.AspNetCore;

public interface IChatifyHubConnections
{
    void Add(string userId, string connectionId);
    void Remove(string userId, string connectionId);
    bool Connected(string id);
}

internal class ChatifyHubConnections : IChatifyHubConnections
{
    private readonly object _lock = new();
    private readonly IDictionary<string, LinkedList<string>> _connections = new Dictionary<string, LinkedList<string>>();
    
    public void Add(string userId, string connectionId)
    {
        lock (_lock)
        {
            if (!_connections.ContainsKey(userId))
            {
                _connections[userId] = new LinkedList<string>();
                return;
            }

            _connections[userId].AddLast(connectionId);
        }
    }

    public void Remove(string userId, string connectionId)
    {
        lock (_lock)
        {
            if (!_connections.ContainsKey(userId))
            {
                return;
            }

            if (_connections[userId].Count == 1)
            {
                _connections.Remove(userId);
                return;
            }

            _connections[userId].Remove(connectionId);
        }
    }

    public bool Connected(string id)
    {
        // Synchronization is not required for read operations
        // ReSharper disable once InconsistentlySynchronizedField
        return _connections.ContainsKey(id);
    }
}