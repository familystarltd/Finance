using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Finance.Infrastructure.Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using System.Infrastructure.CrossCutting.Adapter;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Linq;

namespace Finance.WebAPI
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

            //services.AddOptions();
            //Add cors built in support. 
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFinanceFamilyStarLtdOrigin",
                    builder => builder
                    .WithOrigins("http://finance.familystarltd.com")
                    .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });

            services.AddMvcCore().AddApiExplorer();
            //Add MVC for supporting WebApi requests
            #region MVC Add services.AddMvc()
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                #region // Input Formatters. 
                //var json = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                //if (json != null)
                //{
                //    json.SerializerSettings.ContractResolver =new CamelCasePropertyNamesContractResolver();
                //}
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
            })
            .AddWebApiConventions()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
            });
            #endregion
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
            // Always first middleware (BusinessSettingHttpMiddleware) to set Business Settings
            app.UseMiddleware<BusinessSettingMiddleware>();
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
            AppSettings.Value.BaseDirectory = env.ContentRootPath;
            // Setup business databases where on the main websites
            WebApiConfig.ConfigureBusiness(app.ApplicationServices, AppSettings.Value);
            app.UseMvc();
        }
    }
}