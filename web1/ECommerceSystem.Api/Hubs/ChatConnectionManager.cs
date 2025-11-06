using System.Collections.Concurrent;

namespace ECommerceSystem.Api.Hubs
{
    public class ChatConnectionManager
    {
        // userId -> List<connectionId>
        private readonly ConcurrentDictionary<int, ConcurrentBag<string>> _connections
            = new();

        // Thêm connection
        public void AddConnection(int userId, string connectionId)
        {
            if (!_connections.ContainsKey(userId))
            {
                _connections[userId] = new ConcurrentBag<string>();
            }

            _connections[userId].Add(connectionId);
        }

        // Xóa connection
        public void RemoveConnection(int userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var conns))
            {
                var updated = new ConcurrentBag<string>(conns.Where(c => c != connectionId));
                if (updated.Any())
                    _connections[userId] = updated;
                else
                    _connections.TryRemove(userId, out _);
            }
        }

        // Lấy tất cả connection của user
        public List<string> GetConnections(int userId)
        {
            if (_connections.TryGetValue(userId, out var conns))
                return conns.ToList();
            return new List<string>();
        }
        public List<int> GetOnlineUserIds()
        {
            return _connections.Keys.ToList();
        }

    }
}
