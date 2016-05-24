using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Web.Model;
using System;
using System.Collections.Generic;

namespace FinanceManagement.Application.Service
{
    public interface ICompanyAppService
    {
        Company SetupCompany(CompanyModel company);
        IEnumerable<CompanyModel> GetCompanies();
        CompanyModel GetCompanyById(Guid companyId);
        CompanyModel GetCompanyByName(string companyName);
    }
}
