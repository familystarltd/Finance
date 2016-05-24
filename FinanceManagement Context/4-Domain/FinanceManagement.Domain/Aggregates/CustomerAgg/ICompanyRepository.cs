using System;
using System.Collections.Generic;
using System.Domain;

namespace FinanceManagement.Domain.Aggregates.CustomerAgg
{
    public interface ICompanyRepository : IRepository<Company>
    {
        IEnumerable<Company> GetCompanies();
        Company GetCompanyById(Guid companyId);
        Company GetCompanyByName(string companyName);
    }
}
