using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Service
{
    public interface IInvoiceAppService
    {
        #region FEE INVOICES
        bool ProcessInvoice(DateTime invoiceProcessedDate);
        #endregion

        #region TRANSACTIONS
        IEnumerable<FinancialTransactionModel> GetAllTransactions(DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount);
        #endregion

        #region INVOICES
        IEnumerable<InvoiceModel> GetInvoices(string InvoiceNos);
        IEnumerable<InvoiceModel> GetInvoices(DateTime ProcessedDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceModel> GetInvoices(bool IsPaid, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceModel> GetInvoices(DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceModel> GetInvoices(InvoiceStatus invoiceStatus, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceModel> GetInvoices(string FunderName, string CustomerName, int CustomerNo, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceModel> GetInvoicesByCustomer(Guid CustomerId, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize);
        IEnumerable<InvoiceModel> GetInvoicesByFee(Guid FeeId, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize);
        IEnumerable<InvoiceModel> GetInvoicesForPayment(Guid FunderId, DateTime ReceiptDate);
        InvoiceModel GetInvoice(Guid Invoiceid);
        InvoiceModel GetInvoice(int InvoiceNo);
        InvoiceModel UpdateInvoiceStatus(int InvoiceNo, InvoiceStatus invoiceStatus);
        InvoiceModel UpdateInvoice(InvoiceModel invoice);        
        void GetNoOfInvoicesByStatus(out int New, out int Approved, out int Paid, out int Deleted, out int Cancelled, out int UnApproved, out int UnPaid);
        DateTime? GetFeeInvoiceProcessedMaxDate();
        #endregion

        #region ARTICLE TEMPLATE
        IEnumerable<InvoiceArticleTemplateModel> GetInvoiceArticleTemplates(int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceArticleTemplateModel> GetInvoiceArticleTemplates(string TemplateName, int pageIndex, int pageSize, out int TotalRowCount);
        InvoiceArticleTemplateModel GetInvoiceArticleTemplate(Guid TemplateId);
        InvoiceArticleTemplateModel UpdateInvoiceArticleTemplate(InvoiceArticleTemplateModel Template);
        bool DeleteInvoiceArticleTemplate(Guid TemplateId);
        #endregion
    }
}
