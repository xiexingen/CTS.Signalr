using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Hubs
{
    public class DemoHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("notify",new{Name= "demo-xxg", ConnectId=Context.ConnectionId });
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
