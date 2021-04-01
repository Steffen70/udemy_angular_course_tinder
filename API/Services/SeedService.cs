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
using System;

namespace API.Services
{
    public class SeedService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IOptions<RoleConfiguration> _config;
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;
        public SeedService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            IOptions<RoleConfiguration> config, IWebHostEnvironment env, DataContext context)
        {
            _context = context;
            _env = env;
            _config = config;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedData()
        {
            var init = await CreateDatabaseAsync();
            if (init && _env.IsDevelopment())
                await SeedUsersAsync();
        }

        private async Task<bool> CreateDatabaseAsync()
        {
            await _context.Database.MigrateAsync();

            if (await _context.Connections.AnyAsync())
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Connections");

            if (await _userManager.Users.AnyAsync()) return false;

            var roles = _config.Value.Roles.ToList().Select(r => new AppRole { Name = r });

            foreach (var r in roles)
                await _roleManager.CreateAsync(r);

            var admin = new AppUser
            {
                UserName = "admin",
            };

            await _userManager.CreateAsync(admin, Environment.GetEnvironmentVariable("ADMIN_PASSWORD"));
            await _userManager.AddToRolesAsync(admin, _config.Value.AdminRoles);

            return true;
        }

        private async Task SeedUsersAsync()
        {
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