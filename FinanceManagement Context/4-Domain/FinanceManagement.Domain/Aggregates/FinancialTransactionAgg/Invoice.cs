using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public enum InvoiceType: int
    {
        Fee = 1,
        FNC = 2,
        BillableExpenses = 3,
        Manual= 4
    }
    public enum InvoiceStatus : int
    {
        Draft = 1,
        Void = 2,
        Cancel = 3,
        Approved = 4,
        Paid = 5
    }
    public class Invoice : FinancialTransaction
    {
        public virtual InvoiceType InvoiceType { get; set; }
        public override string Discriminator
        {
            get
            {
                switch (this.InvoiceType)
                {
                    case InvoiceType.Fee:
                        return "Fee";
                    case InvoiceType.FNC:
                        return "FNC";
                    case InvoiceType.BillableExpenses:
                        return "Billable Expenses";
                    case InvoiceType.Manual:
                        return "Manual";
                }
                return "N/A";
            }
            set
            {
                switch (this.InvoiceType)
                {
                    case InvoiceType.Fee:
                        value = "Fee";
                        break;
                    case InvoiceType.FNC:
                        value = "FNC";
                        break;
                    case InvoiceType.BillableExpenses:
                        value = "Billable Expenses";
                        break;
                    case InvoiceType.Manual:
                        value = "Manual";
                        break;
                    default:
                        value = "N/A";
                        break;
                }
            }
        }
        int invoiceNo;
        public int InvoiceNo { get { return invoiceNo; } set { invoiceNo = value; this.TransactionRef = value; } }
        public DateTime InvoiceDate { get; set; }
        public DateTime? InvoiceMonth { get; set; }
        public string PayContactReference { get; set; }
        public DateTime? DueDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public bool IsManual { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<ReceiptInvoice> ReceiptInvoices { get; set; }
        public virtual ICollection<CreditNote> CreditNotes { get; set; }
        public virtual ICollection<InvoiceAdjustment> Adjustments { get; set; }
        public virtual BadDebt BadDebt { get; set; }
        public DateTime? LogDate { get; set; }

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
            get
            {
                if (InvoiceDetails == null)
                    return 0;

                return InvoiceDetails.Sum(i => i.TotalPlusVAT);
            }
        }
        /// <summary>
        /// Total with VAT minus advanced payment 
        /// </summary>
        public decimal DueAmount
        {
            get
            {
                decimal amountReceipt = (this.ReceiptInvoices == null || this.ReceiptInvoices.Count <= 0 ? 0 : this.ReceiptInvoices.Sum(r => r.Receipt.TransactionStatus == FinancialTransactionAgg.TransactionStatus.Void ? 0 :  r.AmountReceived));
                return TotalWithVAT - (AdvancePayment + amountReceipt + (this.BadDebt == null ? 0 : BadDebt.Amount));
            }
        }
        #endregion
    }
}
