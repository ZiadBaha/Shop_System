using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ShopSystem.Core.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Repository.Data.Config
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Table name configuration (optional)
            builder.ToTable("Customers");

            // Property configurations
            builder.Property(c => c.MoneyOwed)
                   .HasColumnType("decimal(18, 2)");

            // Relationships
            builder.HasMany(c => c.Orders)
                   .WithOne(o => o.Customer)
                   .HasForeignKey(o => o.CustomerId);

            builder.HasMany(c => c.Payments)
                   .WithOne(p => p.Customer)
                   .HasForeignKey(p => p.CustomerId);


            builder.HasIndex(c => c.Name)
                   .IsUnique();


            builder.HasIndex(c => c.Phone)
                   .IsUnique();


        }
    }
}
