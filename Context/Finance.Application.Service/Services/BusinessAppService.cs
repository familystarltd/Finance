using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Infrastructure.CrossCutting.Framework.Extensions;

namespace Finance.Application.Service
{
    /// <summary>
    /// The Business service implementation.
    /// <see cref="Finance.Application.Service.IBusinessAppService"/>
    /// </summary>
    public class BusinessAppService : IBusinessAppService
    {
        readonly IBusinessRepository _BusinessRepository;
        public BusinessAppService(IBusinessRepository iBusinessRepository)
        {
            _BusinessRepository = iBusinessRepository;
        }
        public Business SetupBusiness(BusinessModel business)
        {
            try
            {
                Business businessLocal = _BusinessRepository.GetBusinessById(business.Id);
                if (businessLocal == null)
                {

                    _BusinessRepository.Add(DataProjections.ProjectedTo<Business>(business));
                }
                else
                {
                    _BusinessRepository.Merge(businessLocal, DataProjections.ProjectedTo<Business>(business));
                }
                _BusinessRepository.UnitOfWork.Commit();
                return _BusinessRepository.GetBusinessById(business.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<BusinessModel> GetBusinesses()
        {
            try
            {
                return this._BusinessRepository.GetBusinesses().ProjectedToCollection<BusinessModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public BusinessModel GetBusinessById(Guid businessId)
        {
            try
            {
                return this._BusinessRepository.GetBusinessById(businessId).ProjectedTo<BusinessModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public BusinessModel GetBusinessByName(string businessName)
        {
            try
            {
                return this._BusinessRepository.GetBusinessByName(businessName).ProjectedTo<BusinessModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}