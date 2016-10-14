using System.Threading.Tasks;
using System.Net;
using Finance.Application.Service;
using Finance.Web.Model;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Finance.WebAPI
{
    // Business Setting HTTP Middleware
    public class BusinessSettingMiddleware
    {
        private readonly RequestDelegate _next;
        public BusinessSettingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, Application.Service.Settings.AppSetting AppSetting, IOptions<AppSettings> AppSettings)
        {
            if (context.Request.Method.ToLower() != "debug")
            {
                BusinessModel business = Finance.Application.Service.Settings.AppSetting.Business;
                if (business != null && !string.IsNullOrEmpty(business.Name))
                {
                    WebApiConfig.SetDataSourceConnectionString(business.Name, context.RequestServices, AppSettings.Value);
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
            if (context.Request.Method.ToLower() == "delete" || context.Request.Method.ToLower() == "post" || context.Request.Method.ToLower() == "put")
            {
                IAppLogService _LogService = context.RequestServices.GetService<IAppLogService>();
                AppLogModel appLog = new AppLogModel
                {
                    BusinessName = Finance.Application.Service.Settings.AppSetting.Business == null ? string.Empty : Finance.Application.Service.Settings.AppSetting.Business.Name,
                    LogUser = Finance.Application.Service.Settings.AppSetting.User.Name,
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