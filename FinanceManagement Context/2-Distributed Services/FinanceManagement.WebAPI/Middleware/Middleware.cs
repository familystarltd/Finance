using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using FinanceManagement.Application.Service;
using FinanceManagement.Web.Model;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.DistributedServices.WebApi;
using FinanceManagement.Infrastructure.Data.UnitOfWork;

namespace FinanceManagement.WebAPI
{
    // Company Setting HTTP Middleware
    public class CompanySettingHttpMiddleware
    {
        private readonly RequestDelegate _next;
        public CompanySettingHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            CompanyModel company = FinanceManagement.Application.Service.Settings.AppSetting.Company;
            if (company != null && !string.IsNullOrEmpty(company.Name))
            {
                
                WebApiConfig.ConfigureUnitOfWork(context,company.Name);
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
            IAppLogService LogService = context.RequestServices.GetService<IAppLogService>();
            //IAppLogService LogService = (IAppLogService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IAppLogService));
            if (LogService != null && (context.Request.Method == "Delete" || context.Request.Method == "Post" || context.Request.Method == "Put"))
            {
                AppLogModel appLog = new AppLogModel
                {
                    CompanyName = FinanceManagement.Application.Service.Settings.AppSetting.Company == null ? string.Empty : FinanceManagement.Application.Service.Settings.AppSetting.Company.Name,
                    LogUser = FinanceManagement.Application.Service.Settings.AppSetting.User.Name,
                    LogDateTime = DateTime.Now,
                    Request = context.Request.Path.ToUriComponent(),
                    RequestFrom = context.Request.GetClientIPAddress(),
                    RequestMethod = context.Request.Method
                };
                LogService.Log(appLog);
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
    public class OptionsHttpMiddleware
    {
        private readonly RequestDelegate _next;
        public OptionsHttpMiddleware(RequestDelegate next)
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
