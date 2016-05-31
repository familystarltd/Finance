﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain.Aggregates.FeeAgg;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Company Repository implementation.
    /// <see cref="Finance.Domain.ICompanyRepository"/>
    /// </summary>
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public CompanyRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(Company persisted, Company current)
        {
            var currentUOW = this.UnitOfWork as IFinanceDbContext;            
            if (persisted == null || current == null)
                return;
            currentUOW.ApplyCurrentValues(persisted, current);
        }
        public IEnumerable<Company> GetCompanies()
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            return uow.Companies;
        }
        public Company GetCompanyById(Guid companyId)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Companies.Where(c => c.Id == companyId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Company GetCompanyByName(string companyName)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Companies.Where(c => c.Name == companyName).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}