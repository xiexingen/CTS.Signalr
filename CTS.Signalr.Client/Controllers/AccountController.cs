using CTS.Signalr.Client.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CTS.Signalr.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignalrHelper _signalrHelper;

        public AccountController(SignalrHelper signalrHelper)
        {
            _signalrHelper = signalrHelper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new Exception("用户名不能为空");
            }

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name,userName),
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

            await _signalrHelper.PushNotifyAsync(new Dtos.Send{ 
                UserIds=userName,
                NotifyObj=new
                {
                    TenantType = "logout",
                    MethodType = "login"
                }
            });

            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
    }
}