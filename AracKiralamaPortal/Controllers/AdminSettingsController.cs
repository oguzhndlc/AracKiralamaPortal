using AracKiralamaPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminSettingsController: Controller
{
    private readonly SiteSettingsService _settingsService;

    public AdminSettingsController(SiteSettingsService settingsService, UserManager<ApplicationUser> userManager)
    {
        _settingsService = settingsService;
    }

    public IActionResult Index()
    {
        var settings = _settingsService.GetSettings();
        return View(settings);
    }

    [HttpPost]
    public IActionResult Index(SiteSettings model, IFormFile siteLogoFile)
    {
        string defaultLogo = "default_logo.png"; 
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/site");

        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);
        if (siteLogoFile != null && siteLogoFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(model.siteLogo) && model.siteLogo != defaultLogo)
            {
                var oldPath = Path.Combine(uploadFolder, model.siteLogo);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(siteLogoFile.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                siteLogoFile.CopyTo(stream);
            }

            model.siteLogo = fileName;
        }

        _settingsService.SaveSettings(model);
        ViewBag.Message = "Ayarlar başarıyla kaydedildi!";
        return View(model);
    }


}
