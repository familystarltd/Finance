namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System.Domain;
    using Finance.Domain.Aggregates.FeeAgg;
    using System.ComponentModel;

    /// <summary>
    /// Invoice for Fee
    /// </summary>
    public class FeeInvoice : Invoice
    {
        public override Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceType InvoiceType
        {
            get { return FinancialTransactionAgg.InvoiceType.Fee; }
            set { value = FinancialTransactionAgg.InvoiceType.Fee; }
        }
        public override FinancialTransactionType FinancialTransactionType
        {
            get { return FinancialTransactionAgg.FinancialTransactionType.FeeInvoice; }
            set { value = FinancialTransactionAgg.FinancialTransactionType.FeeInvoice; }
        }
        /// <summary>
        /// Get or Set the associated Fee for this Fee Invoice
        /// </summary>
        public Guid? FeeId { get; set; }
        public virtual Fee Fee { get; set; }
    }
}