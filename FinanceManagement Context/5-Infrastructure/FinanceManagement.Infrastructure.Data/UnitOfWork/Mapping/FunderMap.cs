namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    using FinanceManagement.Domain.Aggregates.FeeAgg;
    using System.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;

    /// <summary>
    /// The Funder entity type configuration
    /// </summary>
    class FunderMap : EntityMappingConfiguration<Funder>
    {
        public override void Map(EntityTypeBuilder<Funder> b)
        {
            //configure keys and properties
            b.HasKey(s => s.Id);
            b.ToTable("Funder");
        }
    }
}