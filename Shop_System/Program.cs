using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Shop_System.Extentions;
using ShopSystem.Core.Errors;
using ShopSystem.Core.Models.Account;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Repository.Data.Identity;
using ShopSystem.Repository.Data;

namespace Shop_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region MyRegion
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine("Running in Development");
            }

            #region Configure Services

            builder.Services.AddControllers();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddSwaggerService();
            builder.Services.AddAplictionService();
            builder.Services.AddMemoryCache();

            // Configure the AppIdentityDbContext (for user authentication and roles)
            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnections"))
                              .EnableSensitiveDataLogging()
                              .LogTo(Console.WriteLine)
            );

            // Configure the StoreContext DbContext (for your main application data)
            builder.Services.AddDbContext<StoreContext>((serviceProvider, options) =>
            {
                var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

                // Enable Sensitive Data Logging only in Development
                if (env.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging()  // Enable sensitive data logging in Development
                          .LogTo(Console.WriteLine, LogLevel.Information); // Log SQL queries to console (optional)
                }
                else
                {

                }
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));


            });

            builder.Services.AddAplictionService();
            #endregion

            var app = builder.Build();

            #region Update automatically
            // Create a service scope to resolve services
            using var scope = app.Services.CreateScope();
            var Services = scope.ServiceProvider;

            // Obtain logger factory to create loggers
            var loggerfactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                // Get the database context for Identity
                var identityDbContext = Services.GetRequiredService<AppIdentityDbContext>();

                // Apply database migration asynchronously
                await identityDbContext.Database.MigrateAsync();

                // Get the UserManager service to manage users
                var usermanager = Services.GetRequiredService<UserManager<AppUser>>();

                // Seed initial user data for the Identity context
                //await AppIdentityDbContextSeed.SeedUserAsync(usermanager);
            }
            catch (Exception ex)
            {
                // If an exception occurs during migration or seeding, log the error
                var logger = loggerfactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occurred During Applying The Migrations");
            }
            #endregion

            #region Configure Middlewares

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                        Path.Combine(builder.Environment.ContentRootPath, "Images")),
                RequestPath = "/Resources"
            });
            app.UseCors();

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseMiddleware<ExeptionMiddleWares>();

            // Swagger Configuration
            app.UseSwagger();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares(); // Keep your existing middleware if needed
                app.UseSwaggerUI(); // Basic Swagger UI for development
            }
            else
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty; // This sets Swagger UI at the root URL
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseForwardedHeaders();
            #endregion

            app.Run();
            #endregion
        }
    }
}
