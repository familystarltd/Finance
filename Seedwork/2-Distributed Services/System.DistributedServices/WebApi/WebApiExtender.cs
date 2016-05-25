namespace System.DistributedServices.WebApi
{
    public static class WebApiExtender
    {
        public static string GetClientIPAddress(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return request.GetClientIPAddress();
        }
    }
}