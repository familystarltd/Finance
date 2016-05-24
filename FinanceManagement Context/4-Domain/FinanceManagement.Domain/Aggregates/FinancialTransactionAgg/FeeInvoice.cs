namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using System.Domain;
    using FinanceManagement.Domain.Aggregates.FeeAgg;
    using System.ComponentModel;



    // UK Bank Holidays
    //https://www.gov.uk/bank-holidays.json

    /// <summary>
    /// Invoice for Fee
    /// </summary>
    public class FeeInvoice : Invoice
    {
        public override FinanceManagement.Domain.Aggregates.FinancialTransactionAgg.InvoiceType InvoiceType
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
        public Guid FeeId { get; set; }
        public virtual Fee Fee { get; set; }
    }
}