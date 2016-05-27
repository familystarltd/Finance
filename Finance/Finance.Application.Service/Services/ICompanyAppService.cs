using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;

namespace Finance.Application.Service
{
    public interface ICompanyAppService
    {
        Company SetupCompany(CompanyModel company);
        IEnumerable<CompanyModel> GetCompanies();
        CompanyModel GetCompanyById(Guid companyId);
        CompanyModel GetCompanyByName(string companyName);
    }
}
