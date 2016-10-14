using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain.Aggregates.FeeAgg;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Business Repository implementation.
    /// <see cref="Finance.Domain.IBusinessRepository"/>
    /// </summary>
    public class BusinessRepository : Repository<Business>, IBusinessRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public BusinessRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(Business persisted, Business current)
        {
            var currentUOW = this.UnitOfWork as IFinanceDbContext;            
            if (persisted == null || current == null)
                return;
            currentUOW.ApplyCurrentValues(persisted, current);
        }
        public IEnumerable<Business> GetBusinesses()
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            return uow.Businesses;
        }
        public Business GetBusinessById(Guid businessId)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Businesses.Where(c => c.Id == businessId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Business GetBusinessByName(string businessName)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Businesses.Where(c => c.Name == businessName).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}