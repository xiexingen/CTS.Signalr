using Core.Signalr.Template.Web.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Hubs
{
    public interface IServerNotifyHub
    {
        Task JoinToGroup(string groupName);
    }

    public interface IClientNotifyHub
    {
        Task OnNotify(object data);

        Task OnJoinGroup(object data);
    }


    [Authorize]
    public class NotifyHub : Hub<IClientNotifyHub>,IServerNotifyHub
    {
        private readonly SignalrRedisHelper _signalrRedisHelper;

        public NotifyHub(SignalrRedisHelper signalrRedisHelper)
        {
            _signalrRedisHelper = signalrRedisHelper;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.OnNotify(new { UserId= Context.User.Identity.Name, Name=Context.User.Identity.Name, ConnectId = Context.ConnectionId });

            var userId= Context.User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _signalrRedisHelper.AddConnectForUserAsync(userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _signalrRedisHelper.RemoveConnectForUser(userId, Context.ConnectionId);
            }            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 加入某个组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).OnJoinGroup(new {ConnectId=Context.ConnectionId,groupName=groupName });

            var userId = Context.User.Identity.Name;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _signalrRedisHelper.AddUserForGroupAsync(groupName, userId);
            }
        }
    }
}
