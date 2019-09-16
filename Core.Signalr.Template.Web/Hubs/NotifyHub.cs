using Microsoft.AspNetCore.SignalR;
using System;
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

    public class NotifyHub : Hub<IClientNotifyHub>,IServerNotifyHub
    {
        private readonly IUserIdProvider _userIdProvider;

        public NotifyHub(IUserIdProvider userIdProvider)
        {
            _userIdProvider = userIdProvider;
        }

        //public override async Task OnConnectedAsync()
        //{
        //    var userId=Context.GetHttpContext().Request.Query["userId"].ToString();
        //    await Clients.All.OnNotify(new { Name = userId, ConnectId = Context.ConnectionId });
        //    await base.OnConnectedAsync();
        //}

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
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
