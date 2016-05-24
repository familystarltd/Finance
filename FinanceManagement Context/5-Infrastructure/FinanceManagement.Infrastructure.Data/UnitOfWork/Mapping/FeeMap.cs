using FinanceManagement.Domain.Aggregates.FeeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Infrastructure.Data;
using System;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    /// <summary>
    /// The Fee entity type configuration
    /// </summary>
    class FeeMap : EntityMappingConfiguration<Fee>
    {
        public override void Map(EntityTypeBuilder<Fee> b)
        {
            b.HasKey(f => f.Id);
            b.Property(f => f.Id).IsRequired();
            b.HasMany(f => f.FeeRates).WithOne(fr => fr.Fee).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasOne(f => f.Funder).WithMany(f => f.Fees).HasForeignKey(f => f.FunderId);
        }
    
    }

    #region FEE RATE MAPPING
    /// <summary>
    /// The RateMap entity type configuration
    /// </summary>
    class FeeRateMap : EntityMappingConfiguration<FeeRate>
    {
        public override void Map(EntityTypeBuilder<FeeRate> b)
        {
            b.HasKey(r => r.Id);
            b.HasMany(fr => fr.Rates).WithOne(r => r.FeeRate).IsRequired(true).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasOne(r => r.Fee).WithMany(f => f.FeeRates).IsRequired(true).HasForeignKey(r => r.FeeId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("FeeRate");
        }
    }
    #endregion

    #region RATE MAPPING
    class RateMap : EntityMappingConfiguration<Rate>
    {
        public override void Map(EntityTypeBuilder<Rate> b)
        {
            b.HasKey(r => r.Id);
            b.ToTable("Rate");
        }
    }
    #endregion
}