using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using FinanceManagement.Domain;
using FinanceManagement.Infrastructure.Data.Repositories;
using FinanceManagement.Application.Service;
using System.Infrastructure.CrossCutting.Adapter;
using System.Infrastructure.CrossCutting.Framework.Adapter;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Infrastructure.Data.UnitOfWork;

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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddOptions();
            services.AddCors();
            services.Configure<AppSettings>(a => Configuration.GetSection("AppSettings"));
            services.Configure<MvcOptions>(config =>
            {
                config.OutputFormatters.Clear();
                config.OutputFormatters.Add(WebApiConfig.JsonFormatter());
            });
            services.RegisterAppServices();
            services.RegisterRepositoryServices();
            services.RegisterAdapterServices();
            services.AddTransient(typeof(FinanceManagementDbContext));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<CompanySettingHttpMiddleware>();
            app.UseMiddleware<UserSettingHttpMiddleware>();
            app.UseMiddleware<OptionsHttpMiddleware>();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
