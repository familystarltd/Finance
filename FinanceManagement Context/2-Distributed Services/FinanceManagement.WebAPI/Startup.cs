using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using System.Infrastructure.CrossCutting.Adapter;
using AutoMapper;

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
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.Configure<MvcOptions>(config =>
            {
                config.OutputFormatters.Clear();
                config.OutputFormatters.Add(WebApiConfig.JsonFormatter());
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<Application.Service.Settings.AppSetting>();
            services.AddSingleton(typeof(ConnectionStringDatabase));
            
            services.AddEntityFrameworkSqlServer().AddTransient<IFinanceDbContext, FinanceDbContext>();//.AddTransient(typeof(IFinanceDbContext));
            services.RegisterAppServices();
            services.RegisterRepositoryServices();
            services.RegisterAdapterServices();
            services.RegisterAutomapperProfiles();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ITypeAdapterFactory AdapterFactory)
        {
            // Always first middleware (CompanySettingHttpMiddleware) to set Company Settings
            app.UseMiddleware<CompanySettingMiddleware>();
            app.UseMiddleware<OptionsSettingMiddleware>();
            app.UseMiddleware<UserSettingHttpMiddleware>();
            // Adapter Factory DTO -> Model & Model -> DTO
            TypeAdapterFactory.SetAdapter((ITypeAdapterFactory)AdapterFactory);
            //Logger Factory
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            app.UseMvc();
        }
    }
}
