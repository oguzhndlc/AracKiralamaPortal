using AracKiralamaPortal.Models;
using AracKiralamaPortal.ViewModels; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Name = user.Name,
            Surname = user.Surname,
            ProfileURL = user.ProfileURL
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateField([FromBody] ProfileUpdateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        switch (dto.Field)
        {
            case "Name":
                user.Name = dto.Value;
                break;
            case "Surname":
                user.Surname = dto.Value;
                break;
            case "Email":
                user.Email = dto.Value;
                break;
            case "PhoneNumber":
                user.PhoneNumber = dto.Value;
                break;
        }

        await _userManager.UpdateAsync(user);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePhoto(IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
            return Json(new { success = false });

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Json(new { success = false });

        var uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/images/profiles"
        );

        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        if (!string.IsNullOrEmpty(user.ProfileURL) &&
            user.ProfileURL != "default.png")
        {
            var oldFilePath = Path.Combine(uploadFolder, user.ProfileURL);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        var newFileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
        var newFilePath = Path.Combine(uploadFolder, newFileName);

        using (var stream = new FileStream(newFilePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        user.ProfileURL = newFileName;
        await _userManager.UpdateAsync(user);

        var claims = await _userManager.GetClaimsAsync(user);
        var oldClaim = claims.FirstOrDefault(c => c.Type == "ProfileURL");

        if (oldClaim != null)
            await _userManager.RemoveClaimAsync(user, oldClaim);

        await _userManager.AddClaimAsync(
            user,
            new Claim("ProfileURL", "/images/profiles/" + newFileName)
        );

        await _signInManager.RefreshSignInAsync(user);

        return Json(new
        {
            success = true,
            newPath = "/images/profiles/" + newFileName
        });
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
            user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
        else
            user = await _userManager.FindByNameAsync(model.UsernameOrEmail);

        if (user == null)
        {
            ViewBag.Error = "Böyle bir hesap bulunamadı.";
            return View(model);
        }

        // 🔐 Şifre kontrolü
        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            ViewBag.Error = "Şifre hatalı.";
            return View(model);
        }

        // 🧠 CLAIM’LER
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim("ProfileURL", user.ProfileURL ?? "default.png")
    };

        // 🎯 ROLLERİ DE EKLE
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 🍪 COOKIE OLUŞTUR
        await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

        // 🔀 YÖNLENDİRME
        if (roles.Contains("Admin"))
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
        if (!ModelState.IsValid)
            return View(model);

        // Username kontrol
        if (await _userManager.FindByNameAsync(model.Username) != null)
        {
            ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");
            return View(model);
        }

        // Email kontrol
        if (await _userManager.FindByEmailAsync(model.Email) != null)
        {
            ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Name = model.Name,
            Surname = model.Surname,
            ProfileURL = model.ProfileURL
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index","Home");
    }

    [HttpPost]
    public async Task<IActionResult> LogoutAjax()
    {
        await _signInManager.SignOutAsync();
        return Json(new { success = true });
    }

}
