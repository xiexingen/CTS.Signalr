using Core.Signalr.Template.Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Signalr.Template.Client.Controllers
{
    public class AccountController : Controller
    {
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

            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
    }
}