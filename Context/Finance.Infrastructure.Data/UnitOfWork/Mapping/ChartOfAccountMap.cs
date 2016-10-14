using Finance.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
{
    class AccountMap : EntityMappingConfiguration<Account>
    {
        public override void Map(EntityTypeBuilder<Account> b)
        {
            b.HasKey(a => a.Id);
            b.HasIndex(a => a.Name).IsUnique(true);
            b.HasMany(a => a.ContraAccounts).WithOne(a => a.ContraOffsetAccount).IsRequired(false).HasForeignKey(a => a.ContraOffsetAccountId);//.OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("Account");
        }
    }
    class AccountGroupMap : EntityMappingConfiguration<AccountGroup>
    {
        public override void Map(EntityTypeBuilder<AccountGroup> b)
        {
            b.HasKey(a => a.Id);
            b.HasIndex(a => a.Name).IsUnique(true);
            b.HasMany(a => a.AccountGroups).WithOne(a => a.AccountGroupParent).IsRequired(false).HasForeignKey(a => a.AccountGroupParentId);//.OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            b.ToTable("AccountGroup");
        }
    }
}