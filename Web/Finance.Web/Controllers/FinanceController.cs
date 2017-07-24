using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Presentation.WebAPIProxy.Serialization;
using Finance.Web.JsonToModelConverters;
using Finance.WebAPIProxy.Proxies;
using Finance.Web.Model;
using Finance.Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance.Web.Controllers
{
#if (!DEBUG)
    [Authorize(Roles = "Administrator, Manager,User")]
#endif
    //[Authorize(Roles = "Administrator, Manager,User")]
    public class FinanceController : BaseController<Business>
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            ViewBag.MainHeading = string.Format("{0}{1}", "Finance Management", this.Business == null ? string.Empty : string.Format(" - {0}", this.Business.Name));
        }

        #region CTOR
        public FinanceController(BusinessService businessService, IOptions<AppSettings> appSettings) : base(businessService, appSettings)
        {

            var deseralizeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto
            };
            deseralizeSettings.Converters.Add(new FinancialTransactionModelConverter());
            //deseralizeSettings.Converters.Add(new InvoiceDetailModelConverter());
            _serializer = new JsonNetSerialization(deseralizeSettings);
        }
        #endregion

        #region DASH BOARD
        public ActionResult Index(DashboardViewModel dashboard)
        {
            if (dashboard == null)
                dashboard = new DashboardViewModel();
            if (dashboard.CustomerPagination == null)
            {
                dashboard.CustomerPagination = new PaginationHeader();
                dashboard.CustomerPagination.PageSize = 25;
            }
            //dashboard.Business = this.Business;
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                dashboard = proxy.GetDashboardAsync(dashboard);
            }

            ViewBag.TitleHeader = "Dashboard";
            return View(dashboard);
        }
        #endregion

        #region CUSTOMERS
        [HttpGet]
        public ActionResult Customer(Guid? CustomerId)
        {
            if (CustomerId == Guid.Empty || CustomerId == null)
                return View("Customer", new CustomerModel());
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                CustomerModel customer = proxy.GetCustomer(CustomerId);
                if (customer == null || customer.Id == Guid.Empty)
                    new CustomerModel();
                return View("Customer", customer);
            }
        }
        [HttpPost]
        public ActionResult Customer(CustomerModel Customer)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                foreach (Microsoft.AspNetCore.Http.IFormFile upload in Request.Form.Files)
                {
                    if (Request.Form.Files.Count == 0) continue;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        upload.CopyTo(ms);
                        if (Customer != null)
                            Customer.PersonalInfo.PictureRawPhoto = ms.ToArray();
                    }
                }
                Customer.BusinessId = Business.Id;
                CustomerModel customer = proxy.SaveCustomer(Customer);
                if (customer == null || customer.Id == Guid.Empty)
                    return View("Customer", Customer);
                return View("Customer", customer);
            }
        }
        [HttpGet]
        public ActionResult CustomersWithFees(int PageIndex, int PageSize)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                return View("_CustomersWithFees", proxy.GetCustomersWithFeesAsync(PageIndex, PageSize));
            }
        }
        [Route("CustomersWithFees/PDF", Name = "CustomersWithFeesPDF")]
        public ActionResult CustomersWithFeesPdf()
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                CustomerViewModel customerView = proxy.GetCustomersWithFeesAsync(0, int.MaxValue);
                customerView.BusinessName = Business.Name;
                //return new Rotativa.MVC.ViewAsPdf("_CustomersWithFees", customerView)
                //{
                //    FileName = string.Format("CustomersWithFees_{0}.pdf", DateTime.Now.Date),
                //    RotativaOptions = new Rotativa.Core.DriverOptions
                //    {
                //        PageSize = Size.A4,
                //        PageOrientation = Orientation.Landscape,
                //        PageMargins = new Rotativa.Core.Options.Margins { Left = 5, Right = 5, Bottom = 0 }
                //        //,CustomSwitches = customSwitches
                //    }
                //};
            }
            return View();
        }
        [HttpGet]
        [Route("CustomerList/PDF", Name = "CustomerListPDF")]
        public ActionResult CustomerListPDF(CustomerViewModel CustomerView)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                return View("Customer", CustomerView);
            }
        }

        #endregion

        #region PAYERS
        [HttpGet]
        public ActionResult Payer(Guid? Id)
        {
            PayerModel payer = new PayerModel();
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                if (Id == null)
                {
                    // Get all Payers
                    payer.Payers = proxy.GetPayersAsync();
                    if (payer.Payers == null || payer.Payers.Count() == 0)
                    {
                        payer.PersonalContact = new ContactModel();
                        payer.FeeInvoiceBillingContact = new ContactModel();
                        payer.FeeInvoiceDeliveryContact = new ContactModel();
                        payer.DisbursementInvoiceBillingContact = new ContactModel();
                        payer.DisbursementInvoiceDeliveryContact = new ContactModel();
                    }
                    ViewBag.TitleHeader = "Payers";
                }
                else if (Id == Guid.Empty)
                {
                    payer.PersonalContact = new ContactModel();
                    payer.FeeInvoiceBillingContact = new ContactModel();
                    payer.FeeInvoiceDeliveryContact = new ContactModel();
                    payer.DisbursementInvoiceBillingContact = new ContactModel();
                    payer.DisbursementInvoiceDeliveryContact = new ContactModel();
                    ViewBag.TitleHeader = "New Payer";
                }
                else
                {
                    payer = proxy.GetPayerAsync(Id.Value);
                    if (payer.FeeInvoiceBillingContact == null)
                        payer.FeeInvoiceBillingContact = new ContactModel();
                    if (payer.FeeInvoiceDeliveryContact == null)
                        payer.FeeInvoiceDeliveryContact = new ContactModel();
                    if (payer.DisbursementInvoiceBillingContact == null)
                        payer.DisbursementInvoiceBillingContact = new ContactModel();
                    if (payer.DisbursementInvoiceDeliveryContact == null)
                        payer.DisbursementInvoiceDeliveryContact = new ContactModel();
                    ViewBag.TitleHeader = payer.Name;
                }
            }
            return View("Payer", payer);
        }
        [HttpPost]
        public ActionResult Payer(PayerModel Payer)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                Payer = proxy.UpdatePayerAsync(Payer);
                //return View("Payer", Payer);
                ViewBag.TitleHeader = Payer.Name;
                return RedirectToAction("Payer", new { Id = Payer.Id });
            }
        }
        #endregion

        #region EXPENSE
        [HttpGet]
        public ActionResult Disbursement(Guid? id, DateTime? expenseDate)
        {
            DisbursementViewModel customerExpenseViewModel = new DisbursementViewModel();
            CustomerModel customer;
            if (expenseDate == null)
            {
                expenseDate = DateTime.Now.Date;
            }
            if (id == null)
            {
                customer = new CustomerModel();
                customer.PersonalInfo = new ContactModel();
            }
            else
            {
                using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
                {
                    customer = proxy.GetCustomerWithExpensesAsync(id.ToString(), expenseDate.Value);
                }
            }
            customerExpenseViewModel.DisbursementModel = new DisbursementModel
            {
                Id = Guid.NewGuid(),
                DisbursementDate = expenseDate.Value.Date,
                Expense = new ExpenseModel()
            };
            customerExpenseViewModel.Customer = customer;

            return View("Disbursement", customerExpenseViewModel);
        }
        [HttpPost]
        public ActionResult Disbursement(Guid? id, DisbursementViewModel customerExpenseViewModel)
        {
            foreach (Microsoft.AspNetCore.Http.IFormFile upload in Request.Form.Files)
            {
                if (Request.Form.Files.Count == 0) continue;
                using (MemoryStream ms = new MemoryStream())
                {
                    upload.CopyTo(ms);
                    customerExpenseViewModel.DisbursementModel.Document = ms.ToArray();
                }
            }
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                customerExpenseViewModel.CustomerId = id.Value;
                customerExpenseViewModel.DisbursementModel.PayerId = customerExpenseViewModel.DisbursementModel.Payer != null ? customerExpenseViewModel.DisbursementModel.Payer.Id : Guid.Empty;
                customerExpenseViewModel = proxy.AddNewExpenseForCustomerAsync(customerExpenseViewModel);
            }
            customerExpenseViewModel.DisbursementModel = new DisbursementModel
            {
                Id = Guid.NewGuid(),
                DisbursementDate = DateTime.Now.Date,
                Expense = new ExpenseModel()
            };
            return View("Disbursement", customerExpenseViewModel);
        }
        #endregion

        #region FEES
        public ActionResult CustomerFee(Guid? CustomerId)
        {
            if (CustomerId == null)
                return RedirectToAction("Index");
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                CustomerModel customer = proxy.GetCustomer(CustomerId);
                ViewBag.TitleHeader = customer.PersonalInfo.FullName;
                return View("Fee", customer);
            }
        }
        public ActionResult Fee(Guid? FeeId)
        {
            if (FeeId == null)
                return RedirectToAction("Index");
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                return View(proxy.GetCustomer(FeeId));
            }
        }
        #endregion

        #region FINANCIAL TRANSACTIONS

        #region INVOICES
        [HttpGet]
        [Route("UnpaidInvoices")]
        public ActionResult GetUnpaidInvoices()
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                return PartialView("_InvoiceList", proxy.GetUnPaidInvoices());
            }
        }
        #region FEE
        [HttpGet]
        [Route("Invoices")]
        public ActionResult Invoices(string InvoiceNoSearch)
        {
            if (string.IsNullOrEmpty(InvoiceNoSearch))
                return RedirectToAction("Index");
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                IEnumerable<InvoiceModel> invoices = proxy.GetInvoices(InvoiceNoSearch);
                if (invoices != null && invoices.Count() > 1)
                    return View("FeeInvoicePrintView", invoices);
                else if (invoices != null && invoices.Count() == 1)
                    return View("FeeInvoiceView", invoices.First());
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("PrintPreviewInvoices")]
        public ActionResult PrintPreviewInvoices(string InvoiceNoSearch)
        {
            if (string.IsNullOrEmpty(InvoiceNoSearch))
                return RedirectToAction("Index");
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                IEnumerable<InvoiceModel> invoices = proxy.GetInvoices(InvoiceNoSearch);
                if (invoices != null && invoices.Count() > 0)
                    return View("FeeInvoicePrintView", invoices);
            }
            return RedirectToAction("Index");
        }
        [Route("PrintFeeInvoices")]
        public ActionResult PrintFeeInvoices(string InvoiceNoSearch)
        {
            if (string.IsNullOrEmpty(InvoiceNoSearch))
                return RedirectToAction("Index");
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                IEnumerable<InvoiceModel> invoices = proxy.GetInvoices(InvoiceNoSearch);
                if (invoices != null && invoices.Count() > 0)
                    return View("FeeInvoicePrint", invoices);
            }
            return RedirectToAction("Index");
        }
        [Route("FeeInvoice")]
        public ActionResult FeeInvoice(InvoiceViewModel invoiceViewModel)
        {
            if (invoiceViewModel == null)
                invoiceViewModel = new InvoiceViewModel();
            invoiceViewModel.InvoiceType = InvoiceTypeModel.Fee;
            return View("FeeInvoiceSearch", invoiceViewModel);
        }
        [Route("FNCInvoice")]
        public ActionResult FNCInvoice(InvoiceViewModel invoiceViewModel)
        {
            if (invoiceViewModel == null)
                invoiceViewModel = new InvoiceViewModel();
            invoiceViewModel.InvoiceType = InvoiceTypeModel.FNC;
            return View("FeeInvoiceSearch", invoiceViewModel);
        }
        [HttpGet]
        [Route("FeeInvoiceSearchByNo")]
        public ActionResult FeeInvoiceSearchByNo(int InvoiceNoSearch)
        {
            if (InvoiceNoSearch > 0)
            {
                using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
                {
                    InvoiceModel invoice = proxy.GetInvoice(InvoiceNoSearch);
                    if (invoice == null)
                        return View("FeeInvoiceSearch", new InvoiceViewModel());
                    return View("FeeInvoiceView", invoice);
                }
            }
            return RedirectToAction("FeeInvoice");
        }
        [HttpGet]
        [Route("FeeInvoiceSearch")]
        public ActionResult FeeInvoiceSearch(InvoiceViewModel feeInvoiceSearch)
        {
            if (feeInvoiceSearch.IsPdfPrint)
            {
                feeInvoiceSearch.IsPdfPrint = false;
                return PrintFeeInvoiceSearchPDF(feeInvoiceSearch);
            }
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                if (feeInvoiceSearch.Pagination == null)
                {
                    feeInvoiceSearch.Pagination = new PaginationHeader();
                    feeInvoiceSearch.Pagination.PageSize = 25;
                }
                feeInvoiceSearch.Invoices = new List<InvoiceModel>();
                feeInvoiceSearch.Invoices = proxy.GetInvoices(feeInvoiceSearch);
                //feeInvoiceSearch.PageIndex = feeInvoiceSearch.Pagination.PageIndex + 1;
                return View("FeeInvoiceSearch", feeInvoiceSearch);
            }
        }
        #endregion
        #region EXPENSES
        [Route("DisbursementInvoice")]
        public ActionResult DisbursementInvoice(InvoiceViewModel invoiceViewModel)
        {
            if (invoiceViewModel == null)
                invoiceViewModel = new InvoiceViewModel();
            invoiceViewModel.InvoiceType = InvoiceTypeModel.Expenses;
            return View();
        }
        [HttpPost]
        public ActionResult DisbursementInvoice(DateTime? FromDate, DateTime? ToDate)
        {
            return View();
        }
        [HttpPost]
        public ActionResult GenerateInvoice(DateTime? FromDate, DateTime? ToDate)
        {
            return View();
        }
        #endregion
        #endregion

        #region PDF INVOICES
        [Route("PrintPDFFeeInvoices")]
        public IActionResult PrintPDFFeeInvoices(string InvoiceNoSearch)
        {
            if (string.IsNullOrEmpty(InvoiceNoSearch))
                return RedirectToAction("Index");
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                IEnumerable<InvoiceModel> invoices = proxy.GetInvoices(InvoiceNoSearch);
                if (invoices != null && invoices.Count() > 0)
                {
                    //opt.FormatterMappings.SetMediaTypeMappingForFormat("pdf", new MediaTypeHeaderValue("application/pdf"));
                    return Content("FeeInvoicePrint", "application/pdf");
                    //return new Rotativa.MVC.ViewAsPdf("FeeInvoicePrint", invoices)
                    //{
                    //    FileName = string.Format("Invoice_{0}.pdf", InvoiceNoSearch),
                    //    RotativaOptions = new Rotativa.Core.DriverOptions
                    //    {
                    //        PageSize = Size.A4,
                    //        PageOrientation = Orientation.Portrait,
                    //        PageMargins = new Rotativa.Core.Options.Margins { Left = 10, Right = 10, Bottom = 0 }
                    //        //,CustomSwitches = customSwitches
                    //    }
                    //};
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("PrintFeeInvoiceSearchPDF")]
        public ActionResult PrintFeeInvoiceSearchPDF(InvoiceViewModel feeInvoiceSearch)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                if (feeInvoiceSearch.Pagination == null)
                {
                    feeInvoiceSearch.Pagination = new PaginationHeader();
                    feeInvoiceSearch.Pagination.PageSize = 25;
                }
                feeInvoiceSearch.Invoices = new List<InvoiceModel>();
                feeInvoiceSearch.Invoices = proxy.GetInvoices(feeInvoiceSearch);

                //return new Rotativa.MVC.ViewAsPdf("_InvoiceSearchPrint", feeInvoiceSearch)
                //{
                //    FileName = string.Format("InvoiceSearchPrint_{0}.pdf", DateTime.Now.ToString("yyyy-MM-dd")),
                //    RotativaOptions = new Rotativa.Core.DriverOptions
                //    {
                //        PageSize = Size.A4,
                //        PageOrientation = Orientation.Portrait,
                //        PageMargins = new Rotativa.Core.Options.Margins { Left = 10, Right = 10, Bottom = 0 }
                //        //,CustomSwitches = customSwitches
                //    }
                //};
            }
            return View();
        }
        #endregion

        public ActionResult Receipt(ReceiptViewModel receiptViewModel)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                if (receiptViewModel.Pagination == null)
                {
                    receiptViewModel.Pagination = new PaginationHeader();
                    receiptViewModel.Pagination.PageSize = 25;
                    receiptViewModel.Reset = true;
                }
                if (receiptViewModel.Reset)
                {
                    receiptViewModel.PayerId = null;
                    receiptViewModel.PayerName = string.Empty;
                    receiptViewModel.ToDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    DateTime ToDate = DateTime.Now.Date.AddMonths(-1).AddDays(1);
                    receiptViewModel.FromDate = ToDate.ToString("dd/MM/yyyy");
                    receiptViewModel.Reset = false;
                }
                receiptViewModel.Receipts = proxy.GetReceipts(receiptViewModel);

                return View(receiptViewModel);
            }
        }
        public ActionResult CreditNote(CreditNoteViewModel creditNoteViewModel)
        {
            using (FinanceApiProxy proxy = new FinanceApiProxy(HttpContext, ViewBag.FinanceApiService, _serializer))
            {
                if (creditNoteViewModel.Pagination == null)
                {
                    creditNoteViewModel.Pagination = new PaginationHeader();
                    creditNoteViewModel.Pagination.PageSize = 25;
                    creditNoteViewModel.Reset = true;
                }
                if (creditNoteViewModel.Reset)
                {
                    creditNoteViewModel.PayerId = null;
                    creditNoteViewModel.PayerName = string.Empty;
                    creditNoteViewModel.ToDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                    DateTime ToDate = DateTime.Now.Date.AddMonths(-1).AddDays(1);
                    creditNoteViewModel.FromDate = ToDate.ToString("dd/MM/yyyy");
                    creditNoteViewModel.Reset = false;
                }
                creditNoteViewModel.CreditNotes = proxy.GetCreditNotes(creditNoteViewModel);

                return View(creditNoteViewModel);
            }
        }
        #endregion
    }
}