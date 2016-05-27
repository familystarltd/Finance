using Finance.Domain.Aggregates.CustomerAgg;
using System.Collections.Generic;
using System.Domain;
namespace Finance.Domain.Aggregates.FeeAgg
{
    public class Funder : Entity
    {
        /// <summary>
        /// Get or set the Given short name of this Funder
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the contact of this Funder
        /// </summary>
        public virtual Contact PersonalContact { get; set; }
        /// <summary>
        /// Get or set the Invoice ContactReference to this Funder (It may be empty if the ContactReference is refer to the invoice)
        /// </summary>
        public string InvoiceContactReference { get; set; }
        /// <summary>
        /// Get or Set the Fee Invoice Billing Contact for this Funder
        /// </summary>
        public virtual Contact FeeInvoiceBillingContact { get; set; }
        /// <summary>
        /// Get or Set the Fee Invoice Delivery Contact for this Funder
        /// </summary>
        public virtual Contact FeeInvoiceDeliveryContact { get; set; }
        /// <summary>
        /// Get or Set the Disbursements Invoice Billing Contact for this Funder
        /// </summary>
        public virtual Contact DisbursementInvoiceBillingContact { get; set; }
        /// <summary>
        /// Get or Set the Disbursements Invoice Delivery Contact for this Funder
        /// </summary>
        public virtual Contact DisbursementInvoiceDeliveryContact { get; set; }
        /// <summary>
        /// Get or set the Fees information to this Funder. May have one or more Fees and also maintain the History of Fees
        /// </summary>
        public virtual ICollection<Fee> Fees { get; set; }
    }
}