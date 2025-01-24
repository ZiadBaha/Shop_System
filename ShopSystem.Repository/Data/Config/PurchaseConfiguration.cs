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
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {

            // Configure properties
            builder.Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)") // Specify precision for decimal
                .IsRequired(false); // Optional as TotalAmount can be calculated

            // Configure relationships
            builder.HasMany(p => p.PurchaseItems)
                .WithOne(p=>p.Purchase)
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete to remove associated items

            builder.HasOne(p => p.Merchant)  // Each purchase has one merchant
                   .WithMany(m => m.Purchases)  // A merchant can have many purchases
                   .HasForeignKey(p => p.MerchantId) // Foreign key in Purchase
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
