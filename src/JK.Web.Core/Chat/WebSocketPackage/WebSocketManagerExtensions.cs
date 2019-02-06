using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat
{
    public static class WebSocketManagerExtensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
            PathString path,WebSocketHandler handler)
        {           
            return app.Map(path, (_app) =>
                _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }
    }
}
