using AracKiralamaPortal.Data;
using AracKiralamaPortal.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AracKiralamaPortal.Models;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

// 🌍 KÜLTÜR AYARI (TR)
var culture = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;


// JSON Site Settings
builder.Services.AddSingleton<SiteSettingsService>();

// MVC
builder.Services.AddControllersWithViews();

// DB CONTEXT
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPOSITORY
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Home/Index";
});

var app = builder.Build();



async Task SeedRoles(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));
}

async Task SeedAdmin(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string adminUsername = "admin";
    string adminEmail = "admin@site.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminUsername,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}




using (var scope = app.Services.CreateScope())
{
    await SeedRoles(scope.ServiceProvider);
    await SeedAdmin(scope.ServiceProvider);
}




// ERROR HANDLER
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity middleware
app.UseAuthentication();
app.UseAuthorization();

// ROUTING
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();