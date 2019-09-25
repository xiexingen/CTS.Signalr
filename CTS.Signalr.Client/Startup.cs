using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTS.Signalr.Client.Helpers;
using CTS.Signalr.Client.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CTS.Signalr.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env=env;
        }

        public IConfiguration Configuration { get; }
        readonly IWebHostEnvironment env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSection = Configuration.GetSection("JwtSetting");
            services.Configure<JwtSetting>(option => appSection.Bind(option));
            var appSetting = appSection.Get<JwtSetting>();

            // services.AddHostedService<ClearBackGroundService>();

            services.AddSingleton<SignalrHelper>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOption =>
                 {
                     cookieOption.LoginPath = "/Account/Login";
                     cookieOption.AccessDeniedPath = "/Account/Login";
                 });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeFilter()))
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHsts();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // todo
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "api",
                    pattern: "api/{controller=Values}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });





        }
    }
}
