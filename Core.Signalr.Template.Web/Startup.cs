using Core.Signalr.Template.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;

namespace Core.Signalr.Template.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webEnv)
        {
            Configuration = configuration;
            _webEnv = webEnv;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _webEnv;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(Configuration.GetValue<string>("App:CORS").Split(","))
                       //.SetIsOriginAllowed(origin => true)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            }));

            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            // 添加Signalr
            services.AddSignalR(config =>
            {
                if (_webEnv.IsDevelopment())
                {
                    config.EnableDetailedErrors = true;
                }
            })
                // 支持MessagePack
                .AddMessagePackProtocol(option =>
                {
                    option.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
                    {
                        MessagePack.Resolvers.DynamicGenericResolver.Instance,
                        MessagePack.Resolvers.StandardResolver.Instance
                    };
                });
            //// 使用redis做底板 支持横向扩展 Scale-out
            //.AddStackExchangeRedis(Configuration.GetValue<string>("App:RedisCache:ConnectionString"), o =>
            // {
            //     o.ConnectionFactory = async writer =>
            //     {
            //         var config = new ConfigurationOptions
            //         {
            //             AbortOnConnectFail = false,
            //            // Password = "changeme"
            //        };
            //        //config.EndPoints.Add(IPAddress.Loopback, 0);
            //        //config.SetDefaultPorts();
            //        config.DefaultDatabase = Configuration.GetValue<int>("App:RedisCache:DatabaseId");
            //         var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
            //         connection.ConnectionFailed += (_, e) =>
            //         {
            //             Console.WriteLine("Connection to Redis failed");
            //         };

            //         if (!connection.IsConnected)
            //         {
            //             Console.WriteLine("Did not connect to Redis.");
            //         }

            //         return connection;
            //     };
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_webEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            // app.UseCookiePolicy();
            app.UseRouting();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<NotifyHub>("/notify-hub");
                endpoints.MapHub<DemoHub>("/demo-hub");
            });
        }
    }
}
