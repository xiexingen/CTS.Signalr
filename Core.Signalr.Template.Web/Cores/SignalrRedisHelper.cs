using Core.Signalr.Template.Web.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Cores
{
    public class SignalrRedisHelper
    {
        private readonly IOptions<AppSetting> _appSetting;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private static readonly string _preFix_User= "_signalrc_user";
        private static readonly string _preFix_Group = "_signalrc_group";

        private static IDatabase _database;

        public SignalrRedisHelper(IOptions<AppSetting> redisSetting)
        {
            _appSetting = redisSetting;
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_appSetting.Value.RedisCache.ConnectionString);
        }

        public IDatabase GetDatabase()
        {
            if (_database == null)
            {
                lock (_preFix)
                {
                    if (_database == null)
                    {
                        _database = _connectionMultiplexer.GetDatabase(_appSetting.Value.RedisCache.DatabaseId);
                    }
                }
            }
            return _database;
        }

        public async Task AddConnectForUserAsync(string userId, string connectionId)
        {
            await GetDatabase().SetAddAsync($"{_preFix_User}{userId}", connectionId);
        }
        
        public async Task<List<string>> GetConnectsByUser(string userId)
        {
            return (await GetDatabase().ListRangeAsync($"{_preFix_User}{userId}"))
                .Select(m=>m.ToString()).ToList();
        }
        
        public async Task RemoveConnectForUser(string userId,string connectionId)
        {
            await GetDatabase().SetRemoveAsync($"{_preFix_User}{userId}", connectionId);
        }

        public async Task AddUserForGroupAsync(string group, string userId)
        {
            await GetDatabase().SetAddAsync($"{_preFix_Group}{group}", userId);
        }

        public async Task<List<string>> GetUsersByGroup(string group)
        {
            return (await GetDatabase().ListRangeAsync($"{_preFix_Group}{group}"))
                .Select(m => m.ToString()).ToList();
        }

        public async Task RemoveUserForGroup(string group, string userId)
        {
            await GetDatabase().SetRemoveAsync($"{_preFix_Group}{group}", userId);
        }




        ///// <summary>
        ///// 从redis缓存中的组移除指定用户，并返回组名
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public async Task<List<string>> RemoveUserFromGroup(string userId)
        //{ 
        //    // TODO 查找所有组中含有该userId的组，将该用户移除 并返回组名
        //    return new List<string>();    
        //}
    }
}
