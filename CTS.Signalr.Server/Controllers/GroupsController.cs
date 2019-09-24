using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CTS.Signalr.Server.Cores;
using CTS.Signalr.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CTS.Signalr.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly SignalrRedisHelper _redis;

        public GroupsController(SignalrRedisHelper redis)
        {
            _redis = redis;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await Task.Run(() =>
            {
                return _redis.GetOnlineGroups();
            });
        }
    }
}
