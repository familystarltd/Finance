using Finance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.DistributedServices.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Finance.Web.Controllers
{
    [Route("BusinessApi")]
    [WebApiCrosPolicy]
    public class BusinessApiController : ApiController
    {
        #region PRIVATE FIELDS
        BusinessService _businessService;
        #endregion
        #region CTOR
        public BusinessApiController(BusinessService businessService)
        {
            _businessService = businessService;
        }
        #endregion
        #region BUSINESS
        [HttpPost]
        [Route("SaveBusiness")]
        public HttpResponseMessage SaveBusiness(Business business)
        {
            try
            {
                //Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current.CurrentHandler != null?System.Web.HttpContext.Current:null).Log(new Error(new Exception("Save COMPANY Called on:" + DateTime.Now.ToString())));
                return Request.CreateResponse(HttpStatusCode.OK, _businessService.SaveBusiness(business));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Business/{businessId:guid}")]
        public HttpResponseMessage GetBusiness(Guid businessId)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, _businessService.GetBusinessById(businessId));
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Businesses")]
        public HttpResponseMessage GetBusinesses()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _businessService.GetBusinesses());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Service/Businesses")]
        public HttpResponseMessage GetBusinesses(string ServiceName)
        {
            try
            {
                //Request.CreateResponse()
                //Response.ContentType = "application/json";
                return Request.CreateResponse(HttpStatusCode.OK, _businessService.GetBusinesses());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion
    }
}