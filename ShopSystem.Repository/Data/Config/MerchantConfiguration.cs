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
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {

            builder.Property(m => m.OutstandingBalance)
                   .HasColumnType("decimal(18, 2)")
                   .IsRequired();

           
            builder.HasIndex(p=>p.Phone)
                   .IsUnique();




        }
    }
}
