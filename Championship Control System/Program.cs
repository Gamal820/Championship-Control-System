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
using System.Globalization; 
using Stripe;
using Championship_Control_System.Hubs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.RegisterConfig(connectionString);
builder.Services.AddControllersWithViews()
    .AddExpressLocalization<SharedResource>(options =>
    {
        options.ResourcesPath = "Resources";

        
        options.RequestLocalizationOptions = locOptions =>
        {
            var supported = new[] { "ar-EG", "en-US" }
                .Select(c => new CultureInfo(c))
                .ToList();

            locOptions.SupportedCultures = supported;
            locOptions.SupportedUICultures = supported;

            locOptions.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
        };
    });

builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    };
});



//Stripe Settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddSignalR();

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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapHub<MatchHub>("/matchHub");
app.Run();
