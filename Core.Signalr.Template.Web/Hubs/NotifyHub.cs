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
            
            var userId = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();
            await Clients.All.OnNotify(new { UserId= userId,Name=Context.User.Identity.Name, ConnectId = Context.ConnectionId });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).OnJoinGroup(new {ConnectId=Context.ConnectionId,groupName=groupName });
        }

        public async Task RemoveFromGroup(string groupName) 
        { 
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,groupName);
        }
    }
}
