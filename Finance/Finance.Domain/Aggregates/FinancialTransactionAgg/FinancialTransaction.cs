using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Domain.Aggregates.FeeAgg;
using System;
using System.Collections.Generic;
using System.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    public enum TransactionStatus : int
    {
        New = 1,
        Approved = 2,
        Void = 3,
        Processed = 4
    }
    public enum FinancialTransactionType : int
    {
        FeeInvoice = 1,
        BillableExpenseInvoice = 2,
        Receipt = 3,
        CreditNote = 4,
        BadDebt = 5
    }
    /// <summary>
    /// Debits increase Expenditure, Assets, Dividends (and Credits decrease them)
    /// Credits increase Liabilities, Income. Capital (and Debits decrease them)
    /// </summary>
    
    public abstract class FinancialTransaction : Entity
    {
        const string INVOICE_PREFIX = "INV";
        const string CREDITNOTE_PREFIX = "CR";
        const string RECEIPT_PREFIX = "R";
        const string BADDEBT_PREFIX = "BD";
        public TransactionStatus TransactionStatus { get; set; }

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
        /// Get or Set the Account from Chart of Account which is relates this Transaction.
        /// </summary>
        public virtual Account Account { get; set; }
        public Guid? AccountId { get; set; }
        /// <summary>
        /// Get or Set the Funder which is relates this Transaction.
        /// </summary>
        public virtual Funder Funder { get; set; }
        public Guid? FunderId { get; set; }
        /// <summary>
        /// Get or Set the associated Customer for this Fee Invoice
        /// </summary>
        public virtual Customer Customer { get; set; }
        public Guid? CustomerId { get; set; }
        public virtual FinancialTransactionType FinancialTransactionType { get; set; }
        public virtual string Discriminator
        {
            get
            {
                switch (this.FinancialTransactionType)
                {
                    case FinancialTransactionType.FeeInvoice:
                        return "Fee Invoice";
                    case FinancialTransactionType.BillableExpenseInvoice:
                        return "Disbursements Invoice";
                    case FinancialTransactionType.Receipt:
                        return "Receipt";
                    case FinancialTransactionType.CreditNote:
                        return "Credit Note";
                    case FinancialTransactionAgg.FinancialTransactionType.BadDebt:
                        return "Bad Debt";
                }
                return "N/A";
            }
            set
            {
                switch (this.FinancialTransactionType)
                {
                    case FinancialTransactionType.FeeInvoice:
                        value = "Fee Invoice";
                        break;
                    case FinancialTransactionType.BillableExpenseInvoice:
                        value = "Disbursements Invoice";
                        break;
                    case FinancialTransactionType.Receipt:
                        value = "Receipt";
                        break;
                    case FinancialTransactionType.CreditNote:
                        value = "Credit Note";
                        break;
                    case FinancialTransactionAgg.FinancialTransactionType.BadDebt:
                        value = "Bad Debt";
                        break;
                    default:
                        value = "N/A";
                        break;
                }
            }
        }
        public string RefPrefix
        {
            get
            {
                switch (this.FinancialTransactionType)
                {
                    case FinancialTransactionType.FeeInvoice:
                        return INVOICE_PREFIX;
                    case FinancialTransactionType.BillableExpenseInvoice:
                        return INVOICE_PREFIX;
                    case FinancialTransactionType.Receipt:
                        return RECEIPT_PREFIX;
                    case FinancialTransactionType.CreditNote:
                        return CREDITNOTE_PREFIX;
                    case FinancialTransactionAgg.FinancialTransactionType.BadDebt:
                        return BADDEBT_PREFIX;
                }
                return "";
            }
            set
            {
                switch (this.FinancialTransactionType)
                {
                    case FinancialTransactionType.FeeInvoice:
                        value = INVOICE_PREFIX;
                        break;
                    case FinancialTransactionType.BillableExpenseInvoice:
                        value = INVOICE_PREFIX;
                        break;
                    case FinancialTransactionType.Receipt:
                        value = RECEIPT_PREFIX;
                        break;
                    case FinancialTransactionType.CreditNote:
                        value = CREDITNOTE_PREFIX;
                        break;
                    case FinancialTransactionAgg.FinancialTransactionType.BadDebt:
                        value = BADDEBT_PREFIX;
                        break;
                    default:
                        value = "";
                        break;
                }
            }
        }
    }
}