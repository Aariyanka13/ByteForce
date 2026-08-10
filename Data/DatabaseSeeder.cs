using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using Microsoft.AspNetCore.Identity;
using SmartRecruitmentMatchingPlatform.Models.Enums;

namespace SmartRecruitmentMatchingPlatform.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher
        )
        {
            if (!await context.Skills.AnyAsync())
            {
                var skills = new List<Skill>
            {
                new()
                {
                    Name = "C#",
                    NormalizedName = "C#"
                },
                new()
                {
                    Name = "ASP.NET Core",
                    NormalizedName = "ASP.NET CORE"
                },
                new()
                {
                    Name = "Java",
                    NormalizedName = "JAVA"
                },
                new()
                {
                    Name = "JavaScript",
                    NormalizedName = "JAVASCRIPT"
                },
                new()
                {
                    Name = "HTML",
                    NormalizedName = "HTML"
                },
                new()
                {
                    Name = "CSS",
                    NormalizedName = "CSS"
                },
                new()
                {
                    Name = "SQL",
                    NormalizedName = "SQL"
                },
                new()
                {
                    Name = "Entity Framework Core",
                    NormalizedName = "ENTITY FRAMEWORK CORE"
                },
                new()
                {
                    Name = "Git",
                    NormalizedName = "GIT"
                },
                new()
                {
                    Name = "REST API",
                    NormalizedName = "REST API"
                }
            };

                await context.Skills.AddRangeAsync(skills);
                await context.SaveChangesAsync();
            }
            var adminEmail = "admin@smartrecruitment.com";
            var normalizedAdminEmail = adminEmail.ToUpperInvariant();

            var adminExists = await context.Users
                .AnyAsync(x => x.NormalizedEmail == normalizedAdminEmail);

            if (!adminExists)
            {
                var adminUser = new User
                {
                    FullName = "System Administrator",
                    Email = adminEmail,
                    NormalizedEmail = normalizedAdminEmail,
                    Role = UserRole.Administrator,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                adminUser.PasswordHash = passwordHasher.HashPassword(
                    adminUser,
                    "Admin123");

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
