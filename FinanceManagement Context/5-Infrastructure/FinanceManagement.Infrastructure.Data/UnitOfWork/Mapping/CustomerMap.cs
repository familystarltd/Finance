﻿namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    using FinanceManagement.Domain.Aggregates.CustomerAgg;
    using System.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;

    /// <summary>
    /// The Customer entity type configuration
    /// </summary>
    class CustomerMap : EntityMappingConfiguration<Customer>
    {
        public override void Map(EntityTypeBuilder<Customer> b)
        {
            //configure keys and properties
            b.HasKey(c => c.Id);
            b.HasMany(c => c.CustomerDisbursementFunders).WithOne(c => c.Customer).HasForeignKey(c => c.CustomerId);
            b.HasMany(c => c.Fees).WithOne(f => f.Customer).IsRequired(true).HasForeignKey(f => f.CustomerId);
            b.ToTable("Customer");
        }
    }
}
