using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Signalr.Template.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Core.Signalr.Template.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IHubContext<NotifyHub> _notifyHub;

        public NotifyController(IHubContext<NotifyHub> notifyHub)
        {
            _notifyHub = notifyHub;
        }

        // POST api/notify
        [HttpPost]
        public void Post([FromBody] string value)
        {

            
        }
    }
}
