using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.WebAPIProxy.Serialization;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace System.WebAPIProxy
{
    //public void Configure(IApplicationBuilder app)
    //{
    //    //other code            
    //    HttpHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
    //}
    public static class HttpHelper
    {
        private static IHttpContextAccessor HttpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public static HttpContext HttpContext
        {
            get
            {
                return HttpContextAccessor.HttpContext;
            }
        }
    }
    public abstract class HttpClientWrapper : IDisposable
    {
        private string _baseUrl;
        private Lazy<HttpClient> _lazyClient;
        ISerialization _serialization;
        public HttpClientWrapper(string baseUrl, HttpContext httpContext, ISerialization serializer)
        {
            _baseUrl = baseUrl.Trim('/');
            _lazyClient = new Lazy<HttpClient>(() => new HttpClient()
            {
                BaseAddress = new Uri(baseUrl),
            });
            _serialization = serializer;
            // = HttpHelper.HttpContext; //HttpContext.Current;
            if (httpContext != null && httpContext != null)
            {
                if (httpContext.Request != null && httpContext.Request.Cookies != null)
                {
                    httpContext.Request.Cookies.ToList().ForEach(key => AddHeader(key.Key, httpContext.Request.Cookies[key.Key]));
                }
                //if (httpContext.Response != null && httpContext.Response.Cookies != null)
                //{
                //    httpContext.Response.co.Cookies.ToList().ForEach(key => AddHeader(key, httpContext.Response.Cookies[key].Value));
                //}
            }
        }
        protected HttpClient Client()
        {
            if (_lazyClient == null)
            {
                throw new ObjectDisposedException("Web client has been disposed");
            }
            _lazyClient.Value.DefaultRequestHeaders.Accept.Clear();
            _lazyClient.Value.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _lazyClient.Value;
        }
        protected HttpResponseMessage HttpResponseMessage { get; private set; }
        public void AddHeader(string name, string value)
        {
            _lazyClient.Value.DefaultRequestHeaders.Remove(name);
            _lazyClient.Value.DefaultRequestHeaders.Add(name, value);
        }
        protected void SetTimeOut(TimeSpan Timeout)
        {
            _lazyClient.Value.Timeout = Timeout;
        }
        protected T GetHearderValues<T>(string name)
        {

            IEnumerator<string> iEnum = this.HttpResponseMessage.Headers.GetValues(name).GetEnumerator();
            string value = string.Empty;
            while (iEnum.MoveNext())
            {
                if (iEnum.Current != null)
                {
                    value = iEnum.Current;
                }
            }
            return _serialization.DeSerialize<T>(value);
        }
        protected async Task GET(string urlSegment)
        {
            this.HttpResponseMessage = await Client().GetAsync(urlSegment);
            if (this.HttpResponseMessage.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                AggregateException ex = this.HttpResponseMessage.Content.ReadAsStringAsync().Exception;
                if (ex != null)
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                else
                {
                    string message = this.HttpResponseMessage.Content.ReadAsStringAsync().Result;
                    throw new Exception(message);
                }
            }
        }
        protected async Task<T> GET<T>(string urlSegment)
        {
            this.HttpResponseMessage = await Client().GetAsync(urlSegment);
            if (this.HttpResponseMessage.IsSuccessStatusCode)
            {
                return _serialization.DeSerialize<T>(this.HttpResponseMessage.Content.ReadAsStringAsync().Result);
            }
            else
            {
                AggregateException ex = this.HttpResponseMessage.Content.ReadAsStringAsync().Exception;
                if (ex != null)
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                else
                {
                    string message = this.HttpResponseMessage.Content.ReadAsStringAsync().Result;
                    throw new Exception(message);
                }
            }
        }
        protected async Task<T> POST<T>(string UrlSegment, T data)
        {
            HttpContent content = new StringContent(_serialization.Serialize<T>(data), Encoding.UTF8, "application/json");
            this.HttpResponseMessage = await Client().PostAsync(UrlSegment, content); //Client().PostAsJsonAsync<T>(UrlSegment, data);
            if (this.HttpResponseMessage.IsSuccessStatusCode)
            {
                return _serialization.DeSerialize<T>(await this.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                AggregateException ex = this.HttpResponseMessage.Content.ReadAsStringAsync().Exception;
                if (ex != null)
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                else
                    throw new Exception(this.HttpResponseMessage.Content.ReadAsStringAsync().Result);
            }
        }
        protected async Task<T> PUT<T>(string UrlSegment, T data)
        {
            HttpContent content = new StringContent(_serialization.Serialize<T>(data), Encoding.UTF8, "application/json");
            this.HttpResponseMessage = await Client().PutAsync(UrlSegment, content); // Client().PutAsJsonAsync<T>(UrlSegment, data);
            if (this.HttpResponseMessage.IsSuccessStatusCode)
            {
                return _serialization.DeSerialize<T>(await this.HttpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                AggregateException ex = this.HttpResponseMessage.Content.ReadAsStringAsync().Exception;
                if (ex != null)
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                else
                    throw new Exception(this.HttpResponseMessage.Content.ReadAsStringAsync().Result);
            }
            throw new Exception("Cannot update.............");
        }
        protected async Task DELETE(string UrlSegment)
        {
            this.HttpResponseMessage = await Client().DeleteAsync(UrlSegment);
            if (!this.HttpResponseMessage.IsSuccessStatusCode)
            {
                AggregateException ex = this.HttpResponseMessage.Content.ReadAsStringAsync().Exception;
                if (ex != null)
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                else
                    throw new Exception(this.HttpResponseMessage.Content.ReadAsStringAsync().Result);
                throw new Exception("Unable to delete.............");
            }
        }
        #region DISPOSE
        ~HttpClientWrapper()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_lazyClient != null)
            {
                if (disposing)
                {
                    if (_lazyClient.IsValueCreated)
                    {
                        _lazyClient.Value.Dispose();
                        _lazyClient = null;
                    }
                }
                // There are no unmanaged resources to release
            }
        }
        #endregion
    }
}