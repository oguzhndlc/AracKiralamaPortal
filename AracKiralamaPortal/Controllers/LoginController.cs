using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AracKiralamaPortal.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _adminUser = "admin";
        private readonly string _adminPass = "1234";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (username == _adminUser && password == _adminPass)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, "AdminCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("AdminCookie", principal);

                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre yanlış!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            return RedirectToAction("Index");
        }
    }
}
