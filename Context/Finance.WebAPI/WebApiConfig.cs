using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Infrastructure.CrossCutting.Adapter;
using System.Infrastructure.CrossCutting.Framework.Adapter;
using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Infrastructure.Data.Repositories;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Finance.Application.Service;
using Finance.Domain;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Finance.Application.Service.MappingProfile;
using System.IO;
using Finance.Web.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System;

public class AppSettings
{
    public string ApplicationName { get; set; } = "Finance Management";
    public int MaxItemsPerList { get; set; } = 15;
    public string Service { get; set; }
    public string DataSource { get; set; }
    public string BaseDirectory { get; set; }
    public string BusinessAPIService { get; set; }
    public string BankHolidaySourceFileOnline { get; set; }
    public string BankHolidaySourceFileOffline { get; set; }
}


namespace Finance.WebAPI
{
    public static class WebApiConfig
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            //-> Application Services
            services.AddTransient<IBusinessAppService, BusinessAppService>();
            services.AddTransient<ICustomerAppService, CustomerAppService>();
            services.AddTransient<IPayerAppService, PayerAppService>();
            services.AddTransient<IFeeAppService, FeeAppService>();
            services.AddTransient<IFinanceTransactionAppService, FinanceTransactionAppService>();
            services.AddTransient<IInvoiceAppService, InvoiceAppService>();
            services.AddTransient<IAppLogService, AppLogService>();

            return services;
        }
        public static IServiceCollection RegisterRepositoryServices(this IServiceCollection services)
        {
            //-> Repositories
            services.AddTransient<IBusinessRepository, BusinessRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IPayerRepository, PayerRepository>();
            services.AddTransient<IFeeRepository, FeeRepository>();
            services.AddTransient<IFinanceTransactionRepository, FinanceTransactionRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IAppLogRepository, AppLogRepository>();
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
        public static System.Net.Http.Formatting.JsonMediaTypeFormatter JsonFormatter()
        {
            return new System.Net.Http.Formatting.JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                }
            };
        }
        public static void SetDataSourceConnectionString(string BusinessName, System.IServiceProvider Services, AppSettings AppSettings)
        {
            IHostingEnvironment HostEnvironment = Services.GetService<IHostingEnvironment>();
            Infrastructure.Data.UnitOfWork.ConnectionStringDatabase Connection = Services.GetService<Infrastructure.Data.UnitOfWork.ConnectionStringDatabase>();
            string dbName = BusinessName.Replace(" ", string.Empty);
            if (AppSettings.DataSource.Contains("FileName"))// SQLite
                Connection.ConnectionString= $"FileName={Path.Combine(HostEnvironment.ContentRootPath, "App_Data", $"Finance.{dbName}")}.db";
            else
                // return $"{AppSettings.Value.DataSource};AttachDbFilename={Path.Combine(HostEnvironment.ContentRootPath, "App_Data", $"Finance.{dbName}")}.mdf;Initial Catalog=Finance.{dbName};Integrated Security=True; MultipleActiveResultSets=True;";
                Connection.ConnectionString= string.Format(@"{0}; Database=Finance.{2}; Integrated Security=True; MultipleActiveResultSets=True;", AppSettings.DataSource, dbName, dbName);
        }
        public static void ConfigureBusiness(System.IServiceProvider Services, AppSettings AppSettings)
        {
            var deseralizeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            BusinessApiProxy BusinessApiProxy = new BusinessApiProxy(AppSettings.BusinessAPIService, null, new System.Presentation.WebAPIProxy.Serialization.JsonNetSerialization(deseralizeSettings));
            IEnumerable<BusinessModel> businesses = BusinessApiProxy.GetBusinesses(AppSettings.Service);
            foreach (BusinessModel business in businesses)
            {
                SetDataSourceConnectionString(business.Name, Services, AppSettings);
                Application.Service.IBusinessAppService _BusinessAppService = Services.GetService<Application.Service.IBusinessAppService>();
                if (_BusinessAppService != null)
                {
                    _BusinessAppService.SetupBusiness(business);
                }
            }
        }
    }
}
