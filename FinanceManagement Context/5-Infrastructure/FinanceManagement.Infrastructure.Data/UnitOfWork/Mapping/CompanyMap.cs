using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FinanceManagement.Domain.Aggregates.CustomerAgg;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    class CompanyMap : EntityMappingConfiguration<Company>
    {
        public override void Map(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Company> b)
        {
            b.HasKey(c => c.Id);
            b.ToTable("Company");
        }
    }
}
