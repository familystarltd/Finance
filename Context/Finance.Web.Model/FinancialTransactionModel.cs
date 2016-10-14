using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Web.Model
{
    public enum TransactionStatusModel : int
    {
        New = 1,
        Approved = 2,
        Void = 3,
        Processed = 4
    }
    public enum FinancialTransactionTypeModel : int
    {
        FeeInvoice = 1,
        DisbursementInvoice = 2,
        Receipt = 3,
        CreditNote = 4,
        BadDebt = 5
    }
    public class FinancialTransactionModel
    {
        public Guid Id { get; set; }
        public string RefPrefix { get; set; }
        public FinancialTransactionTypeModel FinancialTransactionType { get; set; }
        public string Discriminator { get; set; }
        public TransactionStatusModel TransactionStatus { get; set; }
        /// <summary>
        /// Transaction ContactReference, which is Getting from child objects (e.g. from derived object 'Invoice', InvoiceNo could be a Transaction ContactReference)
        /// </summary>
        public int TransactionRef { get; set; }
        /// <summary>
        /// Get or Set the Date of the Transaction.
        /// </summary>
        public DateTime ProcessedDate { get; set; }
        /// <summary>
        /// Get or Set the amount to this Transaction.
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Get or Set the description of this Transaction.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or Set the Payer which is relates this Transaction.
        /// </summary>
        public PayerModel Payer { get; set; }
        public Guid PayerId { get; set; }
        /// <summary>
        /// Get or Set the associated Customer for this Fee Invoice
        /// </summary>
        public CustomerModel Customer { get; set; }
        public Guid? CustomerId { get; set; }

    }
}
