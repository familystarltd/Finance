using Finance.Domain.Aggregates.FeeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Infrastructure.Data;
using System;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    /// <summary>
    /// The FNC entity type configuration
    /// </summary>
    class FNCMap : EntityMappingConfiguration<FNC>
    {
        public override void Map(EntityTypeBuilder<FNC> b)
        {
            b.HasKey(f => f.Id);
            b.Property(f => f.Id).IsRequired();
            b.HasMany(f => f.FNCCustomers).WithOne(fr => fr.FNC).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("FNC");
        }
    }
    class FNCCustomerMap : EntityMappingConfiguration<FNCCustomer>
    {
        public override void Map(EntityTypeBuilder<FNCCustomer> b)
        {
            b.HasKey(r => r.Id);
            b.HasMany(fr => fr.FNCRates).WithOne(r => r.FNCCustomer).IsRequired(true).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasOne(r => r.FNC).WithMany(f => f.FNCCustomers).IsRequired(true).HasForeignKey(r => r.FNCId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("FNCCustomer");
        }
    }
    class FNCRateMap : EntityMappingConfiguration<FNCRate>
    {
        public override void Map(EntityTypeBuilder<FNCRate> rate)
        {
            rate.HasKey(r => r.Id);
            rate.ToTable("FNCRate");
        }
    }
}