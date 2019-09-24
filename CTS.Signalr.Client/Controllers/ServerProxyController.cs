using CTS.Signalr.Client.Dtos;
using CTS.Signalr.Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CTS.Signalr.Client.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServerProxyController : Controller
    {
        private readonly SignalrHelper _signalrHelper;

        public ServerProxyController(SignalrHelper signalrHelper)
        {
            _signalrHelper = signalrHelper;
        }

        [HttpPost]
        public async Task AssignTaskToUser([FromForm]string groups)
        {
            await _signalrHelper.PushNotifyAsync(new Dtos.Send()
            {
                ExcludeUsers=true,
                GroupId = groups,
                NotifyObj = new
                {
                    TenantType = "project",
                    MethodType = "tipCount"
                }
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendMessage([FromForm]SendMessageInput input) 
        {
            await _signalrHelper.PushNotifyAsync(new Dtos.Send()
            {
                GroupId = input.SelectGroups,
                UserIds=input.SelectUsers,
                NotifyObj = new
                {
                    TenantType = "chat",
                    MethodType = "message",
                    Message=$"{User.Identity.Name}:{input.Message}"
                }
            });
        }
    }
}