﻿using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Infrastructure.CrossCutting.Adapter;
using System.Infrastructure.CrossCutting.Framework.Adapter;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Infrastructure.Data.Repositories;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using FinanceManagement.Application.Service;
using FinanceManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using FinanceManagement.Application.Service.MappingProfile;
using System.IO;
using FinanceManagement.Web.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
public class AppSettings
{
    public string ApplicationName { get; set; } = "My Great Application";
    public int MaxItemsPerList { get; set; } = 15;
    public string Service { get; set; }
    public string DataSource { get; set; }
    public string CompanyAPIService { get; set; }
    public string BankHolidaySourceFileOnline { get; set; }
    public string BankHolidaySourceFileOffline { get; set; }
}
namespace FinanceManagement.WebAPI
{
    public static class WebApiConfig
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            //-> Application Services
            services.AddTransient<ICompanyAppService, CompanyAppService>();
            services.AddTransient<ICustomerAppService, CustomerAppService>();
            services.AddTransient<IFunderAppService, FunderAppService>();
            services.AddTransient<IFeeAppService, FeeAppService>();
            services.AddTransient<IFinanceTransactionAppService, FinanceTransactionAppService>();
            services.AddTransient<IInvoiceAppService, InvoiceAppService>();
            services.AddTransient<IAppLogService, AppLogService>();

            return services;
        }
        public static IServiceCollection RegisterRepositoryServices(this IServiceCollection services)
        {
            //-> Repositories
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IFunderRepository, FunderRepository>();
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
        public static void SetDataSourceConnectionString(string CompanyName, System.IServiceProvider Services, AppSettings AppSettings)
        {
            IHostingEnvironment HostEnvironment = Services.GetService<IHostingEnvironment>();
            Infrastructure.Data.UnitOfWork.ConnectionStringDatabase Connection = Services.GetService<Infrastructure.Data.UnitOfWork.ConnectionStringDatabase>();
            string dbName = CompanyName.Replace(" ", string.Empty);
            if (AppSettings.DataSource.Contains("FileName"))// SQLite
                Connection.ConnectionString= $"FileName={Path.Combine(HostEnvironment.ContentRootPath, "App_Data", $"Finance.{dbName}")}.db";
            else
                // return $"{AppSettings.Value.DataSource};AttachDbFilename={Path.Combine(HostEnvironment.ContentRootPath, "App_Data", $"Finance.{dbName}")}.mdf;Initial Catalog=Finance.{dbName};Integrated Security=True; MultipleActiveResultSets=True;";
                Connection.ConnectionString= string.Format(@"{0}; Database=Finance.{2}; Integrated Security=True; MultipleActiveResultSets=True;", AppSettings.DataSource, dbName, dbName);
        }
        public static void ConfigureCompany(System.IServiceProvider Services, AppSettings AppSettings)
        {
            var deseralizeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            CompanyApiProxy CompanyApiProxy = new CompanyApiProxy(AppSettings.CompanyAPIService, null, new System.WebAPIProxy.Serialization.JsonNetSerialization(deseralizeSettings));
            IEnumerable<CompanyModel> companies = CompanyApiProxy.GetCompanies(AppSettings.Service);
            foreach (CompanyModel company in companies)
            {
                SetDataSourceConnectionString(company.Name, Services, AppSettings);
                Application.Service.ICompanyAppService _CompanyAppService = Services.GetService<Application.Service.ICompanyAppService>();
                if (_CompanyAppService != null)
                {
                    _CompanyAppService.SetupCompany(company);
                }
            }
        }
    }
}
