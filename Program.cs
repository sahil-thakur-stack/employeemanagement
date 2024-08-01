using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DbUserConnection");
builder.Services.AddTransient<IDbConnection>(sp =>
               new SqlConnection(connectionString));
var DbEmployeeConnection = builder.Configuration.GetConnectionString("DbEmployeeConnection");

builder.Services.AddTransient<IDbConnection>(sp =>
    new SqlConnection(DbEmployeeConnection));

builder.Services.AddTransient<UserConnection>(sp =>
            new UserConnection(connectionString));
builder.Services.AddTransient<EmployeeConnection>(sp =>
    new EmployeeConnection(DbEmployeeConnection));

// Configure JWT Authentication

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Users/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization(); 
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();
