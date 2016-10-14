using System;
using System.Collections.Generic;
using System.Domain;
using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Domain.Aggregates.DisbursementAgg;

namespace Finance.Domain.Aggregates.FinancialTransactionAgg
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