using AracKiralamaPortal.Data;
using AracKiralamaPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace AracKiralamaPortal.Controllers
{

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

        public IActionResult Messages()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }

            var currentUserId = _userManager.GetUserId(User);
            if (userId == currentUserId && role != "Admin")
            {
                return Json(new
                {
                    success = false,
                    message = "Kendi admin rolünü değiştiremezsin!"
                });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, role);

            return Json(new
            {
                success = true,
                role = role
            });
        }
        public IActionResult Reservations()
        {
            var reservations = _context.Reservations.ToList();
            return View(reservations);
        }

        public IActionResult Approve(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation != null)
            {
                reservation.Status = "Onaylandı";
                _context.SaveChanges();
            }
            return RedirectToAction("Reservations");
        }

        public IActionResult Reject(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation != null)
            {
                reservation.Status = "Reddedildi";
                _context.SaveChanges();
            }
            return RedirectToAction("Reservations");
        }
    }
}