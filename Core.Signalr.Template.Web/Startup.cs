using Core.Signalr.Template.Web.Cores;
using Core.Signalr.Template.Web.Hubs;
using Core.Signalr.Template.Web.Logs;
using Core.Signalr.Template.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

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

        readonly string corsPolicy = "CorsPolicy ";

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
            })
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
            if (_webEnv.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
                // app.UseHttpsRedirection();
            }

            loggerFactory.AddLog4Net();

            app.UseStaticFiles();
            app.UseCors(corsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseCookiePolicy();
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Values}/{action=Get}/{id?}");

                endpoints.MapHub<NotifyHub>("/notify-hub");
            });
        }
    }
}
