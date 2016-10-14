using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;

namespace Finance.Application.Service
{
    public interface IBusinessAppService
    {
        Business SetupBusiness(BusinessModel business);
        IEnumerable<BusinessModel> GetBusinesses();
        BusinessModel GetBusinessById(Guid businessId);
        BusinessModel GetBusinessByName(string businessName);
    }
}
