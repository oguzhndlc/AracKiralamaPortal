using AracKiralamaPortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

    // Admin olmayanı anasayfaya at
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


    public IActionResult Dashboard()
    {
        ViewBag.TotalCars = _context.Cars.Count();
        ViewBag.TotalBrands = _context.Brands.Count();
        ViewBag.TotalUsers = _context.Users.Count();

        return View();
    }
}
