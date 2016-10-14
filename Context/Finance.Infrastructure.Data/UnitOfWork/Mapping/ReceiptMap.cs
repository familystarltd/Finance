using Finance.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    //class ReceiptInvoiceMap : EntityMappingConfiguration<ReceiptInvoice>
    //{
    //    public override void Map(EntityTypeBuilder<ReceiptInvoice> rInv)
    //    {
    //        rInv.HasOne(ri => ri.Invoice).WithMany(inv => inv.ReceiptInvoices).HasForeignKey(ri => ri.InvoiceId);
    //        rInv.HasOne(ri => ri.Receipt).WithMany(r => r.ReceiptInvoices).HasForeignKey(ri => ri.ReceiptId);
    //        rInv.ToTable("ReceiptInvoices");
    //    }
    //}
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