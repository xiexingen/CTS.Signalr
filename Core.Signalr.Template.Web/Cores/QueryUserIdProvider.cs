using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

namespace Core.Signalr.Template.Web.Cores
{
    public class QueryUserIdProvider: IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            var userId=connection.GetHttpContext().Request.Query["userId"].ToString();
            return userId;
        }
    }
}
