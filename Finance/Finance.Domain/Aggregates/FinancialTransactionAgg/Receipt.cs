using Finance.Domain.Aggregates.FeeAgg;
using System;
using System.Collections.Generic;
using System.Domain;
namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    public enum ReceiptPayMethod : int
    {
        BankTransfer = 1,
        Cash = 2,
        CreditNote = 3,
        Cheque = 4
    }
    /// <summary>
    /// Receipt will be created for each payment made for Fee or Disbursements Invoice(s)
    /// One Receipt may be created for multiple invoices
    /// One invoice may have one or many Receipts (Part payments)
    /// </summary>
    public class Receipt : FinancialTransaction
    {
        public override FinancialTransactionType FinancialTransactionType { get { return FinancialTransactionAgg.FinancialTransactionType.Receipt; } set { value = FinancialTransactionAgg.FinancialTransactionType.Receipt; } }
        /// <summary>
        /// Get or Set the Number to this Receipt. The Number will get the max value and add one from the storage
        /// </summary>
        int receiptNo;
        public int ReceiptNo { get { return receiptNo; } set { receiptNo = value; this.TransactionRef = value; } }
        public ReceiptPayMethod PaymentMethod { get; set; }
        /// <summary>
        /// Get or Set the ContactReference of this Receipt.
        /// </summary>
        public string ContactReference { get; set; }
        /// <summary>
        /// Get or Set the collection of ReceiptFeeInvoice
        /// </summary>
        public virtual ICollection<ReceiptInvoice> ReceiptInvoices { get; set; }
        /// <summary>
        /// Apply CreditNote, if the paymethod is CreditNote
        /// </summary>
        public virtual CreditNote CreditNote { get; set; }
        /// <summary>
        /// Apply CreditNote, if the paymethod is CreditNote
        /// </summary>
        public Guid? CreditNoteId { get; set; }

        /// <summary>
        /// The collections of CreditNotes are created by Receipt in case of Excess amount
        /// </summary>
        public virtual ICollection<CreditNote> CreditNotes { get; set; }

    }
    public class ReceiptInvoice : Entity
    {
        public Guid ReceiptId { get; set; }
        public virtual Receipt Receipt { get; set; }
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        /// <summary>
        /// Amount received for each invoice
        /// </summary>
        public decimal AmountReceived { get; set; }
    }
}