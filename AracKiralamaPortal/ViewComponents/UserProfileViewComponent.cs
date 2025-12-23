using AracKiralamaPortal.Data;
using AracKiralamaPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AracKiralamaPortal.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UserProfileViewComponent(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.GetUserAsync(HttpContext.User);

            return View(user);
        }
    }


}
