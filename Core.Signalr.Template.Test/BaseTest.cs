using Core.Signalr.Template.Web.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Core.Signalr.Template.Test
{
    public class BaseTest
    {
        public AppSetting GetAppSetting()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false);
            var configuration = builder.Build();

            return configuration.GetSection("App").Get<AppSetting>();
        }
    }
}
