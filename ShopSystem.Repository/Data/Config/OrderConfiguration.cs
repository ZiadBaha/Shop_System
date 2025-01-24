using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ShopSystem.Core.Models.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace ShopSystem.Repository.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();


            builder.Property(o => o.TotalDiscount)
                .HasColumnType("decimal(18, 2)")
            .IsRequired();


            // Configure relationships
            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders) 
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders) 
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); 

          

        }
    }
}
