using Abp.Dependency;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Chat
{
    public class WebSocketConnectionManager : ISingletonDependency
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<string, List<string>> _groups = new ConcurrentDictionary<string, List<string>>();

        public WebSocket GetSocket(string connectionId)
        {
            return _sockets.FirstOrDefault(p => p.Key == connectionId).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public List<string> GetAllFromGroup(string GroupID)
        {
            if (_groups.ContainsKey(GroupID))
            {
                return _groups[GroupID];
            }

            return default(List<string>);
        }

        public string GetConnectionId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AddSocket(string connectionId, WebSocket socket)
        {
            _sockets.TryAdd(connectionId, socket);
        }

        public void AddToGroup(string connectionId, string groupID)
        {
            if (_groups.ContainsKey(groupID))
            {
                if (!_groups[groupID].Contains(connectionId))
                    _groups[groupID].Add(connectionId);
                return;
            }

            _groups.TryAdd(groupID, new List<string> { connectionId });
        }

        public void RemoveFromGroup(string connectionId, string groupId)
        {
            if (_groups.ContainsKey(groupId))
            {
                _groups[groupId].Remove(connectionId);
            }
        }

        public async Task RemoveSocket(string connectionId)
        {
            if (connectionId == null) return;

            WebSocket socket;
            _sockets.TryRemove(connectionId, out socket);

            if (socket == null || socket.State != WebSocketState.Open) return;

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketManager",
                                    cancellationToken: CancellationToken.None);
        }
    }
}
