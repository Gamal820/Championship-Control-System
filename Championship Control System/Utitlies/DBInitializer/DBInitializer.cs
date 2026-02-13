using Championship_Control_System.DataAccess;
using Championship_Control_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Championship_Control_System.Utitlies.DBInitilizer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DBInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(ApplicationDbContext context, ILogger<DBInitializer> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();

                if (_roleManager.Roles.IsNullOrEmpty())
                {
                    _roleManager.CreateAsync(new(SD.SuperAdminRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.TournamentManagerRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.TeamManagerRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.FanRole)).GetAwaiter().GetResult();

                    _userManager.CreateAsync(new()
                    {
                        Email = "superadmin@gmail.com",
                        UserName = "SuperAdmin",
                        EmailConfirmed = true,
                        FirstName = "Super",
                        LastName = "Admin",
                    }, "Gamal123$").GetAwaiter().GetResult();

                    var user = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user!, SD.SuperAdminRole).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }
    }
}