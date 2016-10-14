using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Presentation.WebAPIProxy;
using System.Presentation.WebAPIProxy.Serialization;
using Finance.Web.Model;
using Microsoft.AspNetCore.Http;

namespace Finance.WebAPIProxy.Proxies
{
    /// <summary>
    /// The converter to use when deserializing animal objects
    /// </summary>
    public class FinanceApiProxy : HttpClientWrapper
    {
        #region CTOR
        public FinanceApiProxy(HttpContext httpContext, ISerialization serializer)
            : base("http://intranet.familystarltd.com/api/", httpContext, serializer)
        { }
        public FinanceApiProxy(HttpContext httpContext, string baseUrl, ISerialization serializer)
            : base(baseUrl, httpContext, serializer)
        { }
        #endregion

        #region ASYNC METHODS
        public BusinessModel BusinessSetup(Guid businessId)
        {
            try
            {
                this.SetTimeOut(TimeSpan.FromMinutes(15));
                return Task<BusinessModel>.Run(async () => await this.GET<BusinessModel>(string.Format("FinanceApi/Business/Setup/{0}", businessId))).Result;
            }
            catch
            {
                return null;
            }
        }
        public CustomerModel PostResidentAsCustomerToFinance(CustomerModel customerModel)
        {
            if (customerModel.Id == Guid.Empty)
                return null;
            try
            {
                this.SetTimeOut(TimeSpan.FromMinutes(15));
                return Task<CustomerModel>.Run(async () => await this.POST<CustomerModel>("FinanceApi/SaveCustomer", customerModel)).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<CustomerModel> GetCustomers(string searchText)
        {
            IEnumerable<CustomerModel> customers = Task<IEnumerable<CustomerModel>>.Run(async () => await this.GET<IEnumerable<CustomerModel>>(string.Format("FinanceApi/Customers/{0}", searchText))).Result;
            return customers.AsQueryable<CustomerModel>();
        }
        public CustomerModel GetCustomer(Guid? customerId)
        {
            CustomerModel customer = Task<CustomerModel>.Run(async () => await this.GET<CustomerModel>(string.Format("FinanceApi/Customers/{0}", customerId))).Result;
            return customer;
        }
        public CustomerModel SaveCustomer(CustomerModel Customer)
        {
            return Task<CustomerModel>.Run(async () => await this.POST<CustomerModel>("FinanceApi/SaveCustomer", Customer)).Result;
        }
        public DashboardViewModel GetDashboardAsync(DashboardViewModel dashboard)
        {
            DashboardViewModel dashboardViewModel = Task<DashboardViewModel>.Run(async () => await this.POST<DashboardViewModel>("Financeapi/Dashboard", dashboard)).Result;
            dashboardViewModel.CustomerPagination = this.GetHearderValues<PaginationHeader>("X-Pagination");
            return dashboardViewModel;
        }
        public CustomerViewModel GetCustomersWithFeesAsync(int PageIndex, int PageSize)
        {
            CustomerViewModel CustomerView = new CustomerViewModel();
            CustomerView.Customers = Task<IEnumerable<CustomerModel>>.Run(async () => await this.GET<IEnumerable<CustomerModel>>(string.Format("Financeapi/CustomersWithFees?PageIndex={0}&PageSize={1}", PageIndex,PageSize))).Result;
            CustomerView.Pagination = this.GetHearderValues<PaginationHeader>("X-Pagination");
            return CustomerView;
        }
        public PayerModel NewPayerAsync()
        {
            return Task<PayerModel>.Run(async () => await this.GET<PayerModel>("FinanceApi/NewPayer")).Result;
        }
        public IEnumerable<PayerModel> GetPayersAsync()
        {
            return Task<IEnumerable<PayerModel>>.Run(async () => await this.GET<IEnumerable<PayerModel>>("FinanceApi/Payers")).Result;
        }
        public PayerModel GetPayerAsync(Guid Id)
        {
            return Task<PayerModel>.Run(async () => await this.GET<PayerModel>(string.Format("FinanceApi/Payers/{0}", Id))).Result;
        }
        public PayerModel UpdatePayerAsync(PayerModel Payer)
        {
            return Task<PayerModel>.Run(async () => await this.POST<PayerModel>("FinanceApi/Payer", Payer)).Result;
        }
        public FeeModel GetFee(Guid? feeId)
        {
            return Task<FeeModel>.Run(async () => await this.GET<FeeModel>(string.Format("FinanceApi/Fees/{0}", feeId))).Result;
        }

        #region INVOICES
        public IEnumerable<InvoiceModel> GetInvoices(string invoiceNo)
        {
            return Task<IEnumerable<InvoiceModel>>.Run(async () => await this.GET<IEnumerable<InvoiceModel>>(string.Format("FinanceApi/Invoices?InvoiceNo={0}", invoiceNo))).Result;
        }
        public IEnumerable<InvoiceModel> GetUnPaidInvoices()
        {
            try

            {
                IEnumerable<InvoiceModel> invoices = Task<IEnumerable<InvoiceModel>>.Run(async () => await this.GET<IEnumerable<InvoiceModel>>("/FinanceApi/Invoices?IsPaid=false&pageIndex=0&pageSize=10000")).Result;
                return invoices;
            }
            catch
            {
                return null;
            }
        }
        public InvoiceModel GetInvoice(int invoiceNo)
        {
            return Task<InvoiceModel>.Run(async () => await this.GET<InvoiceModel>(string.Format("FinanceApi/Invoices/{0}", invoiceNo))).Result;
        }
        public IEnumerable<InvoiceModel> GetInvoices(InvoiceViewModel feeInvoiceSearch)
        {
            string urlSegment = string.Format("Financeapi/FeeInvoicesSearch?PayerName={0}&CustomerName={1}&CustomerNo={2}&InvoiceFromDate={3}&InvoiceToDate={4}&pageIndex={5}&pageSize={6}",
                feeInvoiceSearch.PayerName,
                feeInvoiceSearch.CustomerName,
                feeInvoiceSearch.CustomerNo,
                feeInvoiceSearch.InvoiceProcessedFromDate.HasValue ? feeInvoiceSearch.InvoiceProcessedFromDate.Value.Date.ToString("yyyy-MM-dd") : null,
                feeInvoiceSearch.InvoiceProcessedToDate.HasValue ? feeInvoiceSearch.InvoiceProcessedToDate.Value.Date.ToString("yyyy-MM-dd") : null,
                feeInvoiceSearch.Pagination.PageIndex,
                feeInvoiceSearch.Pagination.PageSize
                );
            IEnumerable<InvoiceModel> invoices = Task<IEnumerable<InvoiceModel>>.Run(async () => await this.GET<IEnumerable<InvoiceModel>>(urlSegment)).Result;
            feeInvoiceSearch.Pagination = this.GetHearderValues<PaginationHeader>("X-Pagination");
            return invoices;
        }
        #endregion

        #region RECEIPTS
        public IEnumerable<ReceiptModel> GetReceipts(ReceiptViewModel receiptViewModel)
        {
            DateTime fromDate;
            DateTime toDate;
            bool IsValidFromDate = DateTime.TryParse(receiptViewModel.FromDate, out fromDate);
            bool IsValidToDate = DateTime.TryParse(receiptViewModel.ToDate, out toDate);
            string urlSegment = string.Format("Financeapi/Receipts?PayerId={0}&FromDate={1}&ToDate={2}&pageIndex={3}&pageSize={4}",
                receiptViewModel.PayerId,
                IsValidFromDate ? fromDate.Date.ToString("yyyy-MM-dd") : null,
                IsValidToDate ? toDate.Date.ToString("yyyy-MM-dd") : null,
                receiptViewModel.Pagination.PageIndex,
                receiptViewModel.Pagination.PageSize
                );
            IEnumerable<ReceiptModel> receipts = Task<IEnumerable<ReceiptModel>>.Run(async () => await this.GET<IEnumerable<ReceiptModel>>(urlSegment)).Result;
            receiptViewModel.Pagination = this.GetHearderValues<PaginationHeader>("X-Pagination");
            return receipts;
        }
        #endregion

        #region CREDIT NOTES
        public IEnumerable<CreditNoteModel> GetCreditNotes(CreditNoteViewModel creditNoteViewModel)
        {
            DateTime fromDate;
            DateTime toDate;
            bool IsValidFromDate = DateTime.TryParse(creditNoteViewModel.FromDate, out fromDate);
            bool IsValidToDate = DateTime.TryParse(creditNoteViewModel.ToDate, out toDate);
            string urlSegment = string.Format("Financeapi/CreditNotes?PayerId={0}&FromDate={1}&ToDate={2}&pageIndex={3}&pageSize={4}",
                creditNoteViewModel.PayerId,
                IsValidFromDate ? fromDate.Date.ToString("yyyy-MM-dd") : null,
                IsValidToDate ? toDate.Date.ToString("yyyy-MM-dd") : null,
                creditNoteViewModel.Pagination.PageIndex,
                creditNoteViewModel.Pagination.PageSize
                );
            IEnumerable<CreditNoteModel> creditNotes = Task<IEnumerable<CreditNoteModel>>.Run(async () => await this.GET<IEnumerable<CreditNoteModel>>(urlSegment)).Result;
            creditNoteViewModel.Pagination = this.GetHearderValues<PaginationHeader>("X-Pagination");
            return creditNotes;
        }
        #endregion

        #region EXPENSES
        public CustomerModel GetCustomerWithExpensesAsync(string ID, DateTime disbursementDate)
        {
            CustomerModel customer = null;
            customer = Task<CustomerModel>.Run(async () => await this.GET<CustomerModel>(string.Format("FinanceApi/Disbursements/{0}/{1}", ID, disbursementDate.Date.ToString("yyyy-MM-dd")))).Result;
            return customer;
        }
        public DisbursementViewModel AddNewExpenseForCustomerAsync(DisbursementViewModel customerExpensesModel)
        {
            return Task<CustomerModel>.Run(async () => await this.POST<DisbursementViewModel>("FinanceApi/Disbursements/New", customerExpensesModel)).Result;
        }
        #endregion
        #endregion
    }
}