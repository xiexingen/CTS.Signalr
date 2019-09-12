using Core.Signalr.Template.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;

namespace Core.Signalr.Template.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            }));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // 添加Signalr
            services.AddSignalR()
                // 使用redis做地板 支持横向扩展 Scale-out
                .AddRedis(Configuration.GetValue<string>("App:RedisCache:ConnectionString"), option =>
                {
                    option.Configuration = new ConfigurationOptions()
                    {
                        DefaultDatabase = Configuration.GetValue<int>("App:RedisCache:DatabaseId")
                    };
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseSignalR(route =>
            {
                route.MapHub<NotifyHub>("/notify-hub");
            });
            app.UseMvc(config =>
            {
                config.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
