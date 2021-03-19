using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if(await context.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            users.ForEach(u => {
                using (var hmac = new HMACSHA512())
                {
                    u.UserName = u.UserName.ToLower();
                    u.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("demo"));
                    u.PasswordSalt = hmac.Key;

                    context.Users.Add(u);
                }
            });

            await context.SaveChangesAsync();
        }
    }
}