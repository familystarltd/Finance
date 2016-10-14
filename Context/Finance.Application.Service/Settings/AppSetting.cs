using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Finance.Application.Service.Settings
{
    public class AppSetting
    {
        public AppSetting(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext = httpContextAccessor.HttpContext;
        }
        public static HttpContext HttpContext;
        public static UserModel User
        {
            get
            {
                UserModel User = new UserModel { Name = "SYSTEM", LoggedIn = DateTime.Now, Token = "DEVELOPER" };
                try
                {
                    if (HttpContext == null || HttpContext.Request == null)
                        return User;
                    if (HttpContext.Request.Cookies["APP-USER"] != null)
                    {
                        User = Newtonsoft.Json.JsonConvert.DeserializeObject<Web.Model.UserModel>(HttpContext.Request.Cookies["APP-USER"]);
                        return User;
                    }
                    else
                    {
                        StringValues values = new StringValues();
                        HttpContext.Request.Headers.TryGetValue("APP-USER", out values);
                        if(values.FirstOrDefault() != null)
                        {
                            User = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(values.FirstOrDefault());
                            if (User != null)
                            {
                                return User;
                            }
                        }                        
                    }
                }
                catch { }
                return User;
            }
        }
        public static string UserName { get { return User != null && !string.IsNullOrEmpty(User.Name) ? User.Name : "SYSTEM"; } }
        public static string BankHolidayOfflineSourceFile { get; set; }
        public static string BankHolidayOnlineSourceFile { get; set; }
        public static Web.Model.BusinessModel Business
        {
            get
            {
                Web.Model.BusinessModel business = new BusinessModel();
                if (HttpContext == null || HttpContext.Request == null)
                    return business;
                
                if (HttpContext.Request.Cookies["FINANCE-BUSINESS"] != null)
                {
                    business = Newtonsoft.Json.JsonConvert.DeserializeObject<Web.Model.BusinessModel>(HttpContext.Request.Cookies["FINANCE-BUSINESS"]);
                    return business;
                }
                else
                {
                    StringValues values = new StringValues();
                    HttpContext.Request.Headers.TryGetValue("FINANCE-BUSINESS", out values);
                    if (values.FirstOrDefault() != null)
                    {
                        business = Newtonsoft.Json.JsonConvert.DeserializeObject<BusinessModel>(values.FirstOrDefault());
                        if (business != null)
                        {
                            return business;
                        }
                    }
                }
                return business;
            }
        }
    }
}
