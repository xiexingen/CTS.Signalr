using CTS.Signalr.Client.Dtos;
using CTS.Signalr.Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
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
                GroupIds = groups,
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
                GroupIds = input.SelectGroups,
                UserIds=input.SelectUsers,
                NotifyObj = new
                {
                    TenantType = "chat",
                    MethodType = "message",
                    Message=$"{User.Identity.Name}:{input.Message}"
                }
            });
        }

        /// <summary>
        /// 触发文件下载
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task TriggerFileDownLoad([FromForm]SendNotifyConnectsInput input)
        {
            // 请无视代码问题，模拟后台打包，三秒后通知
            Task.Run(() =>
            {
               Thread.Sleep(3000);
                _signalrHelper.PushNotifyToConnectsAsync(new Dtos.SendToConnects()
                {
                    ConnectionId = input.ConnectionId,
                    ExcludeConnectId=input.ExcludeConnectId,
                    UserId=input.UserId,
                    NotifyObj = new
                    {
                        TenantType = "fileDownload",
                        Content = string.IsNullOrEmpty(input.Content)?$"文件打包完成，下载地址为http://blogs.xxgtalk.cn": input.Content
                    }
                });
            });
            await Task.CompletedTask;
        }
    }
}