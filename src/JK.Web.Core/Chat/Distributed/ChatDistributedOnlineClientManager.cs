using Abp;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.RealTime;
using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Runtime.Caching;

namespace JK.Chat.Distributed
{
    public class ChatDistributedOnlineClientManager : IOnlineClientManager<ChatChannel>
    {
        private readonly ICacheManager _cacheManager;
        public event EventHandler<OnlineClientEventArgs> ClientConnected;
        public event EventHandler<OnlineClientEventArgs> ClientDisconnected;
        public event EventHandler<OnlineUserEventArgs> UserConnected;
        public event EventHandler<OnlineUserEventArgs> UserDisconnected;

        /// <summary>
        /// Online clients.
        /// </summary>
        protected ConcurrentDictionary<string, IOnlineClient> Clients { get; }

        protected readonly object SyncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
        /// </summary>
        public ChatDistributedOnlineClientManager(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            Clients = new ConcurrentDictionary<string, IOnlineClient>();
        }

        public virtual void Add(IOnlineClient client)
        {
            lock (SyncObj)
            {
                var userWasAlreadyOnline = false;
                var user = client.ToUserIdentifierOrNull();

                if (user != null)
                {
                    userWasAlreadyOnline = this.IsOnline(user);
                }

                Clients[client.ConnectionId] = client;
                _cacheManager.GetCache<string, IOnlineClient>(nameof(IOnlineClient))
                    .Set(client.ConnectionId, client);
                ClientConnected.InvokeSafely(this, new OnlineClientEventArgs(client));

                if (user != null && !userWasAlreadyOnline)
                {
                    UserConnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
                }
            }
        }

        public virtual bool Remove(string connectionId)
        {
            lock (SyncObj)
            {
                IOnlineClient client;
                var isRemoved = Clients.TryRemove(connectionId, out client);

                if (isRemoved)
                {
                    var user = client.ToUserIdentifierOrNull();

                    if (user != null && !this.IsOnline(user))
                    {
                        UserDisconnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
                    }

                    ClientDisconnected.InvokeSafely(this, new OnlineClientEventArgs(client));
                }
                _cacheManager.GetCache<string, IOnlineClient>(nameof(IOnlineClient)).Remove(connectionId);
                return isRemoved;
            }
        }

        public virtual IOnlineClient GetByConnectionIdOrNull(string connectionId)
        {
            lock (SyncObj)
            {
                var rs = Clients.GetOrDefault(connectionId);
                if (rs == null)
                {
                    rs = _cacheManager.GetCache<string, IOnlineClient>(nameof(IOnlineClient))
                        .GetOrDefault(connectionId);
                }

                return rs;
            }
        }

        public virtual IReadOnlyList<IOnlineClient> GetAllClients()
        {
            lock (SyncObj)
            {
                return Clients.Values.ToImmutableList();
            }
        }

        [NotNull]
        public virtual IReadOnlyList<IOnlineClient> GetAllByUserId([NotNull] IUserIdentifier user)
        {
            Check.NotNull(user, nameof(user));

            return GetAllClients()
                 .Where(c => (c.UserId == user.UserId && c.TenantId == user.TenantId))
                 .ToImmutableList();
        }
    }
}