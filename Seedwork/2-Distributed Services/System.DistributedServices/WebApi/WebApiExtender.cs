using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace System.DistributedServices.WebApi
{
    public static class WebApiExtender
    {
        public static string GetClientIPAddress(this HttpRequestMessage request)
        {
            return request.GetClientIPAddress();
        }
    }
}