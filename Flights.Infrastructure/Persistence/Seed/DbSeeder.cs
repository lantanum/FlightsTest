using BCrypt.Net;
using Flights.Domain.Users;
using Microsoft.EntityFrameworkCore;


namespace Flights.Infrastructure.Persistence.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db, CancellationToken ct)
        {
            await db.Database.MigrateAsync(ct);
            if (!await db.Roles.AnyAsync(ct))
            {
                var userRole = new Role { Code = "User" };
                var modRole = new Role { Code = "Moderator" };
                db.Roles.AddRange(userRole, modRole);
                await db.SaveChangesAsync(ct);


                db.Users.Add(new User
                {
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    RoleId = modRole.Id
                });
                db.Users.Add(new User
                {
                    Username = "user",
                    Password = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    RoleId = userRole.Id
                });
                await db.SaveChangesAsync(ct);
            }
        }
    }
}