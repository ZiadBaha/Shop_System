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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Composite Key
            builder.HasKey(oi => new { oi.OrderId, oi.ProductId });

            // Relationships
            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Default properties
            builder.Property(oi => oi.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(oi => oi.Discount)
                   .HasColumnType("decimal(18, 2)")  // Specify column type with precision and scale
                   .IsRequired();
        }
    }

}
