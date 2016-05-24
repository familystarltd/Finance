using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Model
{
    public class ReceiptViewModel
    {
        public string CompanyName { get; set; }
        public PaginationHeader Pagination { get; set; }
        public ReceiptModel Receipt { get; set; }
        public IEnumerable<ReceiptModel> Receipts { get; set; }
        public string FunderName { get; set; }
        public string CustomerName { get; set; }
        public Guid? FunderId { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public string FromDate { get; set; }
        //[DateFormat]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public string ToDate { get; set; }
        public bool Reset { get; set; }
    }
    public enum ReceiptPayMethodModel : int
    {
        BankTransfer = 1,
        Cash = 2,
        CreditNote = 3,
        Cheque = 4
    }
    public class ReceiptModel : FinancialTransactionModel
    {
        /// <summary>
        /// Get or Set the Number to this Receipt. The Number will get the max value and add one from the storage
        /// </summary>
        public int ReceiptNo { get; set; }
        /// <summary>
        /// Get or Set the Pay method of this Receipt.
        /// </summary>
        public ReceiptPayMethodModel PaymentMethod { get; set; }
        /// <summary>
        /// Get or Set the Date and Time of this Receipt.
        /// </summary>
        public DateTime ReceiptDateTime { get; set; }
        /// <summary>
        /// Get or Set the ContactReference of this Receipt.
        /// </summary>
        public string ContactReference { get; set; }
        /// <summary>
        /// Get or Set the collection of ReceiptFeeInvoice
        /// </summary>
        public ICollection<ReceiptInvoiceModel> ReceiptInvoices { get; set; }
        /// <summary>
        /// Apply CreditNote
        /// </summary>
        public CreditNoteModel CreditNote { get; set; }
        public Guid? CreditNoteId { get; set; }

        /// <summary>
        /// The collections of CreditNotes are created by Receipt in case of Excess amount
        /// </summary>
        public virtual ICollection<CreditNoteModel> CreditNotes { get; set; }
    }
    public class ReceiptInvoiceModel
    {
        public Guid Id { get; set; }
        public Guid ReceiptId { get; set; }
        public virtual ReceiptModel Receipt { get; set; }
        public Guid InvoiceId { get; set; }
        public virtual InvoiceModel Invoice { get; set; }
        /// <summary>
        /// Amount received for each invoice
        /// </summary>
        public decimal AmountReceived { get; set; }
    }
}