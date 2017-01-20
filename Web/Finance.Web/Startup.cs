﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Finance.Web.Data;
using Finance.Web.Models;
using Finance.Web.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace Finance.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            //Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "App_Data"));
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddOptions();
            services.AddCors();
            services.AddMvcCore().AddApiExplorer().AddJsonFormatters(a => a.ContractResolver = new CamelCasePropertyNamesContractResolver());
            services.AddMvc();
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            })
            .AddWebApiConventions()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Add framework services.
            services.AddIdentity<ApplicationUser, IdentityRole>(
                user =>
                {
                    user.Password.RequireNonAlphanumeric = false;
                }
                ).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            string connection = Configuration.GetConnectionString("DefaultConnection");
            if (connection.ToLower().Contains("FileName".ToLower()))
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
                services.AddDbContext<BusinessDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                services.AddDbContext<BusinessDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddTransient<IBusinessRepository, BusinessRepository>();
            services.AddScoped<BusinessService>();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AuthorizeAdminIntranet", policy => policy.Requirements.Add(new AuthorizeIntranetRequirement("Administrator")));
            //    options.AddPolicy("AuthorizeManagerIntranet", policy => policy.Requirements.Add(new AuthorizeIntranetRequirement("Manager")));
            //    options.AddPolicy("AuthorizeUserIntranet", policy => policy.Requirements.Add(new AuthorizeIntranetRequirement("User")));
            //});
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
