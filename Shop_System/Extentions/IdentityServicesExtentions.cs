using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Core.Services;
using ShopSystem.Repository.Data.Identity;
using ShopSystem.Repository.Reposatories;
using ShopSystem.Service;
using System.Text;

namespace Shop_System.Extentions
{
    public static class IdentityServicesExtentions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add identity services with specified user and role classes
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders()
            .AddRoles<IdentityRole>();

            // Add authentication services
            services.AddAuthentication(Options =>
            {
                // Set default authentication scheme to JWT bearer authentication
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Configure JWT bearer authentication
            .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]))
                };
            });

            services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(configuration.GetConnectionString("IdentityConnections"));
            });

            // Register custom token service
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAdminServices, AdminService>();







            return services;
        }



    }
}
