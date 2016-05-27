using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Infrastructure.CrossCutting.Framework.Extensions;

namespace Finance.Application.Service
{
    /// <summary>
    /// The Company service implementation.
    /// <see cref="Finance.Application.Service.ICompanyAppService"/>
    /// </summary>
    public class CompanyAppService : ICompanyAppService
    {
        readonly ICompanyRepository _CompanyRepository;
        public CompanyAppService(ICompanyRepository iCompanyRepository)
        {
            _CompanyRepository = iCompanyRepository;
        }
        public Company SetupCompany(CompanyModel company)
        {
            try
            {
                Company companyLocal = _CompanyRepository.GetCompanyById(company.Id);
                if (companyLocal == null)
                {

                    _CompanyRepository.Add(DataProjections.ProjectedTo<Company>(company));
                }
                else
                {
                    _CompanyRepository.Merge(companyLocal, DataProjections.ProjectedTo<Company>(company));
                }
                _CompanyRepository.UnitOfWork.Commit();
                return _CompanyRepository.GetCompanyById(company.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CompanyModel> GetCompanies()
        {
            try
            {
                return this._CompanyRepository.GetCompanies().ProjectedToCollection<CompanyModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CompanyModel GetCompanyById(Guid companyId)
        {
            try
            {
                return this._CompanyRepository.GetCompanyById(companyId).ProjectedTo<CompanyModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CompanyModel GetCompanyByName(string companyName)
        {
            try
            {
                return this._CompanyRepository.GetCompanyByName(companyName).ProjectedTo<CompanyModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}