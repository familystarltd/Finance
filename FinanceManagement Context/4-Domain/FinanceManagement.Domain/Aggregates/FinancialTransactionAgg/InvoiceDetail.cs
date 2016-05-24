using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Domain;
namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public class InvoiceDetail : Entity
    {
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        public string Article { get; set; }

        [DefaultValue(20), Range(0.00, 100, ErrorMessage = "VAT must be a % between 0 and 100")]
        public decimal VAT { get; set; }
        public decimal Total { get; set; }
        #region Calculated fields
        public decimal VATAmount
        {
            get
            {
                return TotalPlusVAT - Total;
            }
        }

        public decimal TotalPlusVAT
        {
            get
            {
                return Total * (1 + VAT / 100);
            }
        }
        #endregion
    }
}
