using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Domain;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Domain.Aggregates.DisbursementAgg;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public interface ICustomerDisbursementsInvoiceRepository : IRepository<CustomerDisbursementsInvoice>
    {
        long GetInvoiceNo();
        IEnumerable<CustomerDisbursementsInvoice> GetInvoices(Guid ResidentId, DateTime InvoiceDate, int pageIndex, int pageCount);
        CustomerDisbursementsInvoice GetInvoice(long InvoiceNo);
        IEnumerable<CustomerDisbursementsInvoice> GetInvoice(int SVN, DateTime InvoiceDate);
        IEnumerable<Disbursement> GetDisbursements(Guid ResidentId, DateTime InvoiceDate);
    }
}
