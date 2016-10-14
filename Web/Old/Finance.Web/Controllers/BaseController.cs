using System;
using Microsoft.AspNetCore.Mvc;
using System.Presentation.WebAPIProxy.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Web.Controllers
{
    public class BaseController<TBusiness> : Controller where TBusiness : class, new()
    {
        protected ISerialization _serializer;
        protected TBusiness Business { get; private set; }
        public BaseController()
        {
            //_serializer = WebConfig.GetJsonSerialization();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Business = BusinessManager.SetBusinessToCookie<TBusiness>(filterContext.HttpContext.Request.Headers["BusinessName"]);
            if (Business == null && !this.ControllerContext.RouteData.Values["action"].ToString().Equals("UnAuthorised", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            }
            base.OnActionExecuting(filterContext);
        }
        [Route("UnAuthorised")]
        [AllowAnonymous]
        public ActionResult UnAuthorised()
        {
            ViewBag.TitleHeader = "Unauthorised access";
            return View();
        }
    }
}