﻿using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping
{
    class BadDebtMap : EntityMappingConfiguration<BadDebt>
    {
        public override void Map(EntityTypeBuilder<BadDebt> e)
        {
            e.HasKey(rd => rd.Id);
            e.HasOne(bd => bd.Invoice).WithOne(inv => inv.BadDebt).IsRequired(false).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
            e.ToTable("BadDebt");
        }
    }
}