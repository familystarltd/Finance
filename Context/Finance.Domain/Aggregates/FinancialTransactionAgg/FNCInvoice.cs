namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Domain;
    using Finance.Domain.Aggregates.FeeAgg;
    using System.ComponentModel;
    /// <summary>
    /// Invoice for FNC
    /// </summary>
    public class FNCInvoice : Invoice
    {
        public override Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceType InvoiceType
        {
            get { return FinancialTransactionAgg.InvoiceType.FNC; }
            set { value = FinancialTransactionAgg.InvoiceType.FNC; }
        }
        public override FinancialTransactionType FinancialTransactionType
        {
            get { return FinancialTransactionAgg.FinancialTransactionType.FNCInvoice; }
            set { value = FinancialTransactionAgg.FinancialTransactionType.FNCInvoice; }
        }
        /// <summary>
        /// Get or Set the associated FNC for this Invoice
        /// </summary>
        public Guid? FNCId { get; set; }
        public virtual FNC FNC { get; set; }
    }
    public class FNCInvoiceDetail : InvoiceDetail
    {
        public Guid? FNCRateId { get; set; }
        public virtual FNCRate FNCRate { get; set; }
        public decimal? FNCRateOverride { get; set; }
        public int NoOfDays { get; set; }
    }
}