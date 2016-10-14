using System;
using System.Linq;
using System.Collections;
using System.Presentation.WebAPIProxy.Serialization;
using Finance.Web.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Finance.Web.Data;

namespace Finance.Web
{
    public class BusinessManager
    {
        public static HttpContext HttpContext;
        private static ISerialization _serializer;
        private static IBusinessRepository BusinessRepository;
        public BusinessManager(IHttpContextAccessor httpContextAccessor, IBusinessRepository businessRepository)
        {
            HttpContext = httpContextAccessor.HttpContext;
            _serializer = GetJsonSerialization();
            BusinessRepository = businessRepository;
        }
        public static ISerialization GetJsonSerialization()
        {
            var deseralizeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            return new JsonNetSerialization(deseralizeSettings);
        }
        static Business SetBusinessCookie(string businessJson)
        {
            Business business = new Business();            
            if (!string.IsNullOrEmpty(businessJson))
            {
                business = _serializer.DeSerialize<Business>(businessJson);
                BusinessManager.SetBusinessCookie(business);
            }
            return business;
        }
        static void SetBusinessCookie(Business Business)
        {           
            Business.Logo = null;
            Business.Departments = null;
            string json = _serializer.Serialize<Business>(Business);
            HttpContext.Response.Cookies.Append("HR-COMPANY", json);
        }
        static Business GetBusiness(string businessName)
        {
            if (string.IsNullOrEmpty(businessName))
                return GetBusiness<Business>();
            Models.Business business = BusinessRepository.GetBusinessByName(businessName);
            if (business != null)
            {
                business.Services.ToList().ForEach(s => s.Businesses = null);
                business.Departments = null;
            }
            return business;
        }
        public static bool IsInService(string Service)
        {
            Business business = GetBusiness(null);
            return business != null ? business.IsInService(Service) : false;
        }
        public static T SetBusinessToCookie<T>(string BusinessName)
        {
            Business Business = GetBusiness(BusinessName);
            if (Business != null)
            {
                Business.Logo = null;
                SetBusinessCookie(Business);
            }
            return _serializer.Serialize<Models.Business, T>(Business);
        }
        public static T GetBusiness<T>()
        {
            Business business = new Business();
            if (HttpContext.Request.Cookies["HR-COMPANY"] != null)
            {
                var businessJson = HttpContext.Request.Cookies["HR-COMPANY"];
                business = BusinessManager.SetBusinessCookie(businessJson);
            }
            if (business == null || business.Id == Guid.Empty)
                return default(T);
            return _serializer.Serialize<Models.Business, T>(business);
        }
    }
}