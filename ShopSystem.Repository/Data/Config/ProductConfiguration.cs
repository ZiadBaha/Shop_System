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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Unique Index
            builder.HasIndex(p => p.UniqueNumber)
                .IsUnique()
                .HasDatabaseName("IX_Product_UniqueNumber");

            // Relationships
            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.PurchasePrice)
            .HasColumnType("decimal(18, 2)")  // or .HasPrecision(18, 2)
            .IsRequired();

            builder.Property(p => p.SellingPrice)
           .HasColumnType("decimal(18, 2)")  // Or use .HasPrecision(18, 2)
           .IsRequired();
        }
    }
}
