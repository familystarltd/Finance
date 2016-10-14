using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Finance.Web.Data;
using Finance.Web.Models;
using Finance.WebAPIProxy.Proxies;
using System.IO;
using System.Presentation.WebAPIProxy.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Web.Controllers
{
#if (!DEBUG)
    [Authorize(Roles = "Administrator, Manager,User")]
#endif
    //[Authorize(Roles = "Administrator, Manager,User")]
    public class HomeController : Controller
    {
        #region PRIVATE FIELDS
        AppSettings _AppSettings;
        private string financeAPIService = "";
        BusinessService _businessService;
        protected ISerialization _serializer;
        #endregion
        public HomeController(BusinessService businessService, IOptions<AppSettings> appSettings)
        {
            _AppSettings = appSettings.Value;
            financeAPIService = _AppSettings.FinanceAPIService;
            _businessService = businessService;
            var deseralizeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto
            };
            _serializer = new JsonNetSerialization(deseralizeSettings);
        }
        public IActionResult Index()
        {
            BusinessViewModel businessViewModel = new BusinessViewModel { Business = new Business() };
            businessViewModel.Businesses = _businessService.GetBusinesses();
            if (businessViewModel.Businesses == null || businessViewModel.Businesses.Count() <= 0)
            {
                return RedirectToAction("Business", new { businessId = Guid.Empty });
            }
            return View(businessViewModel);
            //return Redirect("Finance");
        }
        public ActionResult Business(Guid? businessId)
        {
            BusinessViewModel comapanyView = new BusinessViewModel();
            if (businessId == null)
            {
                comapanyView.Businesses = _businessService.GetBusinesses();
                return View("Business", comapanyView);
            }
            else if (businessId == Guid.Empty)
            {
                comapanyView.Business = new Business();
                comapanyView.Businesses = null;
                return View("Business", comapanyView);
            }
            comapanyView.Business = _businessService.GetBusinessById(businessId.Value);
            comapanyView.Businesses = null;
            ViewBag.TitleHeader = "Dashboard";
            return View("Business", comapanyView);
        }
        [HttpPost]
        public ActionResult Business(Business Business)
        {
            //foreach (string upload in Request.Files)
            //{
            //    if (Request.Files[upload].ContentLength == 0) continue;
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        Request.Files[upload].InputStream.CopyTo(ms);
            //        Business.Logo = ms.ToArray();
            //    }
            //}
            BusinessViewModel businessViewModel = new BusinessViewModel();
            businessViewModel.Businesses = null;
            businessViewModel.Business = _businessService.SaveBusiness(Business);
            businessViewModel.SuccessMessage = "Business has been updated successfully";
            using (FinanceApiProxy HrProxy = new FinanceApiProxy(HttpContext, financeAPIService, _serializer))
            {
                HrProxy.BusinessSetup(businessViewModel.Business.Id);
            }
            // Setup Business for each repected service
            //foreach (Service Service in Business.Services)
            //{
            //    switch (Service.Name.ToUpper())
            //    {
            //        case "HR":
            //            using (HRApiProxy HrProxy = new HRApiProxy(hrAPIService, _serializer))
            //            {
            //                HrProxy.BusinessSetup(businessViewModel.Business.Id);
            //            }
            //            break;
            //        case "CARING":
            //            using (CareApiProxy HrProxy = new CareApiProxy(caringAPIService, _serializer))
            //            {
            //                HrProxy.BusinessSetup(businessViewModel.Business.Id);
            //            }
            //            break;
            //        case "FINANCE":
            //            using (FinanceApiProxy HrProxy = new FinanceApiProxy(financeAPIService, _serializer))
            //            {
            //                HrProxy.BusinessSetup(businessViewModel.Business.Id);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
            return View("Business", businessViewModel);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}