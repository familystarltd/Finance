using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Microsoft.EntityFrameworkCore;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    class InvoiceDetailMap : EntityMappingConfiguration<InvoiceDetail>
    {
        public override void Map(EntityTypeBuilder<InvoiceDetail> b)
        {
            b.HasKey(fi => fi.Id);
            b.HasDiscriminator<string>("Discriminator")
                .HasValue<FeeInvoiceDetail>("FeeInvoiceDetail")
                .HasValue<FNCInvoiceDetail>("FNCInvoiceDetail");
            b.ToTable("InvoiceDetail");
        }
    }
    class InvoiceMap : EntityMappingConfiguration<Invoice>
    {
        public override void Map(EntityTypeBuilder<Invoice> b)
        {
            //b.HasKey(fi => fi.Id);
            b.HasMany(fi => fi.InvoiceDetails).WithOne(fid => fid.Invoice).IsRequired(true).HasForeignKey(fid => fid.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.HasMany(fi => fi.ReceiptInvoices).WithOne(r => r.Invoice).IsRequired(true).HasForeignKey(r => r.InvoiceId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("Invoice");
        }
    }
}