using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Web.Hubs
{
    public interface IServerNotifyHub
    {
        Task SendMessage(string userName,string message);
    }

    public interface IClientNotifyHub
    {
        Task OnNotify(object data);

        Task OnReceiveMessage(string userName,string message);
    }

    public class NotifyHub : Hub<IClientNotifyHub>,IServerNotifyHub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.OnNotify(new { Name = "xxg", ConnectId = Context.ConnectionId });
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string userName, string message)
        {
            await Clients.All.OnReceiveMessage(userName, message);
        }
    }
}
