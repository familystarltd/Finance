using System;
using System.Collections.Generic;
using System.Domain;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Domain.Aggregates.DisbursementAgg;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public class CustomerDisbursementsInvoice : Entity
    {
        public long? InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public byte[] InvoiceImage { get; set; }
        public string InvoiceFileName { get; set; }
        public Customer Customer { get; set; }
        public virtual ICollection<Disbursement> Disbursements { get; set; }
    }
}