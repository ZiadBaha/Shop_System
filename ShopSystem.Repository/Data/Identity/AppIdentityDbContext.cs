using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopSystem.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Repository.Data.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }

        // This method is called when the model is being created. 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure the AppUser entity maps to the correct table
            modelBuilder.Entity<AppUser>().ToTable("AspNetUsers"); // Default table for ASP.NET Identity

            // Seed roles data
            SeedRoles(modelBuilder);

            // Apply configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "User", ConcurrencyStamp = "1", NormalizedName = "USER" },
                new IdentityRole { Name = "BussinesOwner", ConcurrencyStamp = "2", NormalizedName = "BUSSINESOWNER" },
                new IdentityRole { Name = "ServiceProvider", ConcurrencyStamp = "3", NormalizedName = "SERVICEPROVIDER" },
                new IdentityRole { Name = "Admin", ConcurrencyStamp = "4", NormalizedName = "ADMIN" }
            );
        }


    }
}
