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
            b.HasKey(fi => fi.Id);
            b.HasMany(fi => fi.InvoiceDetails).WithOne(fid => fid.Invoice).IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasMany(fi => fi.ReceiptInvoices).WithOne(r => r.Invoice).IsRequired(true).HasForeignKey(r => r.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("Invoice");
        }
    }
    class InvoiceDetailsMap : EntityMappingConfiguration<InvoiceDetail>
    {
        public override void Map(EntityTypeBuilder<InvoiceDetail> b)
        {
            b.HasKey(fi => fi.Id);
            b.HasOne(fi => fi.Invoice)
                .WithMany(fi => fi.InvoiceDetails).IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("InvoiceDetail");
        }
    }

    class FeeInvoiceMap : EntityMappingConfiguration<FeeInvoice>
    {
        public override void Map(EntityTypeBuilder<FeeInvoice> b)
        {
            b.HasKey(fi => fi.Id);
            b.HasMany(fi => fi.InvoiceDetails).WithOne().IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasMany(fi => fi.ReceiptInvoices).WithOne().IsRequired(true).HasForeignKey(r => r.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("FeeInvoice");
        }
    }
    class FeeInvoiceDetailsMap : EntityMappingConfiguration<FeeInvoiceDetail>
    {
        public override void Map(EntityTypeBuilder<FeeInvoiceDetail> b)
        {
            b.HasKey(fi => fi.Id);
            b.HasOne(fi => fi.Invoice).WithMany().IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("FeeInvoiceDetail");
        }
    }
}