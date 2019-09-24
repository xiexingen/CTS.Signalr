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
    public class UsersController : ControllerBase
    {
        readonly IOptionsSnapshot<AppSetting> _appSetting;
        private readonly SignalrRedisHelper _redis;

        public UsersController(IOptionsSnapshot<AppSetting> appSetting, SignalrRedisHelper redis)
        {
            _appSetting = appSetting;
            _redis= redis;
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return await Task.Run(() =>
            {
                return _redis.GetOnlineUsers();
            });
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        //[HttpPost]
        //public IActionResult Login()
        //{
        //    var claims = new Claim[] {
        //        new Claim(ClaimTypes.Name,"xxg"),
        //        new Claim(ClaimTypes.Role,"admin")
        //    };
        //    var jwtSetting= _appSetting.Value.JwtSetting;
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        jwtSetting.Issuer,
        //        jwtSetting.Audience,
        //        claims,
        //        DateTime.Now,
        //        DateTime.Now.AddMinutes(10),
        //        creds
        //    );

        //    return Ok(new
        //    {
        //        token = new JwtSecurityTokenHandler().WriteToken(token)
        //    });
        //}
    }
}
