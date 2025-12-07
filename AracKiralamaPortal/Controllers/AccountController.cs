using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AracKiralamaPortal.Models;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            ViewBag.Error = "Kullanıcı bulunamadı.";
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (result.Succeeded)
            return RedirectToAction("Dashboard", "Admin");

        ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
        return View();
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
