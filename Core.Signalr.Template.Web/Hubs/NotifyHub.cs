using Core.Signalr.Template.Web.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Hubs
{
    /// <summary>
    /// 服务端接口
    /// </summary>
    public interface IServerNotifyHub
    {

    }

    /// <summary>
    /// 客户端使用的接口
    /// </summary>
    public interface IClientNotifyHub
    {
        Task OnNotify(object data);

        //Task OnJoinGroup(object data);

        //Task OnLeaveGroup(object data);
    }


    [Authorize]
    public class NotifyHub : Hub<IClientNotifyHub>,IServerNotifyHub
    {
        private readonly SignalrRedisHelper _signalrRedisHelper;
        private readonly ILogger _logger;

        public NotifyHub(SignalrRedisHelper signalrRedisHelper, ILogger<NotifyHub> logger)
        {
            _signalrRedisHelper = signalrRedisHelper;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            //await Clients.All.OnNotify(new { UserId= Context.User.Identity.Name, Name=Context.User.Identity.Name, ConnectId = Context.ConnectionId });

            var userId= Context.User.Identity.Name;
            var groups=Context.GetHttpContext().Request.Query["group"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _signalrRedisHelper.AddConnectForUserAsync(userId, Context.ConnectionId);
                await JoinToGroup(userId, Context.ConnectionId, groups?.Split(','));
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Identity.Name;
            var groups = Context.GetHttpContext().Request.Query["group"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _signalrRedisHelper.RemoveConnectForUserAsync(userId, Context.ConnectionId);
            }
            await LeaveFromGroup(Context.ConnectionId, groups?.Split(','));
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 加入组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private async Task JoinToGroup(string userId,string connectionId,params string[] groups)
        {
            if (!string.IsNullOrWhiteSpace(userId)&& groups!=null&&groups.Length>0)
            {
                foreach (var group in groups)
                {
                    await Groups.AddToGroupAsync(connectionId, group);
                    await _signalrRedisHelper.AddUserForGroupAsync(group, connectionId, userId);

                    // await Clients.Group(group).OnJoinGroup(new { ConnectId = connectionId, UserId = userId, GroupName = group });
                }
            }
        }

        /// <summary>
        /// 从组中移除
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private async Task LeaveFromGroup(string connectionId,params string[] groups)
        {
            if (groups != null && groups.Length > 0)
            {
                foreach (var group in groups)
                {
                    await Groups.RemoveFromGroupAsync(connectionId, group);
                    await _signalrRedisHelper.RemoveConnectFromGroupAsync(group,connectionId);
                    // await Clients.Group(group).OnLeaveGroup(new { ConnectId = connectionId, GroupName = group });
                }
            }
        }
    }
}
