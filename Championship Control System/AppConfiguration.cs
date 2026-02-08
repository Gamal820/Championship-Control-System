using Championship_Control_System.DataAccess;
using Championship_Control_System.Models;
using Championship_Control_System.Repositories;
using Championship_Control_System.Repositories.IRepositories;
using Championship_Control_System.Utitlies;
using Championship_Control_System.Utitlies.DBInitilizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace Championship_Control_System
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services, string connection)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                option.UseSqlServer(connection);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequiredLength = 8;
                option.Password.RequireNonAlphanumeric = false;
                option.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Default login path
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Default access denied path
            });

            services.AddTransient<IEmailSender, EmailSender>();
     
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            services.AddScoped<IRepository<Stadium>, Repository<Stadium>>();
            services.AddScoped<IRepository<Team>, Repository<Team>>();
            services.AddScoped<IRepository<Coach>, Repository<Coach>>();
            services.AddScoped<IRepository<Championship>, Repository<Championship>>();


            services.AddScoped<IDBInitializer, DBInitializer>();
        }
    }
}