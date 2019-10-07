using CTS.Signalr.Server.Dtos;
using CTS.Signalr.Server.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTS.Signalr.Server.Cores
{
    public class SignalrRedisHelper
    {
        private readonly IOptions<AppSetting> _appSetting;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private static IDatabase _database;

        private RedisValue token = Environment.MachineName;

        public static readonly string PREFIXUSER= "signalr_u_";
        public static readonly string PREFIXGROUP = "signalr_g_";


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
            return _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
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

        /// <summary>
        /// 查询指定用户的连接数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<long> GetConnectsCountByUserAsync(string userId)
        {
            return await GetDatabase().SetLengthAsync($"{PREFIXUSER}{userId}");
        }

        /// <summary>
        /// 获取当前在线的所有用户Id列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetOnlineUsers()
        {
            var keys=GetServer().Keys(_appSetting.Value.RedisCache.DatabaseId, pattern:$"{PREFIXUSER}*",int.MaxValue);
            return keys.Select(m=>m.ToString().Substring(PREFIXUSER.Length));
        }


        /// <summary>
        /// 获取在线的所有组列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetOnlineGroups()
        {
            var keys = GetServer().Keys(_appSetting.Value.RedisCache.DatabaseId, pattern: $"{PREFIXGROUP}*", int.MaxValue);
            return keys.Select(m => m.ToString().Substring(PREFIXGROUP.Length));
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

        /// <summary>
        /// 清空redis缓存中的用户和组
        /// </summary>
        /// <returns></returns>
        public async Task ClearUserAndGroup()
        {
            // await GetServer().FlushDatabaseAsync(_appSetting.Value.RedisCache.DatabaseId);

            var userKeys=GetServer().Keys(_appSetting.Value.RedisCache.DatabaseId, pattern: $"{PREFIXUSER}*", int.MaxValue);
            foreach (var key in userKeys)
            {
                await GetDatabase().KeyDeleteAsync(key);
            }

            var groupKeys= GetServer().Keys(_appSetting.Value.RedisCache.DatabaseId, pattern: $"{PREFIXGROUP}*", int.MaxValue);
            foreach (var key in groupKeys)
            {
                await GetDatabase().KeyDeleteAsync(key);
            }
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
