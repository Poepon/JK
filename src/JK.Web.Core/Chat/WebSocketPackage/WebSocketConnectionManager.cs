﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.RealTime;

namespace JK.Chat.WebSocketPackage
{
    public class WebSocketConnectionManager : ISingletonDependency
    {
        protected IOnlineClientManager OnlineClientManager { get; }
        protected ConcurrentDictionary<string, WebSocketClient> Clients { get; }
        protected ConcurrentDictionary<string, List<string>> Groups { get; }

        public WebSocketConnectionManager(IOnlineClientManager onlineClientManager)
        {
            OnlineClientManager = onlineClientManager;
            Clients = new ConcurrentDictionary<string, WebSocketClient>();
            Groups = new ConcurrentDictionary<string, List<string>>();
        }

        protected readonly object SyncObj = new object();

        public WebSocketClient GetWebSocketClient(string connectionId)
        {
            return Clients.FirstOrDefault(p => p.Key == connectionId).Value;
        }

        public virtual IReadOnlyList<WebSocketClient> GetAllClients()
        {
            lock (SyncObj)
            {
                return Clients.Values.ToImmutableList();
            }
        }

        public ConcurrentDictionary<string, WebSocketClient> GetAll()
        {
            return Clients;
        }

        public List<string> GetAllFromGroup(string GroupID)
        {
            if (Groups.ContainsKey(GroupID))
            {
                return Groups[GroupID];
            }

            return new List<string>();
        }

        public string GetConnectionId(WebSocketClient client)
        {
            return Clients.FirstOrDefault(p => p.Value.Equals(client)).Key;
        }

        public virtual IReadOnlyList<WebSocketClient> GetAllByUserId(long userId)
        {
            return GetAllClients()
                 .Where(c => c.UserId == userId)
                 .ToImmutableList();
        }

        public void AddWebSocketClient(string connectionId, WebSocketClient socket)
        {
            Clients.TryAdd(connectionId, socket);
        }

        public void AddToGroup(string connectionId, string groupID)
        {
            if (Groups.ContainsKey(groupID))
            {
                if (!Groups[groupID].Contains(connectionId))
                    Groups[groupID].Add(connectionId);
                return;
            }

            Groups.TryAdd(groupID, new List<string> { connectionId });
        }

        public void RemoveFromGroup(string connectionId, string groupId)
        {
            if (Groups.ContainsKey(groupId))
            {
                Groups[groupId].Remove(connectionId);
            }
        }

        public async Task RemoveClient(string connectionId)
        {
            if (connectionId == null) return;
            if (Clients.TryRemove(connectionId, out var client))
            {
                WebSocket socket = client.WebSocket;
                if (socket == null || socket.State != WebSocketState.Open) return;

                await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                        statusDescription: "Closed by the WebSocketManager",
                                        cancellationToken: CancellationToken.None);
            }
        }
    }
}
