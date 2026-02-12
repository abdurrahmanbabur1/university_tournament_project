using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityTournamentPro.Data;
using UniversityTournamentPro.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using UniversityTournamentPro.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritaban� Ba�lant�s�
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- YEN�: URL YOLLARINI (PATH) K���K HARF YAPAR (Daha profesyonel g�r�n�r) ---
builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// 2. Identity Yap�land�rmas�
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 3. HTTP Pipeline Yap�land�rmas� (Yolun s�ras� �ok �nemlidir kanka!)
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // --- Yollar� burada hesaplamaya ba�lar ---

app.UseAuthentication(); // --- �nce kim oldu�unu sorar ---
app.UseAuthorization();  // --- Sonra yetkin var m� diye bakar ---

// --- YOLLARI (ROUTES) BURADA TANIMLIYORUZ ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // {id?} k�sm� "Detaylar� G�r" i�in hayati!

app.MapRazorPages();

// --- 4. ROLLER� VE ADM�N� OTOMAT�K OLU�TURMA (G�NCELLEND�) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "Ogrenci" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "zanababur99@gmail.com";
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Zana",
                LastName = "Babur",
                EmailConfirmed = true,
                IsApproved = true,
                Faculty = "M�hendislik",
                Department = "Bilgisayar",
                StudentNo = "000000",
                Gender = "Erkek",
                CreatedDate = DateTime.Now
            };

            var createPowerUser = await userManager.CreateAsync(adminUser, "Admin123!");
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            if (!user.IsApproved) user.IsApproved = true;
            await userManager.UpdateAsync(user);

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Admin olu�turulurken hata!");
    }
}

app.Run();