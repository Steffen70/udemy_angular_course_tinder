using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using API.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using API.Data;

namespace API.Services
{
    public class SeedService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;
        public SeedService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            IOptions<ApplicationSettings> config, IWebHostEnvironment env, DataContext context)
        {
            _context = context;
            _env = env;
            _config = config;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task CreateDatabaseAsync()
        {
            await _context.Database.MigrateAsync();

            if (await _userManager.Users.AnyAsync()) return;

            var roles = _config.Value.Roles.ToList().Select(r => new AppRole { Name = r });

            foreach (var r in roles)
                await _roleManager.CreateAsync(r);

            var admin = new AppUser
            {
                UserName = "admin",
            };

            await _userManager.CreateAsync(admin, _config.Value.AdminPassword);
            await _userManager.AddToRolesAsync(admin, _config.Value.AdminRoles);
        }

        public async Task SeedUsersIfDevelopmentAsync()
        {
            if (!_env.IsDevelopment()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if (users == null) return;

            foreach (var u in users)
            {
                u.UserName = u.UserName.ToLower();
                await _userManager.CreateAsync(u, "demo");
                await _userManager.AddToRoleAsync(u, _config.Value.MemberRole);
            }
        }
    }
}