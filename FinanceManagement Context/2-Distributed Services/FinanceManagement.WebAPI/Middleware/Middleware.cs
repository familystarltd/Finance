﻿using System.Threading.Tasks;
using System.Net;
using FinanceManagement.Application.Service;
using FinanceManagement.Web.Model;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace FinanceManagement.WebAPI
{
    // Company Setting HTTP Middleware
    public class CompanySettingMiddleware
    {
        private readonly RequestDelegate _next;
        public CompanySettingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IOptions<AppSettings> AppSettings, Infrastructure.Data.UnitOfWork.ConnectionStringDatabase Connection)
        {
            if (context.Request.Method.ToLower() == "get")
            {
                CompanyModel company = FinanceManagement.Application.Service.Settings.AppSetting.Company;
                Random r = new Random();
                company.Name = "Test" + r.Next(1, 10);
                if (company != null && !string.IsNullOrEmpty(company.Name))
                {
                    IHostingEnvironment host = context.RequestServices.GetService<IHostingEnvironment>();
                    Connection.ConnectionString = WebApiConfig.CreateDataSourceConnectionString(company.Name, host, AppSettings);
                }
            }
            await _next.Invoke(context);
        }
    }
    // USER Setting HTTP handler
    public class UserSettingHttpMiddleware
    {
        private readonly RequestDelegate _next;
        public UserSettingHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method.ToLower() == "get")//(context.Request.Method.ToLower() == "delete" || context.Request.Method.ToLower() == "post" || context.Request.Method.ToLower() == "put")
            {
                IAppLogService _LogService = context.RequestServices.GetService<IAppLogService>();
                AppLogModel appLog = new AppLogModel
                {
                    CompanyName = FinanceManagement.Application.Service.Settings.AppSetting.Company == null ? string.Empty : FinanceManagement.Application.Service.Settings.AppSetting.Company.Name,
                    LogUser = FinanceManagement.Application.Service.Settings.AppSetting.User.Name,
                    LogDateTime = DateTime.Now,
                    Request = context.Request.Path.ToUriComponent(),
                    RequestFrom = context.Connection.RemoteIpAddress.ToString(),
                    RequestMethod = context.Request.Method
                };
                _LogService.Log(appLog);
            }
            await _next.Invoke(context);
        }
        private string GetControllerActionName(HttpContext request)
        {
            Microsoft.AspNetCore.Routing.RouteContext context = new Microsoft.AspNetCore.Routing.RouteContext(request);
            var routeData = context.RouteData;
            var controllerName = routeData.Values["MS_SubRoutes"];
            var actionName = (string)routeData.Values["action"];
            return string.Format("{0}-{1}", controllerName, actionName);
        }
    }
    public class OptionsSettingMiddleware
    {
        private readonly RequestDelegate _next;
        public OptionsSettingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "Options")
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            await _next.Invoke(context);
        }
    }
}