using Core.Signalr.Template.Web.Dtos;
using Core.Signalr.Template.Web.Models;
using Microsoft.Extensions.Logging;
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
        public static readonly string PREFIXUSER= "signalr_u_";
        public static readonly string PREFIXGROUP = "signalr_g_";

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
                lock (PREFIXUSER)
                {
                    if (_database == null)
                    {
                        _database = _connectionMultiplexer.GetDatabase(_appSetting.Value.RedisCache.DatabaseId);
                    }
                }
            }
            return _database;
        }

        public IServer GetServer()
        {
            return _connectionMultiplexer.GetServer(_appSetting.Value.RedisCache.ConnectionString);
        }

        public async Task AddConnectForUserAsync(string userId, string connectionId)
        {
            await GetDatabase().SetAddAsync($"{PREFIXUSER}{userId}", connectionId);
        }
        
        public async Task<List<string>> GetConnectsByUserAsync(string userId)
        {
            return (await GetDatabase().SetMembersAsync($"{PREFIXUSER}{userId}"))
                .Select(m=>m.ToString()).ToList();
        }
        
        public async Task RemoveConnectForUserAsync(string userId,string connectionId)
        {
            await GetDatabase().SetRemoveAsync($"{PREFIXUSER}{userId}", connectionId);
        }

        public async Task AddUserForGroupAsync(string group,string connectId, string userId)
        {
            await GetDatabase().HashSetAsync($"{PREFIXGROUP}{group}",connectId,userId);
        }

        public async Task RemoveConnectFromGroupAsync(string group,string connectId)
        {
            await GetDatabase().HashDeleteAsync($"{PREFIXGROUP}{group}",connectId);
        }

        public async Task<List<UserConnection>> GetUsersByGroupAsync(string group)
        {
            var hashUsers=await GetDatabase().HashGetAllAsync($"{PREFIXGROUP}{group}");
            var users= hashUsers.Select(m=>new UserConnection(){
                ConnectionId = m.Name.ToString(),
                UserId =m.Value.ToString()                
            }).ToList();
            return users;
        }

        //public async Task RemoveUserForGroupAsync(string group, string userId)
        //{
        //    await GetDatabase().SetRemoveAsync($"{PREFIXGROUP}{group}", userId);
        //}

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
