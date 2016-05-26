using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using Microsoft.EntityFrameworkCore;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    class InvoiceMap : EntityMappingConfiguration<Invoice>
    {
        public override void Map(EntityTypeBuilder<Invoice> b)
        {
            //b.HasKey(fi => fi.Id);
            b.HasMany(fi => fi.InvoiceDetails).WithOne(fid => fid.Invoice).IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasMany(fi => fi.ReceiptInvoices).WithOne(r => r.Invoice).IsRequired(true).HasForeignKey(r => r.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            b.ToTable("Invoice");
        }
    }
    class FeeInvoiceMap : EntityMappingConfiguration<FeeInvoice>
    {
        public override void Map(EntityTypeBuilder<FeeInvoice> b)
        {
            //b.HasKey(fi => fi.Id);
            b.HasMany(fi => fi.InvoiceDetails).WithOne().IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasMany(fi => fi.ReceiptInvoices).WithOne().IsRequired(true).HasForeignKey(r => r.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            b.ToTable("FeeInvoice");
        }
    }
}