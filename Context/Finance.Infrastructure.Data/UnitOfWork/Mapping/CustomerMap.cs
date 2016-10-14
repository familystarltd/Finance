namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    using Finance.Domain.Aggregates.CustomerAgg;
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
            b.HasMany(c => c.CustomerDisbursementPayers).WithOne(c => c.Customer).HasForeignKey(c => c.CustomerId);
            b.HasMany(cust => cust.FinancialTransactions).WithOne(c=>c.Customer).IsRequired(false).HasForeignKey(trans => trans.CustomerId);
            b.HasMany(c => c.Fees).WithOne(f => f.Customer).IsRequired(true).HasForeignKey(f => f.CustomerId);
            b.ToTable("Customer");
        }
    }
}
