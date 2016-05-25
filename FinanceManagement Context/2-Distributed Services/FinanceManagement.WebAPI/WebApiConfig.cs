using System.Web.Http;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Infrastructure.CrossCutting.Logging;
using System.Infrastructure.CrossCutting.Validator;
using System.Infrastructure.CrossCutting.Adapter;
using System.Infrastructure.CrossCutting.Framework.Logging;
using System.Infrastructure.CrossCutting.Framework.Validator;
using System.Infrastructure.CrossCutting.Framework.Adapter;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Infrastructure.Data.Repositories;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using FinanceManagement.Application.Service;
using FinanceManagement.Application.Service.MappingProfile;
using FinanceManagement.Web.Model;
using System.DistributedServices.WebApi;
using FinanceManagement.Domain;
using System;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace FinanceManagement.WebAPI
{
    //public class ApplicationPreload : System.Web.Hosting.IProcessHostPreloadClient
    //{
    //    public void Preload(string[] parameters)
    //    {
    //        WebApiBootstrapper.Instance.Start();
    //    }
    //}
    public static class WebApiConfig
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            //-> Application Services
            services.AddScoped<ICompanyAppService, CompanyAppService>();
            services.AddScoped<ICustomerAppService, CustomerAppService>();
            services.AddScoped<IFunderAppService, FunderAppService>();
            services.AddScoped<IFeeAppService, FeeAppService>();
            services.AddScoped<IFinanceTransactionAppService, FinanceTransactionAppService>();
            services.AddScoped<IInvoiceAppService, InvoiceAppService>();
            services.AddScoped<IAppLogService, AppLogService>();

            return services;
        }
        public static IServiceCollection RegisterRepositoryServices(this IServiceCollection services)
        {
            //-> Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IFunderRepository, FunderRepository>();
            services.AddScoped<IFeeRepository, FeeRepository>();
            services.AddScoped<IFinanceTransactionRepository, FinanceTransactionRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IAppLogRepository, AppLogRepository>();
            return services;
        }
        public static IServiceCollection RegisterAdapterServices(this IServiceCollection services)
        {
            services.AddScoped<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();
            return services;
        }
        public static JsonOutputFormatter JsonFormatter()
        {
            return new JsonOutputFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            };
        }
        public static string CreateDataSourceConnectionString(string CompanyName)
        {
            IOptions<AppSettings> AppSettings = HttpContent;
            string dbName = CompanyName.Replace(" ", string.Empty);
            return string.Format(@"{0}; AttachDbFilename=|DataDirectory|Finance.{1}.mdf; Initial Catalog=Finance.{2}; Integrated Security=True; MultipleActiveResultSets=True", AppSettings.Value.DataSource, dbName, dbName);
        }
        public static void ConfigureUnitOfWork(HttpContext context,string CompanyName)
        {
            FinanceManagementDbContext dbContext = context.RequestServices.GetService<FinanceManagementDbContext>();
            if (dbContext != null)
            {
                dbContext.ConnectionString = WebApiConfig.CreateDataSourceConnectionString(CompanyName);
            }
        }
    }
}
