using Core.Signalr.Template.Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Client.Controllers
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
    }
}