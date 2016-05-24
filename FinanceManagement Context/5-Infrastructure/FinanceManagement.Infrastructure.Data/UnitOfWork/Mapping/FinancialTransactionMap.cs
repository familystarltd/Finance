using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    class FinancialTransactionMap : EntityMappingConfiguration<FinancialTransaction>
    {
        public override void Map(EntityTypeBuilder<FinancialTransaction> b)
        {
            b.HasKey(trans => trans.Id);
            b.HasOne(trans => trans.Customer).WithMany(cust => cust.FinancialTransactions).IsRequired(false).HasForeignKey(trans => trans.CustomerId);
            b.ToTable("FinancialTransaction");
        }
    }
}
