using CTS.Signalr.Server.Cores;
using CTS.Signalr.Server.Dtos;
using CTS.Signalr.Server.Hubs;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTS.Signalr.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class NotifyController : ControllerBase
    {
        private readonly IHubContext<NotifyHub,IClientNotifyHub> _notifyHub;
        private readonly SignalrRedisHelper _redis;
        private readonly ILogger _logger;

        public NotifyController(IHubContext<NotifyHub,IClientNotifyHub> notifyHub, SignalrRedisHelper redis,ILogger<NotifyController> logger)
        {
            _notifyHub = notifyHub;
            _redis = redis;
            _logger=logger;
        }

        // POST api/notify
        [HttpPost]
        public async Task Post(NotifyData input)
        {
            _logger.LogDebug($"post:{JsonConvert.SerializeObject(input)}");
            var hasGroups= !string.IsNullOrWhiteSpace(input.GroupIds);

            // input.NotifyObj= MessagePackSerializer.Deserialize<NotifyData>(MessagePackSerializer.SerializeUnsafe(input));
            input.NotifyObj = JsonConvert.SerializeObject(input.NotifyObj);
            if (hasGroups)
            {
                await NotifyWithGroup(input);
            }
            else
            {
                await NotifyWithOutGroup(input);
            }
        }

        /// <summary>
        /// 推送给指定连接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostConnects(NotifyConnectsData input) 
        {
            _logger.LogDebug($"post:{JsonConvert.SerializeObject(input)}");
            List<string> connectionIds= null;
            if (!string.IsNullOrWhiteSpace(input.UserId)&&input.ExcludeConnectId)
            {
                var userConnections = await _redis.GetConnectsByUserAsync(input.UserId);
                connectionIds = userConnections.Where(m=>!m.Equals(input.ConnectionId)).ToList();
            }
            else
            {
                connectionIds = new List<string>()
                {
                    input.ConnectionId
                };
            }

            input.NotifyObj = JsonConvert.SerializeObject(input.NotifyObj);
            await _notifyHub.Clients.Clients(connectionIds).OnNotify(input.NotifyObj);
        }


        private async Task NotifyWithGroup(NotifyData input) 
        {
            // 组连接信息(连接Id、用户Id)
            var groupUsers = new List<UserConnection>();
            // 指定的用户连接Dictionary列表
            // var dictUserConnections = await GetUserConnectDict(input.UserIds);
            var users=input.UserIds.Split(',');
            var groups = input.GroupIds.Split(',');
            foreach (var group in groups)
            {
                var groupUser = await _redis.GetUsersByGroupAsync(group);
                groupUsers.AddRange(groupUser);
            }
            

            // 要通知的连接Id列表(排除||包含 组中指定用户进行推送)
            var notifyConnections= groupUsers
                                    .Where(m => users.Contains(m.UserId)!= input.ExcludeUsers)
                                    .Select(m => m.ConnectionId)
                                    .ToList();            
            await _notifyHub.Clients.Clients(notifyConnections).OnNotify(input.NotifyObj);
        }

        private async Task NotifyWithOutGroup(NotifyData input)
        {
            // 指定的用户连接Dictionary列表
            var dictUserConnections = await GetUserConnectDict(input.UserIds);
            var userConnects = dictUserConnections.SelectMany(m => m.Value).ToList();
            if (input.ExcludeUsers)
            {
                await _notifyHub.Clients.AllExcept(userConnects).OnNotify(input.NotifyObj);
            }
            else
            {
                await _notifyHub.Clients.Clients(userConnects).OnNotify(input.NotifyObj);
            }
        }

        /// <summary>
        /// 根据用户Id集合获取用户-连接字典(一个用户可能多个连接)
        /// </summary>
        /// <param name="strUserIds"></param>
        /// <returns></returns>
        private async Task<Dictionary<string,List<string>>> GetUserConnectDict(string strUserIds) 
        {
            var dictUserConnections = new Dictionary<string, List<string>>();
            if (!string.IsNullOrWhiteSpace(strUserIds))
            {
                var userIds = strUserIds.Split(',');
                foreach (var userId in userIds)
                {
                    var userConnections = await _redis.GetConnectsByUserAsync(userId);
                    dictUserConnections.Add(userId, userConnections);
                }
            }
            return dictUserConnections;
        }
    }
}
