using System;
using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain.Aggregates.CustomerAgg
{
    public interface IBusinessRepository : IRepository<Business>
    {
        IEnumerable<Business> GetBusinesses();
        Business GetBusinessById(Guid businessId);
        Business GetBusinessByName(string businessName);
    }
}
