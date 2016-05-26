using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    class CreditNoteMap : EntityMappingConfiguration<CreditNote>
    {
        public override void Map(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CreditNote> e)
        {
            //e.HasKey(cr => cr.Id);
            e.HasIndex(cr => cr.CreditNoteNo).IsUnique(true);
            e.HasOne(cr => cr.Invoice).WithMany(inv => inv.CreditNotes).IsRequired(false).HasForeignKey(cr => cr.InvoiceId);
            e.HasOne(cr => cr.Receipt).WithMany(r => r.CreditNotes).IsRequired(false).HasForeignKey(cr => cr.ReceiptId);//.OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            e.HasMany(cr => cr.Receipts).WithOne(r => r.CreditNote).IsRequired(false).HasForeignKey(r => r.CreditNoteId);//.OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            e.ToTable("CreditNote");
        }
    }
}