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
    public IActionResult Index(SiteSettings model)
    {
        _settingsService.SaveSettings(model);
        ViewBag.Message = "Ayarlar başarıyla kaydedildi!";
        return View(model);
    }
}
