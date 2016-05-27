using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace System.DistributedServices.WebApi
{

    public struct CorsValues
    {
        public static string ORIGINS = ""; //{ get { return string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CROS_ORIGIN"]) ? "*" : System.Configuration.ConfigurationManager.AppSettings["CROS_ORIGIN"]; } }
        public const string HEADERS = "Accept,Content-Type,X-Requested-With,Origin";
        public const string METHODS = "GET,POST,DELETE,OPTIONS,PUT";
    }
    public class PreflightRequestsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method.Method.Equals("OPTIONS"))
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                // Define and add values to variables: origins, headers, methods (can be global)
                try
                {
                    IEnumerable<string> headerValues = new List<string>();
                    var ORIGIN = string.Empty;
                    var requestOrigin = string.Empty;
                    if (request.Headers.TryGetValues("Access-Control-Allow-Origin", out headerValues))
                    { requestOrigin = headerValues.FirstOrDefault(); }
                    else if(request.Headers.TryGetValues("Origin", out headerValues))
                    { requestOrigin = headerValues.FirstOrDefault(); }
                    ORIGIN = CorsValues.ORIGINS.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(s => s == requestOrigin);                    
                    response.Headers.Add("Access-Control-Allow-Origin", ORIGIN);
                    response.Headers.Add("Access-Control-Allow-Headers", CorsValues.HEADERS);
                    response.Headers.Add("Access-Control-Allow-Methods", CorsValues.METHODS);
                    response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return tsc.Task;
                }
                catch
                {
                    //Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current.CurrentHandler != null?System.Web.HttpContext.Current:null).Log(new Error(new Exception("CROS ORIGIN REQUEST :" + ex.Message)));
                }
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class WebApiCrosPolicyAttribute : Attribute, ICorsPolicyProvider
    {
        private CorsPolicy _policy;
        public WebApiCrosPolicyAttribute()
        {
            // Create a CORS policy.
            _policy = new CorsPolicy { SupportsCredentials = true };
            // Add allowed origins.
            foreach (var origin in CorsValues.ORIGINS.Split(','))
            {
                _policy.Origins.Add(origin);
            }
            // Add allowed methods.
            _policy.Methods.Add("GET");
            _policy.Methods.Add("POST");
            _policy.Methods.Add("DELETE");
            _policy.Methods.Add("OPTIONS");
            _policy.Methods.Add("PUT");
            // Add allowed headers.
            _policy.Headers.Add("Accept");
            _policy.Headers.Add("Content-Type");
            _policy.Headers.Add("X-Requested-With");
            _policy.Headers.Add("Origin");
        }
        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            return Task.FromResult(_policy);
        }
    }
    //public static class WebApiCros
    //{
    //    public static void EnableCors(HttpConfiguration httpConfiguration)
    //    {
    //        httpConfiguration.EnableCors();
    //    }
    //}
}