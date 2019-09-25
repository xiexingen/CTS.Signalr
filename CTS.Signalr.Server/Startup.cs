using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using CTS.Signalr.Server.Cores;
using CTS.Signalr.Server.Hubs;
using CTS.Signalr.Server.Logs;
using CTS.Signalr.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace CTS.Signalr.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        readonly IWebHostEnvironment env;
        readonly string corsPolicy = "CorsPolicy ";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSection = Configuration.GetSection("App");
            services.Configure<AppSetting>(option => appSection.Bind(option));
            var appSetting = appSection.Get<AppSetting>();

            services.AddSingleton<SignalrRedisHelper>();

            // services.AddHostedService<ClearBackGroundService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.SecurityTokenValidators.Clear();
                option.SecurityTokenValidators.Add(new UserTokenValidation()); ;

                option.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var userId = context.Request.Query["userId"].FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            context.Token = userId;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddCors(options => options.AddPolicy(corsPolicy, builder =>
            {
                builder
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .WithOrigins(appSetting.CORS.Split(","))
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .AllowAnyHeader()
                      .Build();
            }));

            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new BadRequestObjectResult(context.ModelState);
                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        // result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // 添加Signalr
            services.AddSignalR(config =>
            {
                if (env.IsDevelopment())
                {
                    config.EnableDetailedErrors = true;
                }
            })
            // 支持MessagePack
            .AddMessagePackProtocol()
            // 使用redis做底板 支持横向扩展 Scale-out
            .AddStackExchangeRedis(o =>
            {
                o.ConnectionFactory = async writer =>
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false,
                        // Password = "changeme",
                        ChannelPrefix = "__signalr_",
                    };
                    //config.EndPoints.Add(IPAddress.Loopback, 0);
                    //config.SetDefaultPorts();
                    config.DefaultDatabase = appSetting.SignalrRedisCache.DatabaseId;
                    var connection = await ConnectionMultiplexer.ConnectAsync(appSetting.SignalrRedisCache.ConnectionString, writer);
                    connection.ConnectionFailed += (_, e) =>
                    {
                        Console.WriteLine("Connection to Redis failed.");
                    };

                    if (connection.IsConnected)
                    {
                        Console.WriteLine("connected to Redis.");
                    }
                    else
                    {
                        Console.WriteLine("Did not connect to Redis");
                    }

                    return connection;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHsts();

            loggerFactory.AddLog4Net();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(corsPolicy);
            //app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Users}/{action=Get}/{id?}");

                endpoints.MapHub<NotifyHub>("/notify-hub");
            });

        }
    }
}
