using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FinanceManagement.Application.Service;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Infrastructure.Data.Repositories;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using FinanceManagement.Domain;
using System.Infrastructure.CrossCutting.Adapter;
using System.Infrastructure.CrossCutting.Framework.Adapter;

namespace FinanceManagement.WebAPI
{
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
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<AppSettings>(a => Configuration.GetSection("AppSettings"));
            
            
            //-> Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IFunderRepository, FunderRepository>();
            services.AddScoped<IFeeRepository, FeeRepository>();
            services.AddScoped<IFinanceTransactionRepository, FinanceTransactionRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IAppLogRepository, AppLogRepository>();
            //-> Application Services
            services.AddScoped<ICompanyAppService, CompanyAppService>();
            services.AddScoped<ICustomerAppService, CustomerAppService>();
            services.AddScoped<IFunderAppService, FunderAppService>();
            services.AddScoped<IFeeAppService, FeeAppService>();
            services.AddScoped<IFinanceTransactionAppService, FinanceTransactionAppService>();
            services.AddScoped<IInvoiceAppService, InvoiceAppService>();
            services.AddScoped<IAppLogService, AppLogService>();
            //-> Adapters
            services.AddScoped<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
        }
    }
}
