using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using System.Infrastructure.CrossCutting.Adapter;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
namespace FinanceManagement.WebAPI
{
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
            services.AddMvcCore().AddJsonFormatters(a => a.ContractResolver = new CamelCasePropertyNamesContractResolver());
            //Add cors built in support. 
            services.AddCors();
            services.AddMvcCore().AddApiExplorer();
            //Add MVC for supporting WebApi requests 
            #region MVC Add services.AddMvc(); 
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                #region // Input Formatters. 
                //options.InputFormatters.Clear();
                //options.InputFormatters.Add(new JsonInputFormatter()
                //{
                //    SerializerSettings = new JsonSerializerSettings()
                //    {
                //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                //        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                //        NullValueHandling = NullValueHandling.Ignore
                //    }
                //});
                #endregion
                //Output formater //as part of get/post request, set the header Accept = application/json or application/xml 
                options.OutputFormatters.Clear();
                options.OutputFormatters.Add(new JsonOutputFormatter
                {
                    SerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    }
                });
            }).AddWebApiConventions();
            #endregion
            services.AddOptions();
            services.AddCors();
            services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Microsoft.Extensions.Options.IOptions<AppSettings> AppSettings, ITypeAdapterFactory AdapterFactory)
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
            // Setup company databases where on the main websites
            WebApiConfig.ConfigureCompany(app.ApplicationServices, AppSettings.Value);
            app.UseMvc();
        }
    }
}
