using AracKiralamaPortal.Data;
using AracKiralamaPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin,Employee")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // 🔒 ADMIN DEĞİLSE ENGELLE
    public override async Task OnActionExecutionAsync(
     ActionExecutingContext context,
     ActionExecutionDelegate next)
    {
        var user = await _userManager.GetUserAsync(context.HttpContext.User);

        if (user == null ||
            !(await _userManager.IsInRoleAsync(user, "Admin") ||
              await _userManager.IsInRoleAsync(user, "Employee")))
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        await next();
    }


    // 📊 DASHBOARD
    public IActionResult Dashboard()
    {
        ViewBag.TotalCars = _context.Cars.Count();
        ViewBag.TotalBrands = _context.Brands.Count();
        ViewBag.TotalUsers = _context.Users.Count();
        return View();
    }

    // 👥 KULLANICILAR
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    public IActionResult Messages()
    {
        return View();
    }

    // 🔁 TEK ROL DEĞİŞTİRME (ADMIN / EMPLOYEE / USER)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı" });
        }

        // 🚫 Admin kendi rolünü düşüremez
        var currentUserId = _userManager.GetUserId(User);
        if (userId == currentUserId && role != "Admin")
        {
            return Json(new
            {
                success = false,
                message = "Kendi admin rolünü değiştiremezsin!"
            });
        }

        // 🔥 TÜM ROLLERİ TEMİZLE
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        // ➕ YENİ ROL ATA (User da dahil)
        await _userManager.AddToRoleAsync(user, role);

        return Json(new
        {
            success = true,
            role = role
        });
    }
}
