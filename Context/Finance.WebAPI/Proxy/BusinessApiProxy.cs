using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Presentation.WebAPIProxy;
using System.Presentation.WebAPIProxy.Serialization;
using Finance.Web.Model;
using Microsoft.AspNetCore.Http;

namespace Finance.WebAPI
{
    /// <summary>
    /// The converter to use when deserializing animal objects
    /// </summary>
    public class BusinessApiProxy : HttpClientWrapper
    {
        #region CTOR
        public BusinessApiProxy(ISerialization serializer,HttpContext Context)
            : base("http://intranet.familystarltd.com/api/", Context, serializer)
        { }
        public BusinessApiProxy(string baseUrl, HttpContext Context, ISerialization serializer)
            : base(baseUrl, Context,serializer)
        { }
        #endregion

        #region ASYNC METHODS
        public BusinessModel BusinessSetupOnFinanceAsync(BusinessModel business)
        {
            if (business.Id == Guid.Empty)
                return null;
            try
            {
                return Task<BusinessModel>.Run(async () => await this.POST<BusinessModel>("BusinessSetup", business)).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<BusinessModel> GetBusinesses()
        {
            IEnumerable<BusinessModel> compaines = Task<IEnumerable<BusinessModel>>.Run(async () => await this.GET<IEnumerable<BusinessModel>>(string.Format("BusinessApi/Businesses"))).Result;
            return compaines;
        }
        public IEnumerable<BusinessModel> GetBusinesses(string ServiceName)
        {
            try
            {
                IEnumerable<BusinessModel> compaines = Task<IEnumerable<BusinessModel>>.Run(async () => await this.GET<IEnumerable<BusinessModel>>(string.Format("BusinessApi/Service/Businesses?ServiceName={0}", ServiceName))).Result;
                return compaines;
            }
            catch {return new List<BusinessModel>(); }
        }
        public BusinessModel GetBusiness(Guid BusinessId)
        {
            BusinessModel business = Task<BusinessModel>.Run(async () => await this.GET<BusinessModel>(string.Format("BusinessApi/Business/{0}", BusinessId))).Result;
            return business;
        }
        #endregion
    }
}