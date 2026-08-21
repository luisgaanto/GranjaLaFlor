using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Services;
using Microsoft.EntityFrameworkCore;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*
  * Configures Cookie Authentication as the application's authentication scheme.
  * If an unauthenticated user requests a protected resource, ASP.NET Core
  * automatically redirects to the Login page. Logout and Access Denied
  * routes are also configured for future authentication and authorization.
 */
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

        options.SlidingExpiration = true;
    });
/* 
 * use a variable to connect to DB by searching a .json file in this case having the "connectionString" called "granja_la_flor_connection".
 * It is at the appsettings.Development.json file. Recieves the connextion string "granja_la_flor_connection". 
*/
var connectionString = builder.Configuration
    .GetConnectionString("granja_la_flor_connection");

// Creates and registers ApplicationDbContext in the Dependency Injection (DI)
// container using MySQL as the database engine. A new DbContext instance is
// created automatically for each HTTP request.

// Creates and injects  "ApplicationDbContext"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //use MySQL as the database engine. // Automatically detect the MySQL version.
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

/*
 * Registers application Services in the Dependency Injection (DI) container.
 * ASP.NET Core automatically creates one instance of each Service per HTTP
 * request (Scoped lifetime) and injects it whenever a Controller requires it.
*/

builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<BroilerHouseService>();
builder.Services.AddScoped<BroodService>();
builder.Services.AddScoped<IncomeConcentrateService>();
builder.Services.AddScoped<DailyCheckService>();
builder.Services.AddScoped<ExpectedValueService>();
builder.Services.AddScoped<WeeklyCheckService>();
builder.Services.AddScoped<BroodReportService>();
builder.Services.AddScoped<BroodReportPdfService>();

QuestPDF.Settings.License =
    LicenseType.Community;

var app = builder.Build();

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

/*
  * Authentication identifies the current user by reading the authentication
  * cookie and creating the user's identity.
  *
  * Authorization then evaluates whether the authenticated user has permission
  * to access the requested resource.
  *
  * Authentication must always execute before Authorization.
 */
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
