using Championship_Control_System;
using Championship_Control_System.DataAccess;
using Championship_Control_System.Models;
using Championship_Control_System.Repositories;
using Championship_Control_System.Repositories.IRepositories;
using Championship_Control_System.Utitlies; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using LazZiya.ExpressLocalization;
using System.Globalization; // required for CultureInfo

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.RegisterConfig(connectionString);
builder.Services.AddControllersWithViews()
    .AddExpressLocalization<SharedResource>(options =>
    {
        options.ResourcesPath = "Resources";

        // Configure RequestLocalizationOptions since ExpressLocalizationOptions
        // does not expose SupportedCultures directly.
        options.RequestLocalizationOptions = locOptions =>
        {
            var supported = new[] { "ar-EG", "en-US" }
                .Select(c => new CultureInfo(c))
                .ToList();

            locOptions.SupportedCultures = supported;
            locOptions.SupportedUICultures = supported;

            // set default request culture (optional)
            locOptions.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
        };
    });
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRequestLocalization();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"); 
app.Run();