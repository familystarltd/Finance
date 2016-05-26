using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    class ReceiptMap : EntityMappingConfiguration<Receipt>
    {
        public override void Map(EntityTypeBuilder<Receipt> b)
        {
            //b.HasKey(fi => fi.Id);
            b.HasMany(r => r.ReceiptInvoices).WithOne(rinv => rinv.Receipt).IsRequired(true).HasForeignKey(rinv => rinv.ReceiptId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            b.HasOne(r => r.CreditNote).WithMany(cr => cr.Receipts).IsRequired(false).HasForeignKey(r => r.CreditNoteId);//.OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("Receipt");
        }
    }
}