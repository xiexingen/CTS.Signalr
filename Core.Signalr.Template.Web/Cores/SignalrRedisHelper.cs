using Core.Signalr.Template.Web.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Cores
{
    public class SignalrRedisHelper
    {
        private readonly IOptions<AppSetting> _appSetting;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private static readonly string _preFix="_signalr_";
        public SignalrRedisHelper(IOptions<AppSetting> redisSetting)
        {
            _appSetting = redisSetting;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_appSetting.Value.RedisCache.ConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.GetDatabase(_appSetting.Value.RedisCache.DatabaseId);
        }

        public async Task SetAddAsync(string key,string v) {
            await GetDatabase().SetAddAsync($"{_preFix}{key}",v);
        }

        public async Task SetRemoveAsync(string key, string v)
        {
            await GetDatabase().SetRemoveAsync($"{_preFix}{key}", v);
        }
    }
}
