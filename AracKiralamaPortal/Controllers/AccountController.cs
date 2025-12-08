using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AracKiralamaPortal.Models;
using AracKiralamaPortal.ViewModels; 

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
        return View(new LoginViewModel());
    }

    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        ApplicationUser user = null;

        
        if (model.UsernameOrEmail.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
        }


        if (user == null)
        {
            ViewBag.Error = "Böyle bir hesap bulunamadı.";
            return View(model);
        }

        
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        
        if (!result.Succeeded)
        {
            ViewBag.Error = "Şifre hatalı.";
            return View(model);
        }

        
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToAction("Dashboard", "Admin");

        return RedirectToAction("Index", "Home");
    }




    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        Console.WriteLine("Register POST tetiklendi");

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState geçersiz");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            Console.WriteLine("Kullanıcı başarıyla oluşturuldu!");
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            Console.WriteLine("Hata: " + error.Description);
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }


    
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index","Home");
    }
}
