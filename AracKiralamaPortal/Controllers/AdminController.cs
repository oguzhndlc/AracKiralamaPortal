using AracKiralamaPortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using AracKiralamaPortal.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ADMIN OLMAYANI ENGELLEME
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = await _userManager.GetUserAsync(context.HttpContext.User);

        if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        await next();
    }

    // DASHBOARD
    public IActionResult Dashboard()
    {
        ViewBag.TotalCars = _context.Cars.Count();
        ViewBag.TotalBrands = _context.Brands.Count();
        ViewBag.TotalUsers = _context.Users.Count();

        return View();
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> MakeAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Json(new { success = false });

        if (!await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        return Json(new
        {
            success = true,
            isAdmin = true
        });
    }


    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string userId)
    {
        // Giriş yapan admin
        var currentUserId = _userManager.GetUserId(User);

        // Kendi adminliğini kaldırmayı engelle
        if (userId == currentUserId)
        {
            return Json(new
            {
                success = false,
                message = "Kendi adminliğini kaldıramazsın!"
            });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Json(new { success = false });

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }

        return Json(new
        {
            success = true,
            isAdmin = false
        });
    }
}
