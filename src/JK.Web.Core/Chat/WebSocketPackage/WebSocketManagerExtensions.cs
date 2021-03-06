﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace JK.Chat.WebSocketPackage
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
