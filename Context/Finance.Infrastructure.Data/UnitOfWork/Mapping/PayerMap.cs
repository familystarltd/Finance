namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    using Finance.Domain.Aggregates.FeeAgg;
    using System.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;

    /// <summary>
    /// The Payer entity type configuration
    /// </summary>
    class PayerMap : EntityMappingConfiguration<Payer>
    {
        public override void Map(EntityTypeBuilder<Payer> b)
        {
            //configure keys and properties
            b.HasKey(s => s.Id);
            b.ToTable("Payer");
        }
    }
}