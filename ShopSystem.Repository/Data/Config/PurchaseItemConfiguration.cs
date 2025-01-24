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
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {


            builder.Property(pi => pi.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.Quantity)
                .IsRequired();

            builder.Property(pi => pi.PricePerUnit)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(pi => pi.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }

}
