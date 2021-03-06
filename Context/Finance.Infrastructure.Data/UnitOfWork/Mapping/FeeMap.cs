﻿using Finance.Domain.Aggregates.FeeAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Infrastructure.Data;
using System;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
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
            b.HasOne(f => f.Payer).WithMany(f => f.Fees).HasForeignKey(f => f.PayerId);
            b.ToTable("Fee");
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
    class RateMap : EntityMappingConfiguration<Rate>
    {
        public override void Map(EntityTypeBuilder<Rate> rate)
        {
            rate.HasKey(r => r.Id);
            rate.HasDiscriminator<string>("Discriminator").HasValue<WeeklyRate>("WeeklyRate");
            //.HasValue<DailyRate>("daily_rate").HasValue<HourlyRate>("hourly_rate").HasValue<MonthlyRate>("monthly_rate");
            rate.ToTable("Rates");
        }
    }
    #endregion

    //#region RATE MAPPING
    //class RateMap : EntityMappingConfiguration<Rate>
    //{
    //    public override void Map(EntityTypeBuilder<Rate> b)
    //    {
    //        b.HasKey(r => r.Id);
    //        b.ToTable("Rate");
    //    }
    //}
    //class WeeklyRateMap : EntityMappingConfiguration<WeeklyRate>
    //{
    //    public override void Map(EntityTypeBuilder<WeeklyRate> b)
    //    {
    //        //b.HasKey(r => r.Id);
    //        b.ToTable("Rate");
    //    }
    //}
    //class DailyRateMap : EntityMappingConfiguration<DailyRate>
    //{
    //    public override void Map(EntityTypeBuilder<DailyRate> b)
    //    {
    //        //b.HasKey(r => r.Id);
    //        b.ToTable("Rate");
    //    }
    //}
    //class MonthlyRateMap : EntityMappingConfiguration<MonthlyRate>
    //{
    //    public override void Map(EntityTypeBuilder<MonthlyRate> b)
    //    {
    //        //b.HasKey(r => r.Id);
    //        b.ToTable("Rate");
    //    }
    //}
    //class HourlyRateMap : EntityMappingConfiguration<HourlyRate>
    //{
    //    public override void Map(EntityTypeBuilder<HourlyRate> b)
    //    {
    //        //b.HasKey(r => r.Id);
    //        b.ToTable("Rate");
    //    }
    //}
    //#endregion
}