using System;
using System.Threading.Tasks;
using Abp.RealTime;
using JK.Chat.WebSocketPackage;

namespace JK.Chat
{
    public abstract class OnlineClientHubBase : WebSocketHandler
    {
        protected IOnlineClientManager OnlineClientManager { get; }

        protected OnlineClientHubBase(
            IOnlineClientManager onlineClientManager,
            WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            OnlineClientManager = onlineClientManager;
        }

        public override async Task OnConnected(string connectionId, WebSocketClient client)
        {
            await base.OnConnected(connectionId, client);

            var onlineClient = new OnlineClient(
                client.ConnectionId,
                client.IpAddress,
                client.TenantId,
                client.UserId
            );
            onlineClient["UserName"] = "";
            OnlineClientManager.Add(onlineClient);
        }

        public override async Task OnDisconnected(WebSocketClient client)
        {
            await base.OnDisconnected(client);

            try
            {
                OnlineClientManager.Remove(client.ConnectionId);
            }
            catch (Exception ex)
            {

            }

        }
    }
}