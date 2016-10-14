using Finance.Web.Data;
using Finance.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Web
{
    public class AppUserStore : UserStore<ApplicationUser>
    {
        public AppUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }
    public class AppSettings
    {
        public string FinanceAPIService { get; set; }
        //public static UserManager<ApplicationUser> UserManager
        //{
        //    get
        //    {
        //        return new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //    }
        //}
        public static RouteValueDictionary SetBusinessRouteValues(Models.Business buisness)
        {
            return new RouteValueDictionary { { "BusinessName", buisness.Name } };
        }
    }
}
