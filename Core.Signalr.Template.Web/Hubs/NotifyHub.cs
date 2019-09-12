using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Hubs
{
    public class NotifyHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("notify",new{Name="xxg",ConnectId=Context.ConnectionId });
            await base.OnConnectedAsync();
        }
    }
}
