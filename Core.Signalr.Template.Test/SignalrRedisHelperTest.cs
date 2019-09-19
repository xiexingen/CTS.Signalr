using Core.Signalr.Template.Web.Cores;
using Core.Signalr.Template.Web.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Core.Signalr.Template.Test
{
    public class SignalrRedisHelperTest:BaseTest
    {
        public SignalrRedisHelper _redis=> new SignalrRedisHelper(Microsoft.Extensions.Options.Options.Create<AppSetting>(GetAppSetting()));

        [Fact]
        public async Task TestAddConnectForUserAsync()
        {
            Stopwatch sw=new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                await _redis.AddConnectForUserAsync($"test:{i}",$"connection1{i}");
                await _redis.AddConnectForUserAsync($"test:{i}", $"connection2{i}");
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Fact]
        public async Task TestGetConnectsByUserAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
               var connectionId=await _redis.GetConnectsByUserAsync($"test:{i}");
                Console.WriteLine(connectionId);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
