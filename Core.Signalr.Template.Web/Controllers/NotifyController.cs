using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Signalr.Template.Web.Cores;
using Core.Signalr.Template.Web.Dtos;
using Core.Signalr.Template.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Core.Signalr.Template.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class NotifyController : ControllerBase
    {
        private readonly IHubContext<NotifyHub> _notifyHub;
        private readonly SignalrRedisHelper _redis;

        public NotifyController(IHubContext<NotifyHub> notifyHub, SignalrRedisHelper redis)
        {
            _notifyHub = notifyHub;
            _redis = redis;
        }

        // POST api/notify
        [HttpPost]
        public NotifyData Post(NotifyData values)
        {

            return values;
        }
    }
}
