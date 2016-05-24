using System;
using System.Collections.Generic;
using System.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        IEnumerable<Invoice> GetInvoices(bool IsPaid, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<Invoice> GetInvoices(InvoiceStatus invoiceStatus, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<Invoice> GetInvoicesByFee(Guid FeeId, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize);
        IEnumerable<Invoice> GetInvoices(Guid FunderId, DateTime ToDate);
        IEnumerable<Invoice> GetInvoicesForProcessing(Guid FunderId, DateTime ProcessedDate);
        void GetNoOfInvoicesByStatus(out int New, out int Approved, out int Paid, out int Deleted, out int Cancelled, out int UnApproved, out int UnPaid);

        #region ARTICLE TEMPLATE
        IEnumerable<InvoiceArticleTemplate> GetInvoiceArticleTemplates(int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<InvoiceArticleTemplate> GetInvoiceArticleTemplates(string TemplateName, int pageIndex, int pageSize, out int TotalRowCount);
        InvoiceArticleTemplate GetInvoiceArticleTemplate(Guid TemplateId);
        InvoiceArticleTemplate UpdateInvoiceArticleTemplate(InvoiceArticleTemplate Template);
        bool DeleteInvoiceArticleTemplate(Guid TemplateId);
        #endregion
    }
}