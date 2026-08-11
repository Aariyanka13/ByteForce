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
                var skillNames = new[]
                {
                    "C#", "ASP.NET Core", "Java", "JavaScript", "HTML",
                    "CSS", "SQL", "Entity Framework Core", "Git", "REST API"
                };

                var skills = skillNames.Select(name => new Skill
                {
                    Name = name,
                    NormalizedName = name.ToUpperInvariant()
                }).ToList();

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
