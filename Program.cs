using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartRecruitmentMatchingPlatform.Data;

using SmartRecruitmentMatchingPlatform.Interfaces.Repositories;
using SmartRecruitmentMatchingPlatform.Interfaces.Services;

using SmartRecruitmentMatchingPlatform.Interface.Repositories;
using SmartRecruitmentMatchingPlatform.Interface.Services;
using SmartRecruitmentMatchingPlatform.Interface.Storage;

using SmartRecruitmentMatchingPlatform.Middleware;
using SmartRecruitmentMatchingPlatform.Models.Entities;
using SmartRecruitmentMatchingPlatform.Options;
using SmartRecruitmentMatchingPlatform.Repositories;
using SmartRecruitmentMatchingPlatform.Services;

namespace SmartRecruitmentMatchingPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());
                });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Enter JWT token"
                    });

                options.AddSecurityRequirement(
                    new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference =
                                    new Microsoft.OpenApi.Models.OpenApiReference
                                    {
                                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(
                    JwtOptions.SectionName));

            builder.Services.Configure<CvStorageOptions>(
                builder.Configuration.GetSection(
                    CvStorageOptions.SectionName));

            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException(
                    "JWT configuration is missing.");

            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtOptions.Issuer,
                            ValidAudience = jwtOptions.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwtOptions.Key)),

                            ClockSkew = TimeSpan.Zero
                        };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<
                IPasswordHasher<User>,
                PasswordHasher<User>>();

            // Member 1
            builder.Services.AddScoped<
                IUserRepository,
                UserRepository>();

            builder.Services.AddScoped<
                IAuthService,
                AuthService>();

            builder.Services.AddScoped<
                IJwtTokenService,
                JwtTokenService>();

            // Member 3
            builder.Services.AddScoped<
                IEmployerProfileRepository,
                EmployerProfileRepository>();

            builder.Services.AddScoped<
                IEmployerProfileService,
                EmployerProfileService>();

            builder.Services.AddScoped<
                IVacancyRepository,
                VacancyRepository>();

            builder.Services.AddScoped<
                IVacancyService,
                VacancyService>();

            // Member 4
            builder.Services.AddScoped<
                IApplicationRepository,
                ApplicationRepository>();

            // Member 2
            builder.Services.AddScoped<
                IJobSeekerRepository,
                JobSeekerRepository>();

            builder.Services.AddScoped<
                ISkillRepository,
                SkillRepository>();

            builder.Services.AddScoped<
                ICvDocumentRepository,
                CvDocumentRepository>();

            builder.Services.AddScoped<
                IJobSeekerProfileService,
                JobSeekerProfileService>();

            builder.Services.AddScoped<
                ISkillService,
                SkillService>();

            builder.Services.AddScoped<
                ICvService,
                CvService>();

            builder.Services.AddScoped<
                IFileStorageService,
                LocalFileStorageService>();

            // Member 5 - Notifications
            builder.Services.AddScoped<
                INotificationRepository,
                NotificationRepository>();

            builder.Services.AddScoped<
                INotificationService,
                NotificationService>();

            // Member 5 - Admin
            builder.Services.AddScoped<
                IAdminRepository,
                AdminRepository>();

            builder.Services.AddScoped<
                IAdminService,
                AdminService>();

            // Member 5 - Contact Requests
            builder.Services.AddScoped<
                IContactRequestRepository,
                ContactRequestRepository>();

            builder.Services.AddScoped<
                IContactRequestService,
                ContactRequestService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var passwordHasher = scope.ServiceProvider
                    .GetRequiredService<IPasswordHasher<User>>();

                DatabaseSeeder.SeedAsync(
                        dbContext,
                        passwordHasher)
                    .GetAwaiter()
                    .GetResult();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}