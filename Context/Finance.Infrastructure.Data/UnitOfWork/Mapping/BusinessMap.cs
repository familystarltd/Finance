using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Finance.Domain.Aggregates.CustomerAgg;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    class BusinessMap : EntityMappingConfiguration<Business>
    {
        public override void Map(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Business> b)
        {
            b.HasKey(c => c.Id);
            b.ToTable("Business");
        }
    }
}
