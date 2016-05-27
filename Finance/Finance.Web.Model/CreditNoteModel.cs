using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Web.Model
{
    public class CreditNoteViewModel
    {
        public string CompanyName { get; set; }
        public PaginationHeader Pagination { get; set; }
        public CreditNoteModel CreditNote { get; set; }
        public IEnumerable<CreditNoteModel> CreditNotes { get; set; }
        public string FunderName { get; set; }
        public string CustomerName { get; set; }
        public Guid? FunderId { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public string FromDate { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public string ToDate { get; set; }
        public bool Reset { get; set; }
    }
    public class CreditNoteModel : FinancialTransactionModel
    {
        /// <summary>
        /// Get or Set the Number to this Credit Note. The Number will get the max value and add one from the storage
        /// </summary>
        public int CreditNoteNo { get; set; }
        /// <summary>
        /// Get or Set the Date and Time of this Credit Note.
        /// </summary>
        public DateTime CreditDateTime { get; set; }
        /// <summary>
        /// Get or Set the Fee Invoice which is Credit relates to this Credit Note.
        /// </summary>
        public Guid? InvoiceId { get; set; }
        public InvoiceModel Invoice { get; set; }
        public decimal InvoiceCreditAmount { get; set; }
        public virtual ICollection<ReceiptModel> Receipts { get; set; }
        public decimal CreditAvailable { get; set; }  //{ get { return Receipts == null || Receipts.Count == 0 ? this.Amount : this.Amount - Receipts.Sum(r => r.Amount); } }
    }
}
