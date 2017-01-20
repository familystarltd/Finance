using Finance.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    class FinancialTransactionMap : EntityMappingConfiguration<FinancialTransaction>
    {
        public override void Map(EntityTypeBuilder<FinancialTransaction> b)
        {
            b.HasKey(trans => trans.Id);
            b.HasDiscriminator<string>("Discriminator")
                .HasValue<FeeInvoice>("Fee")
                .HasValue<FNCInvoice>("FNC")
                .HasValue<CreditNote>("Credit Note")
                .HasValue<BadDebt>("BadDebt");
            b.ToTable("FinancialTransactions");
        }
    }
}
