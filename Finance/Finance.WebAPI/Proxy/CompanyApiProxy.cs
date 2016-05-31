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
    public class CompanyApiProxy : HttpClientWrapper
    {
        #region CTOR
        public CompanyApiProxy(ISerialization serializer,HttpContext Context)
            : base("http://intranet.familystarltd.com/api/", Context, serializer)
        { }
        public CompanyApiProxy(string baseUrl, HttpContext Context, ISerialization serializer)
            : base(baseUrl, Context,serializer)
        { }
        #endregion

        #region ASYNC METHODS
        public CompanyModel CompanySetupOnFinanceAsync(CompanyModel company)
        {
            if (company.Id == Guid.Empty)
                return null;
            try
            {
                return Task<CompanyModel>.Run(async () => await this.POST<CompanyModel>("CompanySetup", company)).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<CompanyModel> GetCompanies()
        {
            IEnumerable<CompanyModel> compaines = Task<IEnumerable<CompanyModel>>.Run(async () => await this.GET<IEnumerable<CompanyModel>>(string.Format("CompanyApi/Companies"))).Result;
            return compaines;
        }
        public IEnumerable<CompanyModel> GetCompanies(string ServiceName)
        {
            IEnumerable<CompanyModel> compaines = Task<IEnumerable<CompanyModel>>.Run(async () => await this.GET<IEnumerable<CompanyModel>>(string.Format("CompanyApi/Service/Companies?ServiceName={0}", ServiceName))).Result;
            return compaines;
        }
        public CompanyModel GetCompany(Guid CompanyId)
        {
            CompanyModel company = Task<CompanyModel>.Run(async () => await this.GET<CompanyModel>(string.Format("CompanyApi/Company/{0}", CompanyId))).Result;
            return company;
        }
        #endregion
    }
}