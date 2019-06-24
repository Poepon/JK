using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Castle.Facilities.Logging;
using Exceptionless;
using JK.Abp.RabbitMQ;
using JK.Authentication.JwtBearer;
using JK.Chat;
using JK.Chat.WebSocketPackage;
using JK.Configuration;
using JK.Identity;
using JK.Web.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Linq;

namespace JK.Web.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(() =>
                {
                    return
                        ConnectionMultiplexer.Connect(_appConfiguration["RedisCache:ConnectionString"])
                            .GetDatabase(_appConfiguration.GetValue<int>("RedisCache:DatabaseId"));
                }, "JKProtection-Keys");

            services.AddMiniProfiler().AddEntityFramework();

            services.AddRabbitMQ(_appConfiguration);

            // MVC
            services.AddMvc(
                options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            );

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            services.AddScoped<IWebResourceManager, WebResourceManager>();


            // Configure Abp and Dependency Injection
            return services.AddAbp<JKWebMvcModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider service, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("~/Error?statusCode={0}");
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseJwtTokenMiddleware();

            app.UseWebSockets();

            app.MapWebSocketManager("/chatws", service.GetService<ChatHub>());

            if (Environment.GetEnvironmentVariable("EnableExceptionless") == "true")
                app.UseExceptionless(_appConfiguration);

            if (Environment.GetEnvironmentVariable("EnableProfiler") == "true")
                app.UseMiniProfiler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
