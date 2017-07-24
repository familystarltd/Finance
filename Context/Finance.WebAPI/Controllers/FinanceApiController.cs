using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Finance.Web.Model;
using Finance.Application.Service;
using System.Infrastructure.CrossCutting.Framework;
using Newtonsoft.Json;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using System.Presentation.WebAPIProxy.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Cors;

namespace Finance.WebAPI.Controllers
{
    [Route("FinanceApi")]
    [EnableCors("AllowFinanceFamilyStarLtdOrigin")]
    public class FinanceApiController : ApiController
    {
        public override ResponseMessageResult ResponseMessage(HttpResponseMessage response)
        {
            JsonMediaTypeFormatter _camelCasingFormatter = new JsonMediaTypeFormatter();
            _camelCasingFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var objectContent = response.Content as ObjectContent;
            if (objectContent != null)
            {
                if (objectContent.Formatter is JsonMediaTypeFormatter)
                {
                    response.Content = new ObjectContent(objectContent.ObjectType, objectContent.Value, _camelCasingFormatter);
                }
            }
            return base.ResponseMessage(response);
        }
        public override JsonResult Json<T>(T content, JsonSerializerSettings serializerSettings)
        {
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return base.Json<T>(content, serializerSettings);
        }

        #region PRIVATE FIELDS
        readonly IBusinessAppService _BusinessAppService;
        readonly ICustomerAppService _CustomerAppService;
        readonly IPayerAppService _PayerAppService;
        readonly IFeeAppService _FeeAppService;
        readonly IInvoiceAppService _InvoiceAppService;
        readonly IFinanceTransactionAppService _FinanceTransactionAppService;
        AppSettings AppSettings;
        //private string AppSettings.BusinessAPIService = Microsoft.Extensions.Configuration.ConfigurationExtensions.AppSettings["BusinessAPIService"];
        private ISerialization _serializer;
        #endregion

        JsonMediaTypeFormatter _camelCasingFormatter = new JsonMediaTypeFormatter();

        #region CTOR
        public FinanceApiController(IOptions<AppSettings> AppSettings, IBusinessAppService BusinessAppService, ICustomerAppService CustomerAppService, IPayerAppService PayerAppService, IFeeAppService FeeAppService, IInvoiceAppService InvoiceAppService, IFinanceTransactionAppService FinanceTransactionAppService)
        {
            _camelCasingFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _camelCasingFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            if (_BusinessAppService == null)
                this._BusinessAppService = BusinessAppService;
            if (_CustomerAppService == null)
                this._CustomerAppService = CustomerAppService;
            if (_PayerAppService == null)
                this._PayerAppService = PayerAppService;
            if (_FeeAppService == null)
                this._FeeAppService = FeeAppService;
            if (this._InvoiceAppService == null)
                this._InvoiceAppService = InvoiceAppService;
            if (_FinanceTransactionAppService == null)
                this._FinanceTransactionAppService = FinanceTransactionAppService;
            this.AppSettings = AppSettings.Value;

            var deseralizeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            _serializer = new JsonNetSerialization(deseralizeSettings);
        }
        //public FinanceApiController() { }
        #endregion

        #region DASHBOARD
        [HttpGet]
        [Route("NoOfDays")]
        public HttpResponseMessage GetNoOfDays(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                Calendar calendar = new Calendar(AppSettings.BankHolidaySourceFileOffline);
                int weekdays = 0;
                int saturdays = 0;
                int sundays = 0;
                int bankHolidays = 0;
                calendar.GetDays(FromDate.Date, ToDate.Date, out weekdays, out saturdays, out sundays, out bankHolidays);
                int weekends = saturdays + sundays;
                var Days = new { weekdays = weekdays, saturdays = saturdays, sundays = sundays, bankHolidays = bankHolidays };
                return Request.CreateResponse(HttpStatusCode.OK, Days);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }

        }
        [HttpPost]
        [Route("Dashboard")]
        public HttpResponseMessage Dashboard(DashboardViewModel dashboard)
        {
            try
            {
                #region TEST CREATE RECEIPT - COMMENTED
                //Guid PayerId = new Guid("c9a5e68f-c13e-46c1-9eec-b13a47d9f26e");
                //IEnumerable<InvoiceModel> invoices = this._InvoiceAppService.GetInvoicesForPayment(PayerId, DateTime.Now.Date);
                //InvoiceModel invoice = null;//this._InvoiceAppService.GetInvoice(1001);
                //invoice = invoices.FirstOrDefault();
                //if (invoice != null)
                //{
                //    ReceiptModel receipt = new ReceiptModel();
                //    receipt.PaymentMethod = ReceiptPayMethodModel.CreditNote;
                //    receipt.PayerId = invoice.Payer.Id;
                //    receipt.CustomerId = invoice.Customer.Id;
                //    receipt.ProcessedDate = DateTime.Now.Date;
                //    switch (receipt.PaymentMethod)
                //    {
                //        case ReceiptPayMethodModel.BankTransfer:
                //            receipt.Amount = 10000;
                //            receipt.ContactReference = "Bank Ref 0001";
                //            break;
                //        case ReceiptPayMethodModel.Cash:
                //            receipt.Amount = 10000;
                //            receipt.ContactReference = "Cash Ref 0001";
                //            break;
                //        case ReceiptPayMethodModel.CreditNote:
                //            IEnumerable<CreditNoteModel> creditNotes = this._FinanceTransactionAppService.GetCreditNotes(PayerId, DateTime.Now.Date);
                //            if (creditNotes == null || creditNotes.Count() == 0)
                //                throw new Exception("Credit Note has not been found");
                //            CreditNoteModel credit = creditNotes.FirstOrDefault();
                //            receipt.CreditNoteId = credit.Id;
                //            receipt.Amount = credit.CreditAvailable - 3000;
                //            receipt.ContactReference = string.Format("Paid by Credit Note {0}", credit.CreditNoteNo);
                //            break;
                //        case ReceiptPayMethodModel.Cheque:
                //            receipt.Amount = 10000;
                //            receipt.ContactReference = "Cheque 0001";
                //            break;
                //        default:
                //            break;
                //    }
                //    ReceiptInvoiceModel receiptInvoice = new ReceiptInvoiceModel();
                //    receiptInvoice.Invoice = null;
                //    receiptInvoice.InvoiceId = invoice.Id;
                //    receiptInvoice.AmountReceived = receipt.PaymentMethod == ReceiptPayMethodModel.CreditNote ? receipt.Amount : invoice.DueAmount;
                //    receipt.ReceiptInvoices = new List<ReceiptInvoiceModel>();
                //    receipt.ReceiptInvoices.Add(receiptInvoice);
                //    this._FinanceTransactionAppService.CreateReceipt(receipt);
                //}
                #endregion

                if (dashboard == null)
                    dashboard = new DashboardViewModel();
                Calendar calendar = new Calendar(AppSettings.BankHolidaySourceFileOffline, AppSettings.BankHolidaySourceFileOnline);
                IEnumerable<Event> eventsCurrentYear = calendar.GetEvents(DateTime.Now.Year);
                IEnumerable<Event> eventsNextYear = calendar.GetEvents(DateTime.Now.AddMonths(1).Year);
                dashboard.IsBankHolidayFileExists = eventsCurrentYear != null && eventsCurrentYear.Count()>0 && eventsNextYear != null && eventsNextYear.Count()>0;
                dashboard.CustomersWithoutFees = _CustomerAppService.GetCustomersWithoutFees();
                // Get invoice statuses after processing of invoices.
                int NoOfNewInvoices = 0;
                int NoOfApprovedInvoices = 0;
                int NoOfPaidInvoices = 0;
                int NoOfDeletedInvoices = 0;
                int NoOfCancelledInvoices = 0;
                int NoOfUnApprovedInvoices = 0;
                int NoOfUnPaidInvoices = 0;
                this._InvoiceAppService.GetNoOfInvoicesByStatus(out NoOfNewInvoices, out NoOfApprovedInvoices, out NoOfPaidInvoices, out NoOfDeletedInvoices, out NoOfCancelledInvoices, out NoOfUnApprovedInvoices, out NoOfUnPaidInvoices);
                dashboard.NoOfNewInvoices = NoOfNewInvoices;
                dashboard.NoOfApprovedInvoices = NoOfApprovedInvoices;
                dashboard.NoOfPaidInvoices = NoOfPaidInvoices;
                dashboard.NoOfDeletedInvoices = NoOfDeletedInvoices;
                dashboard.NoOfCancelledInvoices = NoOfCancelledInvoices;
                dashboard.NoOfUnApprovedInvoices = NoOfUnApprovedInvoices;
                dashboard.NoOfUnPaidInvoices = NoOfUnPaidInvoices;
                dashboard.InvoiceProcessedMaxDate = this._InvoiceAppService.GetFeeInvoiceProcessedMaxDate();
                if(!dashboard.InvoiceProcessedMaxDate.HasValue)
                {
                    dashboard.InvoiceProcessedMaxDate = _FeeAppService.GetMinFeeDate();
                }
                else
                {
                    dashboard.InvoiceProcessedMaxDate = dashboard.InvoiceProcessedMaxDate.Value.AddMonths(1);
                }
                dashboard.Businesses = this._BusinessAppService.GetBusinesses();
                if (dashboard.Businesses.Count() > 0 && string.IsNullOrEmpty(dashboard.BusinessName))
                {
                    dashboard.BusinessName = dashboard.Businesses.FirstOrDefault().Name;
                }
                if (dashboard.CustomerPagination != null)
                {
                    DateTime FeeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).Date.AddDays(-1);
                    int TotalRowCount = 0;
                    dashboard.Customers = _CustomerAppService.GetCustomersWithFees(FeeDate, dashboard.CustomerPagination.PageIndex, dashboard.CustomerPagination.PageSize, out TotalRowCount);
                    this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            PageSize = dashboard.CustomerPagination.PageSize,
                            TotalCount = TotalRowCount,
                            PageIndex = dashboard.CustomerPagination.PageIndex,
                            TotalPages = (int)Math.Ceiling((double)TotalRowCount / dashboard.CustomerPagination.PageSize),
                        }));
                }
                else
                {
                    dashboard.Customers = _CustomerAppService.GetCustomersByBusiness(dashboard.BusinessName);
                }
                return Request.CreateResponse(HttpStatusCode.OK, dashboard, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpPost]
        [Route("UploadBankHolidayFile")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> UploadBankHolidayFile()
        {
            try
            {

                var provider = new MultipartMemoryStreamProvider();
                string root = AppSettings.BaseDirectory;
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(file.Headers.ContentDisposition.FileName.Trim('\"'));
                    var filePath = System.IO.Path.Combine(root, info.Name);
                    byte[] fileArray = await file.ReadAsByteArrayAsync();
                    using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        await fs.WriteAsync(fileArray, 0, fileArray.Length);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region BUSINESS
        [HttpGet]
        [Route("Business/Setup/{businessId:guid}")]
        public HttpResponseMessage BusinessSetup(Guid businessId)
        {
            try
            {
                BusinessApiProxy BusinessApiProxy = new BusinessApiProxy(AppSettings.BusinessAPIService, this.ActionContext.HttpContext, _serializer);
                BusinessModel business = BusinessApiProxy.GetBusiness(businessId);
                if (business == null || string.IsNullOrEmpty(business.Name) || business.Id == Guid.Empty)
                {
                    //Elmah.ErrorLog.GetDefault(this.ActionContext.HttpContext).Log(new Error(new Exception("Failed on BUSINESS creation From Main site Called on:" + DateTime.Now.ToString())));
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
                //WebApiConfig.ConfigureUnitOfWork(business.Name);
                Application.Service.IBusinessAppService _BusinessAppService = null; //(Application.Service.IBusinessAppService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(Application.Service.IBusinessAppService));
                if (_BusinessAppService != null)
                {
                    _BusinessAppService.SetupBusiness(business);
                }
                //Elmah.ErrorLog.GetDefault(this.ActionContext.HttpContext.CurrentHandler != null?this.ActionContext.HttpContext:null).Log(new Error(new Exception("Save BUSINESS From Main site Called on:" + DateTime.Now.ToString())));
                return Request.CreateResponse(HttpStatusCode.OK, _BusinessAppService.SetupBusiness(business), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region CUSTOMER
        [HttpGet]
        [Route("Customers")]
        public HttpResponseMessage GetActiveCustomers(string searchText)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _CustomerAppService.GetActiveCustomers(searchText), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("CustomersWithFees")]
        public HttpResponseMessage GetActiveCustomersWithFees(int PageIndex, int PageSize)
        {
            try
            {
                int TotalRowCount = 0;
                DateTime FeeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).Date.AddDays(-1);
                IEnumerable<CustomerModel> customers = _CustomerAppService.GetCustomersWithFees(FeeDate, PageIndex, PageSize, out TotalRowCount);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                    }));
                return Request.CreateResponse(HttpStatusCode.OK, customers, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("CustomersAll")]
        public HttpResponseMessage GetAllCustomers(string searchText)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _CustomerAppService.GetAllCustomers(searchText), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("CustomersBySearch", Name = "CustomersBySearch")]
        public HttpResponseMessage GetCustomersBySearch(string searchText, int pageIndex, int pageSize)
        {
            try
            {
                int TotalRowCount = 0;

                IEnumerable<CustomerModel> customers = _CustomerAppService.GetAllCustomers(searchText, pageIndex, pageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("CustomersBySearch", new { searchText = string.IsNullOrEmpty(searchText) ? " " : searchText }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("CustomersBySearch", new { page = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("CustomersBySearch", new { page = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));

                return Request.CreateResponse(HttpStatusCode.OK, customers, _camelCasingFormatter);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Customers/{customerId:guid}")]
        public HttpResponseMessage GetCustomer(Guid customerId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _CustomerAppService.GetCustomer(customerId), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpPost]
        [Route("SaveCustomer")]
        public HttpResponseMessage SaveCustomer(CustomerModel customerModel)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _CustomerAppService.SaveCustomer(customerModel), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region PAYER
        [HttpGet]
        [Route("NewPayer")]
        public HttpResponseMessage NewPayer()
        {
            try
            {
                PayerModel fundModel = new PayerModel();
                fundModel.PersonalContact = new ContactModel();
                return Request.CreateResponse(HttpStatusCode.OK, fundModel, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Payers")]
        public HttpResponseMessage GetPayers()
        {
            try
            {
                int rows = 0;
                return Request.CreateResponse(HttpStatusCode.OK, _PayerAppService.GetPayers(string.Empty, 0, 500, out rows), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Payers/{Id:guid}")]
        public HttpResponseMessage GetPayer(Guid Id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._PayerAppService.GetPayer(Id), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Payers")]
        public HttpResponseMessage GetPayer(string searchText)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._PayerAppService.GetPayers(searchText), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("PayersBySearch", Name = "PayersBySearch")]
        public HttpResponseMessage PayersBySearch(string searchText, int pageIndex, int pageSize)
        {
            try
            {
                int TotalRowCount = 0;
                IEnumerable<PayerModel> payers = _PayerAppService.GetPayers(searchText, pageIndex, pageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("PayersBySearch", new { searchText = string.IsNullOrEmpty(searchText) ? " " : searchText }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("PayersBySearch", new { page = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("PayersBySearch", new { page = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, payers, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpPost]
        [Route("Payer")]
        public HttpResponseMessage UpdatePayer(PayerModel payerModel)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _PayerAppService.SavePayer(payerModel), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        [HttpDelete]
        [Route("Payer/Delete/{Id:guid}")]
        public HttpResponseMessage DeletePayer(Guid Id)
        {
            try
            {
                _PayerAppService.DeletePayer(Id);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (ArgumentException)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
        #endregion

        #region FEE
        [Route("Fees/{feeId:guid}")]
        public HttpResponseMessage GetFee(Guid feeId)
        {
            try
            {
                FeeModel fee = _FeeAppService.GetFee(feeId);
                if (fee != null)
                    fee.MaxFeeSetupDate = _CustomerAppService.GetMaxFeeSetupDate(fee.CustomerId);
                return Request.CreateResponse(HttpStatusCode.OK, fee, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpPost]
        [Route("Fee")]
        public HttpResponseMessage SaveFee(FeeModel feeModel)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._FeeAppService.SaveFee(feeModel), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region EXPENSES
        [HttpGet]
        [Route("Disbursements/{id:guid}/{DisbursementDate:datetime}")]
        public HttpResponseMessage GetCustomerWithExpenses(Guid? id, DateTime DisbursementDate)
        {
            try
            {
                DateTime FromDate = new DateTime(DisbursementDate.Year, DisbursementDate.Month, 1);
                DateTime ToDate = new DateTime(DisbursementDate.Year, DisbursementDate.Month, DateTime.DaysInMonth(DisbursementDate.Year, DisbursementDate.Month));
                CustomerModel customerModel = this._CustomerAppService.GetCustomerWithDisbursements(id.Value, FromDate, ToDate);
                if (customerModel == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, customerModel, _camelCasingFormatter);
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        [HttpGet]
        [Route("Expenses")]
        public HttpResponseMessage GetExpenses(string search)
        {
            try
            {
                IEnumerable<ExpenseModel> expenses = _CustomerAppService.GetExpenses(search);
                return Request.CreateResponse(HttpStatusCode.OK, expenses, _camelCasingFormatter);
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpPost]
        [Route("Disbursements/New")]
        public HttpResponseMessage AddDisbursement(DisbursementViewModel customerExpenseViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerExpenseViewModel == null && customerExpenseViewModel.DisbursementModel == null)
                        return Request.CreateResponse(HttpStatusCode.NoContent);


                    if (customerExpenseViewModel.DisbursementModel.Expense == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NoContent, customerExpenseViewModel, _camelCasingFormatter);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, _CustomerAppService.AddNewDisbursement(customerExpenseViewModel), _camelCasingFormatter);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        [HttpDelete]
        [Route("Disbursements/Delete/{id:guid}")]
        public HttpResponseMessage DeleteDisbursement(Guid id)
        {
            try
            {
                bool result = this._CustomerAppService.DeleteDisbursement(id);
                if (result)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region FEE INVOICE
        [HttpGet]
        [Route("ProcessFeeInvoice", Name = "ProcessFeeInvoice")]
        public HttpResponseMessage ProcessFeeInvoice(DateTime invoiceProcessedDate)
        {
            try
            {
                if (_InvoiceAppService.ProcessInvoice(invoiceProcessedDate.Date))
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invoices already have beed generated! Please change the Invoice Generated Date");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("FeeInvoicesSearch", Name = "FeeInvoicesSearch")]
        public HttpResponseMessage GetInvoices(string PayerName, string CustomerName, string CustomerNo, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize)
        {
            try
            {
                int TotalRowCount = 0;
                int customerNo = 0;
                int.TryParse(CustomerNo, out customerNo);
                if (!InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    InvoiceToDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
                else if (InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceToDate = InvoiceFromDate.Value.AddMonths(1).AddDays(-1);
                }
                else if (!InvoiceFromDate.HasValue && InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(InvoiceToDate.Value.Year, InvoiceToDate.Value.Month, 1);
                }

                IEnumerable<InvoiceModel> invoices = this._InvoiceAppService.GetInvoices(PayerName, CustomerName, customerNo, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("FeeInvoicesSearch",
                        new
                        {
                            PayerName = string.IsNullOrEmpty(PayerName) ? " " : PayerName,
                            CustomerName = string.IsNullOrEmpty(CustomerName) ? " " : CustomerName,
                            CustomerNo = string.IsNullOrEmpty(CustomerNo) ? " " : CustomerNo,
                            InvoiceFromDate = InvoiceFromDate.Value.Date,
                            InvoiceToDate = InvoiceToDate.Value.Date
                        }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("FeeInvoicesSearch", new { page = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("FeeInvoicesSearch", new { page = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, invoices, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Invoices")]
        public HttpResponseMessage GetInvoices(string InvoiceNo)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoices(InvoiceNo), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Invoices/{InvoiceNo:int}")]
        public HttpResponseMessage GetInvoice(int InvoiceNo)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoice(InvoiceNo), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Invoices")]
        public HttpResponseMessage GetInvoices(DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize)
        {
            try
            {
                if (!InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    InvoiceToDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
                else if (InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceToDate = new DateTime(InvoiceFromDate.Value.Year, InvoiceFromDate.Value.Month, 1);
                    InvoiceToDate = InvoiceToDate.Value.AddMonths(11);
                }
                else if (!InvoiceFromDate.HasValue && InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(InvoiceToDate.Value.Year, InvoiceToDate.Value.Month, 1);
                    InvoiceFromDate = InvoiceFromDate.Value.AddMonths(-11);
                }
                int TotalRowCount = 0;
                IEnumerable<InvoiceModel> invoices = this._InvoiceAppService.GetInvoices(InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("Invoices", new { InvoiceFromDate = InvoiceFromDate.Value.Date, InvoiceToDate = InvoiceToDate.Value.Date }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("Invoices", new { pageIndex = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("Invoices", new { pageIndex = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, invoices, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Invoices", Name = "ProcessedInvoices")]
        public HttpResponseMessage GetInvoices(DateTime ProcessedDate, int pageIndex, int pageSize)
        {
            try
            {
                int TotalRowCount = 0;
                IEnumerable<InvoiceModel> invoices = this._InvoiceAppService.GetInvoices(ProcessedDate.Date, pageIndex, pageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("ProcessedInvoices", new { ProcessedDate = ProcessedDate.Date }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("ProcessedInvoices", new { ProcessedDate = ProcessedDate.Date, page = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("ProcessedInvoices", new { ProcessedDate = ProcessedDate.Date, page = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, invoices);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("InvoicesByCustomer")]
        public HttpResponseMessage GetInvoices(Guid CustomerId, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize)
        {
            try
            {
                if (!InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    InvoiceToDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
                else if (InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceToDate = new DateTime(InvoiceFromDate.Value.Year, InvoiceFromDate.Value.Month, 1);
                    InvoiceToDate = InvoiceToDate.Value.AddMonths(11);
                }
                else if (!InvoiceFromDate.HasValue && InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(InvoiceToDate.Value.Year, InvoiceToDate.Value.Month, 1);
                    InvoiceFromDate = InvoiceFromDate.Value.AddMonths(-11);
                }
                int TotalRowCount = 0;
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("Invoices", new { CustomerId = CustomerId, InvoiceFromDate = InvoiceFromDate.Value.Date, InvoiceToDate = InvoiceToDate.Value.Date }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("Invoices", new { page = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("Invoices", new { page = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoicesByCustomer(CustomerId, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("InvoicesByFee")]
        public HttpResponseMessage GetInvoicesByFee(Guid FeeId, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageCount)
        {
            try
            {
                if (!InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(DateTime.Now.Year, 1, 1);
                    InvoiceToDate = new DateTime(DateTime.Now.Year, 12, 31);
                }
                else if (InvoiceFromDate.HasValue && !InvoiceToDate.HasValue)
                {
                    InvoiceToDate = new DateTime(InvoiceFromDate.Value.Year, InvoiceFromDate.Value.Month, 1);
                    InvoiceToDate = InvoiceToDate.Value.AddMonths(11);
                }
                else if (!InvoiceFromDate.HasValue && InvoiceToDate.HasValue)
                {
                    InvoiceFromDate = new DateTime(InvoiceToDate.Value.Year, InvoiceToDate.Value.Month, 1);
                    InvoiceFromDate = InvoiceFromDate.Value.AddMonths(-11);
                }
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoicesByFee(FeeId, InvoiceFromDate.Value.Date, InvoiceToDate.Value.Date, pageIndex, pageCount), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpGet]
        [Route("Invoices", Name = "PaidInvoices")]
        public HttpResponseMessage GetInvoices(bool IsPaid, int pageIndex, int pageSize)
        {
            try
            {
                IEnumerable<InvoiceModel> invoices;
                int TotalRowCount = 0;
                invoices = this._InvoiceAppService.GetInvoices(IsPaid, null, null, pageIndex, pageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = pageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = pageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / pageSize),
                        PageLink = urlHelper.Link("PaidInvoices", new { IsPaid = IsPaid }),
                        PrevPageLink = pageIndex > 0 ? urlHelper.Link("PaidInvoices", new { IsPaid = IsPaid, pageIndex = pageIndex - 1, pageSize = pageSize }) : "",
                        NextPageLink = pageIndex < (int)Math.Ceiling((double)TotalRowCount / pageSize) - 1 ? urlHelper.Link("PaidInvoices", new { IsPaid = IsPaid, pageIndex = pageIndex + 1, pageSize = pageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, invoices, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        [Route("Invoices", Name = "InvoicesForPayment")]
        public HttpResponseMessage GetInvoices(Guid PayerId, DateTime PayDate, int PageIndex, int PageSize)
        {
            try
            {
                IEnumerable<InvoiceModel> invoices;
                int TotalRowCount = 0;
                invoices = this._InvoiceAppService.GetInvoicesForPayment(PayerId, PayDate.Date);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                        PageLink = urlHelper.Link("InvoicesForPayment", new { PayerId = PayerId }),
                        PrevPageLink = PageIndex > 0 ? urlHelper.Link("InvoicesForPayment", new { PayerId = PayerId, pageIndex = PageIndex - 1, pageSize = PageSize }) : "",
                        NextPageLink = PageIndex < (int)Math.Ceiling((double)TotalRowCount / PageSize) - 1 ? urlHelper.Link("InvoicesForPayment", new { PayerId = PayerId, pageIndex = PageIndex + 1, pageSize = PageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, invoices, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Invoice", Name = "InvoiceForPayment")]
        public HttpResponseMessage GetInvoice(int InvoiceNo, DateTime PayDate)
        {
            try
            {

                InvoiceModel invoice = this._InvoiceAppService.GetInvoice(InvoiceNo);
                if (invoice.InvoiceDate.Date <= PayDate.Date && invoice.DueAmount > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, invoice, _camelCasingFormatter);
                }
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        public HttpResponseMessage UpdateInvoiceStatus(int InvoiceNo, Finance.Web.Model.InvoiceStatus invoiceStatus)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.UpdateInvoiceStatus(InvoiceNo, invoiceStatus), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Invoice", Name = "Invoice")]
        [HttpPost]
        public HttpResponseMessage UpdateInvoice(InvoiceModel invoice)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.UpdateInvoice(invoice), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        #endregion

        #region FNC INVOICE

        #endregion

        #region INVOICE ARTICLE
        [Route("InvoiceArticleTemplates/{TemplateId:guid}")]
        public HttpResponseMessage GetInvoiceArticleTemplate(Guid TemplateId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoiceArticleTemplate(TemplateId), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("InvoiceArticleTemplates", Name = "InvoiceArticleTemplates")]
        public HttpResponseMessage GetInvoiceArticleTemplates(int PageIndex, int PageSize)
        {
            try
            {
                int TotalRowCount = 0;
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                        PageLink = urlHelper.Link("InvoiceArticleTemplates", new { }),
                        PrevPageLink = PageIndex > 0 ? urlHelper.Link("InvoiceArticleTemplates", new { page = PageIndex - 1, pageSize = PageSize }) : "",
                        NextPageLink = PageIndex < (int)Math.Ceiling((double)TotalRowCount / PageSize) - 1 ? urlHelper.Link("InvoiceArticleTemplates", new { page = PageIndex + 1, pageSize = PageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoiceArticleTemplates(PageIndex, PageSize, out TotalRowCount), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("InvoiceArticleTemplates", Name = "InvoiceArticleTemplatesByTemplateName")]
        public HttpResponseMessage GetInvoiceArticleTemplates(string TemplateName, int PageIndex, int PageSize)
        {
            try
            {
                int TotalRowCount = 0;
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                        PageLink = urlHelper.Link("InvoiceArticleTemplatesByTemplateName", new { TemplateName = TemplateName }),
                        PrevPageLink = PageIndex > 0 ? urlHelper.Link("InvoiceArticleTemplatesByTemplateName", new { TemplateName = TemplateName, page = PageIndex - 1, pageSize = PageSize }) : "",
                        NextPageLink = PageIndex < (int)Math.Ceiling((double)TotalRowCount / PageSize) - 1 ? urlHelper.Link("InvoiceArticleTemplatesByTemplateName", new { TemplateName = TemplateName, page = PageIndex + 1, pageSize = PageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.GetInvoiceArticleTemplates(TemplateName, PageIndex, PageSize, out TotalRowCount), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("InvoiceArticleTemplate", Name = "InvoiceArticleTemplate")]
        [HttpPost]
        public HttpResponseMessage UpdateInvoiceArticleTemplate(InvoiceArticleTemplateModel InvoiceArticleTemplate)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.UpdateInvoiceArticleTemplate(InvoiceArticleTemplate), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("InvoiceArticleTemplateDelete", Name = "InvoiceArticleTemplateDelete")]
        [HttpPost]
        public HttpResponseMessage DeleteInvoiceArticleTemplate(Guid TemplateId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._InvoiceAppService.DeleteInvoiceArticleTemplate(TemplateId), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region RECEIPT
        [Route("Receipt", Name = "GetReceipt")]
        public HttpResponseMessage GetReceipt(int ReceiptNo)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, this._FinanceTransactionAppService.GetReceipt(ReceiptNo), _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("Receipts", Name = "Receipts")]
        public HttpResponseMessage GetReceipts(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize)
        {
            try
            {
                IEnumerable<ReceiptModel> receipts;
                int TotalRowCount = 0;
                receipts = this._FinanceTransactionAppService.GetReceipts(PayerId, FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                        PageLink = urlHelper.Link("Receipts", new { PayerId = PayerId, FromDate = FromDate, ToDate = ToDate }),
                        PrevPageLink = PageIndex > 0 ? urlHelper.Link("Receipts", new { PayerId = PayerId, FromDate = FromDate, ToDate = ToDate, PageIndex = PageIndex - 1, PageSize = PageSize }) : "",
                        NextPageLink = PageIndex < (int)Math.Ceiling((double)TotalRowCount / PageSize) - 1 ? urlHelper.Link("Receipts", new { PayerId = PayerId, FromDate = FromDate, ToDate = ToDate, PageIndex = PageIndex + 1, PageSize = PageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, receipts, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [HttpPost]
        [Route("Receipt", Name = "Receipt")]
        public HttpResponseMessage UpdateReceipt(ReceiptModel receipt)
        {
            try
            {
                if (receipt.Id == Guid.Empty)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _FinanceTransactionAppService.CreateReceipt(receipt), _camelCasingFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion

        #region CREDIT NOTE
        [Route("CreditNote", Name = "GetCreditNote")]
        public HttpResponseMessage GetCreditNote(int CreditNoteNo)
        {
            try
            {
                CreditNoteModel creditNote = this._FinanceTransactionAppService.GetCreditNote(CreditNoteNo);
                return Request.CreateResponse(HttpStatusCode.OK, creditNote, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("CreditNote", Name = "CreditNote")]
        [HttpPost]
        public HttpResponseMessage UpdateCreditNote(CreditNoteModel creditNote)
        {
            try
            {
                if (creditNote.Id == null || creditNote.Id == Guid.Empty)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _FinanceTransactionAppService.CreateCreditNote(creditNote), _camelCasingFormatter);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        [Route("CreditNotes", Name = "CreditNotes")]
        public HttpResponseMessage GetCreditNotes(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize)
        {
            try
            {
                //int TotalRow = 0;
                //IEnumerable<CreditNoteModel> credits = new List<CreditNoteModel>();
                //if (PayerId.HasValue)
                //    credits = _FinanceTransactionAppService.GetCreditNotesForReceiptsByPayer(PayerId.Value, DateTime.Now.Date, 0, 100, out TotalRow);
                IEnumerable<CreditNoteModel> creditNotes;
                int TotalRowCount = 0;
                creditNotes = this._FinanceTransactionAppService.GetCreditNotes(PayerId, FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                        PageLink = urlHelper.Link("Receipts", new { PayerId = PayerId, FromDate = FromDate, ToDate = ToDate }),
                        PrevPageLink = PageIndex > 0 ? urlHelper.Link("Receipts", new { PayerId = PayerId, FromDate = FromDate, ToDate = ToDate, PageIndex = PageIndex - 1, PageSize = PageSize }) : "",
                        NextPageLink = PageIndex < (int)Math.Ceiling((double)TotalRowCount / PageSize) - 1 ? urlHelper.Link("Receipts", new { PayerId = PayerId, FromDate = FromDate, ToDate = ToDate, PageIndex = PageIndex + 1, PageSize = PageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, creditNotes, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }

        [Route("CreditNotesForReceipt", Name = "CreditNotesForReceipt")]
        public HttpResponseMessage GetCreditNotesForReceipt(Guid PayerId, DateTime ReceiptDate, int PageIndex, int PageSize)
        {
            try
            {
                IEnumerable<CreditNoteModel> creditNotes;
                int TotalRowCount = 0;
                creditNotes = this._FinanceTransactionAppService.GetCreditNotesForReceiptsByPayer(PayerId, ReceiptDate.Date, PageIndex, PageSize, out TotalRowCount);
                var urlHelper = new UrlHelper(this.ActionContext);
                this.ActionContext.HttpContext.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        PageSize = PageSize,
                        TotalCount = TotalRowCount,
                        PageIndex = PageIndex,
                        TotalPages = (int)Math.Ceiling((double)TotalRowCount / PageSize),
                        PageLink = urlHelper.Link("CreditNotesForReceipt", new { PayerId = PayerId, ReceiptDate = ReceiptDate }),
                        PrevPageLink = PageIndex > 0 ? urlHelper.Link("CreditNotesForReceipt", new { PayerId = PayerId, ReceiptDate = ReceiptDate, PageIndex = PageIndex - 1, PageSize = PageSize }) : "",
                        NextPageLink = PageIndex < (int)Math.Ceiling((double)TotalRowCount / PageSize) - 1 ? urlHelper.Link("CreditNotesForReceipt", new { PayerId = PayerId, ReceiptDate = ReceiptDate, PageIndex = PageIndex + 1, PageSize = PageSize }) : ""
                    }
                    ));
                return Request.CreateResponse(HttpStatusCode.OK, creditNotes, _camelCasingFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException != null ? ex.InnerException : ex);
            }
        }
        #endregion
    }
}