using Core.Signalr.Template.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // 添加Signalr
            services.AddSignalR()
                // 使用redis做地板 支持横向扩展 Scale-out
                .AddRedis(Configuration.GetValue<string>("App:RedisCache:ConnectionString"), option=>{
                    option.Configuration=new ConfigurationOptions()
                    {
                        DefaultDatabase= Configuration.GetValue<int>("App:RedisCache:DatabaseId")
                    }; 
                })
                // 支持MessagePack
                .AddMessagePackProtocol();
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
            app.UseStaticFiles();
            app.UseSignalR(route =>
            {
                route.MapHub<ServerNotifyHub>("/server-notify");
            });
            app.UseMvc(config => {
                config.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
