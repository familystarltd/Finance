using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Model
{
    public enum InvoiceTypeModel : int
    {
        None=0,
        Fee = 1,
        FNC = 2,
        Expenses = 3,
        Manual = 4
    }
    public class InvoiceModel : FinancialTransactionModel
    {
        public virtual InvoiceTypeModel InvoiceType { get; set; }
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? InvoiceMonth { get; set; }
        public string PayContactReference { get; set; }
        public DateTime? DueDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public bool IsManual { get; set; }
        //abstract public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<InvoiceDetailModel> InvoiceDetails { get; set; }
        public virtual ICollection<ReceiptInvoiceModel> ReceiptInvoices { get; set; }
        public virtual ICollection<CreditNoteModel> CreditNotes { get; set; }
        public virtual ICollection<InvoiceAdjustmentModel> Adjustments { get; set; }
        public virtual BadDebtModel BadDebt { get; set; }
        public DateTime? LogDate { get; set; }
        public string LogUser { get; set; }

        #region CALCULATED FIELDS
        public decimal TotalAdjustment
        {
            get
            {
                return Adjustments == null ? 0 : Adjustments.Sum(adj => adj.Amount);
            }
            set
            {
                value = Adjustments == null ? 0 : Adjustments.Sum(adj => adj.Amount);
            }
        }
        public decimal VATAmount
        {
            get
            {
                return this.TotalWithVAT - this.NetTotal;
            }
        }

        /// <summary>
        /// Total before VAT
        /// </summary>
        public decimal NetTotal
        {
            get
            {
                return InvoiceDetails == null ? 0 : InvoiceDetails.Sum(i => i.Total);
            }
        }

        public decimal AdvancePayment { get; set; }

        /// <summary>
        /// Total with VAT
        /// </summary>
        public decimal TotalWithVAT
        {
            get;
            set;
            //get
            //{
            //    if (InvoiceDetails == null)
            //        return 0;

            //    return InvoiceDetails.Sum(i => i.TotalPlusVAT);
            //}
        }
        /// <summary>
        /// Total with VAT minus advanced payment 
        /// </summary>
        public decimal DueAmount
        {
            get
            {
                decimal amountReceipt = (this.ReceiptInvoices == null || this.ReceiptInvoices.Count <= 0 ? 0 : this.ReceiptInvoices.Sum(r => r.AmountReceived));
                return TotalWithVAT - (AdvancePayment + amountReceipt);
            }
        }
        #endregion
    }
}
