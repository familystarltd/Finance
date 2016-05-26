using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Infrastructure.CrossCutting.Adapter;
using System.Infrastructure.CrossCutting.Framework.Adapter;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Infrastructure.Data.Repositories;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using FinanceManagement.Application.Service;
using FinanceManagement.Domain;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using FinanceManagement.Application.Service.MappingProfile;
using System.IO;

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
        public static IServiceCollection RegisterAutomapperProfiles(this IServiceCollection services)
        {
            MapperConfiguration MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainToViewModelMapper());
                cfg.AddProfile(new ViewModelToDomainMapper());
            });
            services.AddSingleton<IMapper>(sp => MapperConfiguration.CreateMapper());
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
        public static string CreateDataSourceConnectionString(string CompanyName, Microsoft.AspNetCore.Hosting.IHostingEnvironment HostEnvironment, IOptions<AppSettings> AppSettings)
        {
            string dbName = CompanyName.Replace(" ", string.Empty);
            if (AppSettings.Value.DataSource.Contains("FileName"))// SQLite
                return $"FileName={Path.Combine(HostEnvironment.ContentRootPath, "App_Data", $"Finance.{dbName}")}.db";
            else
               // return $"{AppSettings.Value.DataSource};AttachDbFilename={Path.Combine(HostEnvironment.ContentRootPath, "App_Data", $"Finance.{dbName}")}.mdf;Initial Catalog=Finance.{dbName};Integrated Security=True; MultipleActiveResultSets=True;";
            return string.Format(@"{0}; Database=Finance.{2}; Integrated Security=True; MultipleActiveResultSets=True;", AppSettings.Value.DataSource, dbName, dbName);
        }
        public static void ConfigureUnitOfWork(HttpContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment HostEnvironment, IOptions<AppSettings> AppSettings, string CompanyName)
        {
            IFinanceDbContext dbContext = context.RequestServices.GetService<IFinanceDbContext>();
            if (dbContext != null)
            {
                //dbContext.ConnectionString = WebApiConfig.CreateDataSourceConnectionString(CompanyName, HostEnvironment, AppSettings);
            }
        }
    }
}
