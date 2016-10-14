using System;
using Microsoft.AspNetCore.Mvc;
using System.Presentation.WebAPIProxy.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Finance.Web.Controllers
{
    public class BaseController<TBusiness> : Controller where TBusiness : class, new()
    {
        AppSettings _AppSettings;
        protected ISerialization _serializer;
        protected TBusiness Business { get; private set; }
        BusinessService _businessService;
        public BaseController(BusinessService businessService,IOptions<AppSettings> appSettings)
        {
            _AppSettings = appSettings.Value;
            _businessService = businessService;
            //_serializer = WebConfig.GetJsonSerialization();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            Microsoft.Extensions.Primitives.StringValues business = "";
            if (filterContext.HttpContext.Request.Query.TryGetValue("BusinessName", out business))
            {
                Microsoft.AspNetCore.Http.CookieOptions opt = new Microsoft.AspNetCore.Http.CookieOptions();
                opt.Expires = DateTimeOffset.Now.AddDays(1);
            }
            Business = _businessService.SetBusinessToCookie<TBusiness>(business);
            ViewBag.IsBusinessExists = true;
            if (Business == null && !this.ControllerContext.RouteData.Values["action"].ToString().Equals("UnAuthorised", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.IsBusinessExists = false;
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            }
            ViewData["FinanceApiService"] = _AppSettings.FinanceAPIService;
            ViewBag.FinanceApiService = _AppSettings.FinanceAPIService;
            base.OnActionExecuting(filterContext);
        }
        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    ViewData["FinanceApiService"] = _AppSettings.FinanceAPIService;
        //    ViewBag.FinanceApiService = _AppSettings.FinanceAPIService;
        //    base.OnActionExecuted(context);
        //}
        [Route("UnAuthorised")]
        [AllowAnonymous]
        public ActionResult UnAuthorised()
        {
            ViewBag.TitleHeader = "Unauthorised access";
            return View();
        }
    }
}