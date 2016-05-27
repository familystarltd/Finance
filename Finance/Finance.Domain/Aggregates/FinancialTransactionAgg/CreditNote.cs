using Finance.Domain.Aggregates.FeeAgg;
using System;
using System.Collections.Generic;
using System.Domain;
using System.Linq;
namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    public class CreditNote : FinancialTransaction
    {
        public override FinancialTransactionType FinancialTransactionType
        {
            get { return FinancialTransactionAgg.FinancialTransactionType.CreditNote; }
            set { value = FinancialTransactionAgg.FinancialTransactionType.CreditNote; }
        }
        /// <summary>
        /// Get or Set the Number to this Credit Note. The Number will get the max value and add one from the storage
        /// </summary>
        int creditNoteNo;
        public int CreditNoteNo { get { return creditNoteNo; } set { creditNoteNo = value; this.TransactionRef = value; } }
        public virtual Guid? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        /// <summary>
        /// If the Credit Not has been created by Receipt in case of Excess amount, then Receipt would be assigned here
        /// </summary>
        public virtual Receipt Receipt { get; set; }
        /// <summary>
        /// If the Credit Not has been created by Receipt in case of Excess amount, then ReceiptId would be assigned here
        /// </summary>
        public virtual Guid? ReceiptId { get; set; }
        /// <summary>
        /// If the Receipt payment method is CreditNote, then only Receipt would be assigned here
        /// </summary>
        public virtual ICollection<Receipt> Receipts { get; set; }
        public decimal CreditAvailable { get { return Receipts == null || Receipts.Count == 0 ? this.Amount : this.Amount - Receipts.Sum(r => r.TransactionStatus == FinancialTransactionAgg.TransactionStatus.Void ? 0 : r.Amount); } }
    }
}