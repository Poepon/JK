using System;
using System.Net;
using System.Threading.Tasks;
using Abp.RealTime;
using JK.Chat.WebSocketPackage;
using Microsoft.AspNetCore.Http;

namespace JK.Chat
{
    public abstract class OnlineClientHubBase : WebSocketHandler
    {
        private readonly IAppContext _appContext;
        protected IOnlineClientManager OnlineClientManager { get; }

        protected OnlineClientHubBase(
            IAppContext appContext,
            IOnlineClientManager onlineClientManager,
            WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            _appContext = appContext;
            OnlineClientManager = onlineClientManager;
        }

        public override async Task OnConnected(string connectionId, WebSocketClient client, HttpContext context)
        {
            await base.OnConnected(connectionId, client, context);

            var onlineClient = new OnlineClient(
                client.ConnectionId,
                client.IpAddress,
                client.TenantId,
                client.UserId
            );
            onlineClient["UserName"] = "";
            onlineClient["Server"] = _appContext.LocalHostName;
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