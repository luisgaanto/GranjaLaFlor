using DB_GranjaLaFlor.Data.Context;
using DB_GranjaLaFlor.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// use a variable to connect to DB by searching a .json file in this case having the "connectionString" called "granja_la_flor_connection". It is at the appsettings.Development.json file. Recieves the connextion string "granja_la_flor_connection". 
var connectionString = builder.Configuration
    .GetConnectionString("granja_la_flor_connection");

// Creates and injects  "ApplicationDbContext"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //use MySQL as the database engine. // Automatically detect the MySQL version.
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<RoleService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
