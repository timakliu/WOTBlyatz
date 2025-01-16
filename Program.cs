using Microsoft.EntityFrameworkCore;
using YWOTBlyatz.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WOTBlyatz.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.AddDebug();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Настройки безопасности
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

////Настройка аутентификации
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = "817459924499-djegsgdigmcmn32ab11j4g2f19sedm74.apps.googleusercontent.com";
//    googleOptions.ClientSecret = "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";
//    googleOptions.CallbackPath = "/signin-google";
//});

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "817459924499-djegsgdigmcmn32ab11j4g2f19sedm74.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
    googleOptions.CallbackPath = "/signin-google";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//app.Use(async (context, next) =>
//{
//    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
//    await next();
//});


app.UseAuthentication(); // ВАЖНО: должно быть перед Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
