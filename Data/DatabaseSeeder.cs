using Microsoft.EntityFrameworkCore;
using SmartRecruitmentMatchingPlatform.Models.Entities;

namespace SmartRecruitmentMatchingPlatform.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(
        ApplicationDbContext context)
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
        }
    }
}
